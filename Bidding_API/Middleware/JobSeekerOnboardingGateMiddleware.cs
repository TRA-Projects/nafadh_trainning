using System;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Security.Claims;
using System.Text.Json;
using System.Threading.Tasks;
using Bidding_DLL.Services.Interfaces;
using Bidding_Models;
using Microsoft.AspNetCore.Http;

namespace Bidding_API.Middleware
{
    /// <summary>
    /// Job seekers may hold a JWT before email verification / profile completion, but only
    /// allowlisted endpoints are reachable until each gate is cleared.
    /// </summary>
    public sealed class JobSeekerOnboardingGateMiddleware
    {
        /// <summary>Allowed while email is still unverified.</summary>
        private static readonly PathString[] EmailPendingAllowedPrefixes =
        {
            new PathString("/api/v2/auth/me"),
            new PathString("/api/v2/auth/refresh-token"),
            new PathString("/api/v2/auth/verify-email"),
            // Full registration surface (form reopen / civil lookup / OTP) + public config lookups.
            new PathString("/api/v2/registration"),
            new PathString("/api/jobseekers/admin/system-config"),
            new PathString("/api/jobseekers/admin/system-variables"),
        };

        /// <summary>Allowed while email is verified but profile wizard is incomplete.</summary>
        private static readonly PathString[] ProfilePendingAllowedPrefixes =
        {
            new PathString("/api/v2/auth/me"),
            new PathString("/api/v2/auth/refresh-token"),
            new PathString("/api/v2/auth/profiles"),
            new PathString("/api/v2/profile"),
            new PathString("/api/v2/registration/auth-settings"),
            new PathString("/api/jobseekers/profile-completion"),
            new PathString("/api/jobseekers/users/profile-completion-done"),
            new PathString("/api/jobseekers/profile"),
            new PathString("/api/jobseekers/attachments"),
            new PathString("/api/jobseekers/suggestions"),
            new PathString("/api/jobseekers/admin/system-variables"),
            new PathString("/api/jobseekers/admin/system-config"),
            new PathString("/api/jobseekers/admin/skills"),
            new PathString("/api/jobseekers/admin/skill-categories"),
            // Current Location catalog on /complete-profile (Governorates / Wilayats / Cities).
            new PathString("/api/Location"),
        };

        private readonly RequestDelegate _next;

        public JobSeekerOnboardingGateMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(
            HttpContext context,
            IUserRepository userRepository,
            IProfileV2Repository profileRepository)
        {
            var user = context.User;
            if (user?.Identity?.IsAuthenticated != true)
            {
                await _next(context);
                return;
            }

            if (!IsJobSeekerVendorType(user))
            {
                await _next(context);
                return;
            }

            var email = user.FindFirstValue(ClaimTypes.Email)
                ?? user.FindFirstValue(JwtRegisteredClaimNames.Email)
                ?? user.FindFirstValue("email");

            if (string.IsNullOrWhiteSpace(email))
            {
                await WriteForbiddenAsync(context, "EmailNotVerified", "Email is not verified. check your mailbox");
                return;
            }

            var appUser = await userRepository.GetPrimaryUserByProfileEmailAsync(email.Trim());
            if (appUser == null || !appUser.EmailConfirmed)
            {
                if (!IsAllowedPath(context.Request.Path, EmailPendingAllowedPrefixes))
                {
                    await WriteForbiddenAsync(context, "EmailNotVerified", "Email is not verified. check your mailbox");
                    return;
                }

                await _next(context);
                return;
            }

            var profileId = ResolveProfileId(user, appUser.ProfileId);
            if (profileId <= 0)
            {
                await _next(context);
                return;
            }

            // Always read a fresh flag — pooled DbContext trackers can otherwise keep a stale false.
            var profileCompleted = await profileRepository.IsUserProfileCompletedAsync(profileId);
            if (profileCompleted == false)
            {
                if (!IsAllowedPath(context.Request.Path, ProfilePendingAllowedPrefixes))
                {
                    await WriteForbiddenAsync(
                        context,
                        "ProfileNotCompleted",
                        "Profile is not completed. Finish your profile to continue.");
                    return;
                }
            }

            await _next(context);
        }

        private static int ResolveProfileId(ClaimsPrincipal user, int? appUserProfileId)
        {
            var raw = user.FindFirstValue("ProfileId");
            if (!string.IsNullOrWhiteSpace(raw) && int.TryParse(raw, out var fromClaim) && fromClaim > 0)
            {
                return fromClaim;
            }

            return appUserProfileId ?? 0;
        }

        private static bool IsAllowedPath(PathString path, PathString[] allowedPrefixes)
        {
            foreach (var allowed in allowedPrefixes)
            {
                if (path.StartsWithSegments(allowed, StringComparison.OrdinalIgnoreCase))
                {
                    return true;
                }
            }

            return false;
        }

        private static bool IsJobSeekerVendorType(ClaimsPrincipal user)
        {
            var raw = user.FindFirstValue("VendorType");
            if (string.IsNullOrWhiteSpace(raw) || !int.TryParse(raw, out var vendorType))
            {
                return false;
            }

            return vendorType == (int)Enums.ProfileTypes.FreeLancer
                || vendorType == (int)Enums.ProfileTypes.JobSeeker;
        }

        private static async Task WriteForbiddenAsync(HttpContext context, string code, string message)
        {
            context.Response.StatusCode = (int)HttpStatusCode.Forbidden;
            context.Response.ContentType = "application/json";
            var payload = JsonSerializer.Serialize(new
            {
                Code = code,
                Message = message,
                Description = message,
            });
            await context.Response.WriteAsync(payload);
        }
    }
}
