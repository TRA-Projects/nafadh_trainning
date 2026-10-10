using Bidding_Core.Interfaces;
using Bidding_Core.Repository;
using Bidding_Core.Services.Implementations;
using Bidding_Core.Services.Interfaces;
using Bidding_DLL.Data_Access_Layer;
using Bidding_DLL.Services.Implementations;
using Bidding_DLL.Services.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace Bidding_API.Common
{
    public static class ServiceExtensions
    {
        /// <summary>Data-access classes (Bidding_DLL / Data Access Layer).</summary>
        public static void InjectDLLComponents(this IServiceCollection services)
        {
            services.AddTransient<UserRefreshTokenDAC>();
            services.AddTransient<NotificationTypeDAC>();
            services.AddTransient<SMSEmailNotificationDAC>();
            services.AddTransient<TemplateDAC>();
        }

        /// <summary>Repositories (interface → implementation) and domain services.</summary>
        public static void InjectInterfacesComponents(this IServiceCollection services)
        {
            services.AddTransient<IUserRefreshToken, UserRefreshTokenRepository>();
            services.AddTransient<INotificationType, NotificationTypeRepository>();
            services.AddTransient<ITemplate, TemplateRepository>();
            services.AddTransient<ISMSEmailNotification, SMSEmailNotificationRepository>();

            services.AddTransient<IUserRepository, UserRepository>();
            services.AddTransient<IProfileV2Repository, ProfileV2Repository>();

            services.AddTransient<IAuthService, AuthService>();
            services.AddTransient<IRegistrationUserService, RegistrationUserService>();
            services.AddTransient<INafathJobSeekerService, NafathJobSeekerService>();
            services.AddTransient<INotificationService, NotificationService>();

            services.AddScoped<INotificationDispatchService, NotificationDispatchService>();
            services.AddScoped<ICustomNotificationService, CustomNotificationService>();
            services.AddSingleton<IShowcaseService, ShowcaseMockService>();
            services.AddScoped<INotificationTemplateRenderer, NotificationTemplateRenderer>();
            services.AddScoped<IAdminSystemService, AdminSystemService>();
            services.AddScoped<IIdentityAccessAdminService, IdentityAccessAdminService>();
        }
    }
}
