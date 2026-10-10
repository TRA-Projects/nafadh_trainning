using Bidding_API.Common;
using Bidding_API.ExceptionHandler;
using Bidding_API.jwtConfigurations;
using Bidding_API.jwtConfigurations.Implementation;
using Bidding_API.jwtConfigurations.Interfaces;
using Bidding_API.jwtConfigurationss.Implementation;
using Bidding_Core;
using Bidding_DLL;
using Bidding_DLL.Seeding;
using Bidding_Models;
using Bidding_Models.DatabaseModel;
using ConstructionHub_Core.Helpers;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json.Serialization;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Bidding_API
{
    public class Startup
    {
        public Startup(IConfiguration Configuration, IWebHostEnvironment env)
        {
            configuration = Configuration;

            var logDir = Helpers.NLogBootstrap.Configure();
            NLog.LogManager.GetCurrentClassLogger().Info(
                "NLog initialized. logDir={0} contentRoot={1} appBase={2}",
                logDir,
                env.ContentRootPath,
                AppContext.BaseDirectory);
        }

        public IConfiguration configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            // Live MOL/Nafath can be slow from local/dev networks; keep above Angular civil-lookup timeout.
            services.AddHttpClient("NafathJobSeeker", c =>
            {
                c.Timeout = TimeSpan.FromSeconds(60);
                c.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            });

            services.AddCors(options =>
            {
                var configuredOrigins = FrontendOriginsConfiguration.GetAllowedOrigins(configuration);

                options.AddPolicy("AllowFrontend", builder =>
                    builder.WithOrigins(configuredOrigins)
                           .AllowCredentials()
                           .AllowAnyHeader()
                           .AllowAnyMethod());
            });

            services.AddAutoMapper(typeof(Startup));
            services.AddControllers().AddNewtonsoftJson(options =>
            {
                options.SerializerSettings.ContractResolver = new DefaultContractResolver();
                options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore;
            });
            services.AddDistributedMemoryCache();
            services.AddSession(options =>
            {
                options.IdleTimeout = TimeSpan.FromMinutes(15);
                options.Cookie.HttpOnly = true;
                options.Cookie.SameSite = SameSiteMode.None;
                options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
                options.Cookie.IsEssential = true;
            });

            services.Configure<IdentityOptions>(options =>
            {
                options.Password.RequireDigit = true;
                options.Password.RequiredLength = 8;
                options.Password.RequireNonAlphanumeric = true;
                options.Password.RequireUppercase = true;
                options.Password.RequireLowercase = true;
                options.SignIn.RequireConfirmedPhoneNumber = true;
                options.User.RequireUniqueEmail = true;
            });

            services.AddDbContextPool<CHDBContext>(options => options.UseSqlServer(
                configuration["ConnectionStrings:SqlConnection"],
                b => { b.MigrationsAssembly("Bidding_DLL"); b.UseRowNumberForPaging(); }));
            services.AddIdentity<AppUser, Roles>().AddEntityFrameworkStores<CHDBContext>().AddDefaultTokenProviders();

            services.Configure<DataProtectionTokenProviderOptions>(opt => opt.TokenLifespan = TimeSpan.FromHours(2));

            var jwtSection = configuration.GetSection("JwtBearerTokenSettings");
            services.Configure<JwtBearerTokenSettings>(jwtSection);
            var jwtBearerTokenSettings = jwtSection.Get<JwtBearerTokenSettings>();
            var bypassJwtValidation = configuration.GetValue<bool>("BypassJwtValidation");

            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
                .AddJwtBearer(options =>
                {
                    options.RequireHttpsMetadata = false;
                    options.SaveToken = true;
                    if (bypassJwtValidation)
                    {
                        // Dev-only: accept any bearer string. Do not parse wire token as JWT.
                        options.TokenValidationParameters = new TokenValidationParameters
                        {
                            ValidateIssuer = false,
                            ValidateAudience = false,
                            ValidateLifetime = false,
                            ValidateIssuerSigningKey = false,
                            RequireSignedTokens = false,
                            RequireExpirationTime = false,
                            SignatureValidator = (_, __) => CreateDevBypassJwt(),
                        };
                    }
                    else
                    {
                        options.TokenValidationParameters = new TokenValidationParameters()
                        {
                            ValidateIssuer = true,
                            ValidIssuer = jwtBearerTokenSettings.Issuer,
                            ValidateAudience = true,
                            ValidAudience = jwtBearerTokenSettings.Audience,
                            ValidateIssuerSigningKey = true,
                            IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(JwtBearerTokenSecretKey.AccessKeyTokenSecret)),
                            ValidateLifetime = true,
                            ClockSkew = TimeSpan.Zero
                        };
                    }
                    options.Events = new JwtBearerEvents
                    {
                        OnAuthenticationFailed = context =>
                        {
                            if (context.Exception.GetType() == typeof(SecurityTokenExpiredException))
                            {
                                context.Response.Headers.Add("Token-Expired", "true");
                            }
                            return Task.CompletedTask;
                        }
                    };
                });

            services.AddSingleton<DataProtectionPurposeString>();
            services.AddScoped<ICommonActions, CommonActions>();
            services.AddSingleton<ILoggerManager, LoggerManager>();
            services.AddSingleton<SupportTicketService>();

            services.AddSingleton<SMSSettings>();
            services.AddSingleton<EmailSettings>();
            services.AddSingleton<EmailSettingsVerifications>();

            services.AddSingleton<ITokenGenerator, TokenGenerator>();
            services.AddSingleton<IAccessTokenService, AccessTokenService>();
            services.AddSingleton<IRefreshTokenService, RefreshTokenService>();
            services.AddSingleton<IRefreshTokenValidator, RefreshTokenValidator>();
            services.AddSingleton(jwtBearerTokenSettings);

            services.InjectInterfacesComponents();
            services.InjectDLLComponents();

            services.AddHostedService<Bidding_API.BackgroundJobs.CustomNotificationDispatchService>();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, CHDBContext context, ILoggerManager logger, IConfiguration configuration)
        {
            logger.LogInfo($"API started. Environment={env.EnvironmentName}; ContentRoot={env.ContentRootPath}");

            // The database schema and EF migrations are owned by the main (Jobseekers) solution.
            // This API never migrates; it only upserts Identity & Access catalog rows when enabled.
            if (configuration.GetValue<bool>("RunIdentityAccessSeeder"))
            {
                using (context)
                {
                    IdentityAccessDbSeeder.ApplyAsync(context).GetAwaiter().GetResult();
                }
            }

            app.ConfigureExceptionHandler(logger, configuration);

            app.UseStaticFiles();
            app.UseRouting();
            app.UseCors("AllowFrontend");
            app.UseSession();

            app.UseAuthentication();
            app.UseAuthorization();
            app.UseMiddleware<Bidding_API.Middleware.JobSeekerOnboardingGateMiddleware>();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }

        /// <summary>Dev bypass only — never parse the incoming Authorization value as JWT JSON.</summary>
        private static JwtSecurityToken CreateDevBypassJwt()
        {
            return new JwtSecurityToken(
                issuer: "dev-bypass",
                audience: "dev-bypass",
                claims: new[]
                {
                    new Claim(ClaimTypes.Name, "dev-bypass"),
                    new Claim("VendorType", "0"),
                },
                notBefore: DateTime.UtcNow.AddMinutes(-5),
                expires: DateTime.UtcNow.AddHours(12));
        }
    }
}
