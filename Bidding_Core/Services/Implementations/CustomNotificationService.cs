using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Bidding_API.Helpers;
using Bidding_Core.Helpers;
using Bidding_Core.Services.Interfaces;
using Bidding_DLL;
using Bidding_Models;
using Bidding_Models.DatabaseModel.Notifications;
using Bidding_Models.Jobseekers;
using Microsoft.EntityFrameworkCore;

namespace Bidding_Core.Services.Implementations
{
    public class CustomNotificationService : ICustomNotificationService
    {
        private const int DefaultPageSize = 5;
        private const int MaxPageSize = 100;
        private const int DueBatchSize = 5;
        private const int SaveEvery = 50;

        private readonly CHDBContext _context;
        private readonly INotificationDispatchService _dispatch;

        public CustomNotificationService(CHDBContext context, INotificationDispatchService dispatch)
        {
            _context = context;
            _dispatch = dispatch;
        }

        public async Task<PagedListResponseDto<CustomNotificationRowDto>> GetPageAsync(
            int? pageIndex,
            int? pageSize,
            string q,
            string status,
            string channel)
        {
            var query = _context.CustomNotifications.AsNoTracking().AsQueryable();

            var search = (q ?? string.Empty).Trim();
            if (search.Length > 0)
            {
                query = query.Where(n => n.Title_en.Contains(search) || n.Title_ar.Contains(search));
            }

            var statusFilter = (status ?? string.Empty).Trim();
            if (statusFilter.Length > 0)
            {
                query = query.Where(n => n.Status == statusFilter);
            }

            switch ((channel ?? string.Empty).Trim().ToLowerInvariant())
            {
                case "email":
                    query = query.Where(n => n.SendEmail);
                    break;
                case "sms":
                    query = query.Where(n => n.SendSms);
                    break;
                case "system":
                    query = query.Where(n => n.SendSystem);
                    break;
            }

            var (index, size) = NormalizePaging(pageIndex, pageSize);
            var total = await query.CountAsync();
            var maxIndex = Math.Max(0, (int)Math.Ceiling(total / (double)size) - 1);
            index = Math.Min(index, maxIndex);

            var rows = await query
                .Include(n => n.Targets)
                .OrderByDescending(n => n.CreatedAtUtc)
                .ThenByDescending(n => n.Id)
                .Skip(index * size)
                .Take(size)
                .ToListAsync();

            return new PagedListResponseDto<CustomNotificationRowDto>
            {
                rows = rows.Select(n => MapRow(n, new CustomNotificationRowDto())).ToList(),
                total = total,
                pageIndex = index,
                pageSize = size,
            };
        }

        public async Task<CustomNotificationDto> GetAsync(long id)
        {
            var entity = await _context.CustomNotifications.AsNoTracking()
                .Include(n => n.Targets)
                .FirstOrDefaultAsync(n => n.Id == id);
            if (entity == null)
            {
                throw new SafeExecutionHelper.NotFoundException("Notification not found.");
            }

            return MapDetail(entity);
        }

        public async Task<CustomNotificationDto> CreateAsync(UpsertCustomNotificationRequestDto body, string callerUserId)
        {
            var utcNow = DateTime.UtcNow;
            var (targets, scheduledAtUtc) = await ValidateRequestAsync(body, utcNow);

            var entity = new CustomNotification
            {
                Status = CustomNotificationStatuses.Scheduled,
                CreatedByUserId = callerUserId,
                CreatedAtUtc = utcNow,
            };
            Apply(entity, body, targets, scheduledAtUtc, utcNow);

            _context.CustomNotifications.Add(entity);
            await _context.SaveChangesAsync();
            return MapDetail(entity);
        }

        public async Task<CustomNotificationDto> UpdateAsync(long id, UpsertCustomNotificationRequestDto body)
        {
            var entity = await _context.CustomNotifications
                .Include(n => n.Targets)
                .FirstOrDefaultAsync(n => n.Id == id);
            if (entity == null)
            {
                throw new SafeExecutionHelper.NotFoundException("Notification not found.");
            }

            EnsureEditable(entity);

            var utcNow = DateTime.UtcNow;
            var (targets, scheduledAtUtc) = await ValidateRequestAsync(body, utcNow);

            _context.CustomNotificationTargets.RemoveRange(entity.Targets);
            entity.Targets = new List<CustomNotificationTarget>();
            Apply(entity, body, targets, scheduledAtUtc, utcNow);

            await _context.SaveChangesAsync();
            return MapDetail(entity);
        }

        public async Task<CustomNotificationDto> CancelAsync(long id)
        {
            var entity = await _context.CustomNotifications
                .Include(n => n.Targets)
                .FirstOrDefaultAsync(n => n.Id == id);
            if (entity == null)
            {
                throw new SafeExecutionHelper.NotFoundException("Notification not found.");
            }

            EnsureEditable(entity);

            entity.Status = CustomNotificationStatuses.Cancelled;
            entity.UpdatedAtUtc = DateTime.UtcNow;
            await _context.SaveChangesAsync();
            return MapDetail(entity);
        }

        public async Task<CustomNotificationRecipientPreviewDto> PreviewRecipientsAsync(IEnumerable<CustomNotificationTargetDto> targets)
        {
            var normalized = CustomNotificationRules.NormalizeTargets(targets);
            var (users, orgs, jobSeekers) = await ResolveRecipientsAsync(normalized);
            var registered = users.Select(u => u.UserId).Distinct().Count();
            return new CustomNotificationRecipientPreviewDto
            {
                RegisteredUsers = registered,
                EmailOnlyOrganizations = orgs.Count,
                UnregisteredJobSeekers = jobSeekers.Count,
                Total = registered + orgs.Count + jobSeekers.Count,
            };
        }

        public async Task<int> ProcessDueAsync(DateTime utcNow)
        {
            var dueIds = await _context.CustomNotifications.AsNoTracking()
                .Where(n => n.Status == CustomNotificationStatuses.Scheduled && n.ScheduledAtUtc <= utcNow)
                .OrderBy(n => n.ScheduledAtUtc)
                .Select(n => n.Id)
                .Take(DueBatchSize)
                .ToListAsync();

            var processed = 0;
            foreach (var id in dueIds)
            {
                // Atomic claim so two API instances never send the same notification twice.
                var claimed = await _context.Database.ExecuteSqlInterpolatedAsync(
                    $"UPDATE CustomNotifications SET Status = {CustomNotificationStatuses.Processing}, UpdatedAtUtc = {utcNow} WHERE Id = {id} AND Status = {CustomNotificationStatuses.Scheduled}");
                if (claimed == 0)
                {
                    continue;
                }

                await ProcessOneAsync(id);
                processed++;
            }

            return processed;
        }

        public async Task<PagedListResponseDto<UserNotificationDto>> GetMyNotificationsAsync(
            string userId,
            int? pageIndex,
            int? pageSize,
            bool unreadOnly)
        {
            EnsureCaller(userId);

            var query = _context.UserNotifications.AsNoTracking().Where(n => n.UserId == userId);
            if (unreadOnly)
            {
                query = query.Where(n => !n.IsRead);
            }

            var (index, size) = NormalizePaging(pageIndex, pageSize);
            var total = await query.CountAsync();
            var maxIndex = Math.Max(0, (int)Math.Ceiling(total / (double)size) - 1);
            index = Math.Min(index, maxIndex);

            var rows = await query
                .OrderByDescending(n => n.CreatedAtUtc)
                .ThenByDescending(n => n.Id)
                .Skip(index * size)
                .Take(size)
                .ToListAsync();

            return new PagedListResponseDto<UserNotificationDto>
            {
                rows = rows.Select(MapInbox).ToList(),
                total = total,
                pageIndex = index,
                pageSize = size,
            };
        }

        public Task<int> GetMyUnreadCountAsync(string userId)
        {
            EnsureCaller(userId);
            return _context.UserNotifications.AsNoTracking().CountAsync(n => n.UserId == userId && !n.IsRead);
        }

        public async Task<UserNotificationDto> MarkReadAsync(string userId, long id)
        {
            EnsureCaller(userId);

            var row = await _context.UserNotifications.FirstOrDefaultAsync(n => n.Id == id);
            if (row == null || !CustomNotificationRules.CanUserAccessInboxRow(row.UserId, userId))
            {
                throw new SafeExecutionHelper.NotFoundException("Notification not found.");
            }

            if (!row.IsRead)
            {
                row.IsRead = true;
                row.ReadAtUtc = DateTime.UtcNow;
                await _context.SaveChangesAsync();
            }

            return MapInbox(row);
        }

        public Task<int> MarkAllReadAsync(string userId)
        {
            EnsureCaller(userId);
            var utcNow = DateTime.UtcNow;
            return _context.Database.ExecuteSqlInterpolatedAsync(
                $"UPDATE UserNotifications SET IsRead = 1, ReadAtUtc = {utcNow} WHERE UserId = {userId} AND IsRead = 0");
        }

        private async Task ProcessOneAsync(long id)
        {
            var entity = await _context.CustomNotifications
                .Include(n => n.Targets)
                .FirstOrDefaultAsync(n => n.Id == id);
            if (entity == null)
            {
                return;
            }

            var sent = 0;
            var failed = 0;
            var skipped = 0;
            try
            {
                var targets = entity.Targets.Select(ToDto).ToList();
                var (users, orgs, jobSeekers) = await ResolveRecipientsAsync(targets);
                var plan = CustomNotificationRules.Plan(entity, users, orgs, jobSeekers);
                entity.TotalRecipients = plan.TotalRecipients;

                var pending = 0;
                foreach (var delivery in plan.Deliveries)
                {
                    if (delivery.Skipped)
                    {
                        _dispatch.LogCustomSkipped(
                            delivery.Channel,
                            delivery.Receiver,
                            delivery.Locale,
                            delivery.ProfileId,
                            entity.Id,
                            delivery.SkipReason);
                        skipped++;
                    }
                    else if (delivery.Channel == CustomNotificationChannels.Email)
                    {
                        var ok = await _dispatch.SendCustomEmailAsync(
                            delivery.Receiver,
                            delivery.Subject,
                            delivery.Body,
                            delivery.Locale,
                            delivery.ProfileId,
                            entity.Id);
                        if (ok) sent++; else failed++;
                    }
                    else if (delivery.Channel == CustomNotificationChannels.Sms)
                    {
                        var ok = await _dispatch.SendCustomSmsAsync(
                            delivery.Receiver,
                            delivery.Body,
                            delivery.Locale,
                            delivery.ProfileId,
                            entity.Id);
                        if (ok) sent++; else failed++;
                    }
                    else if (delivery.Channel == CustomNotificationChannels.System)
                    {
                        _context.UserNotifications.Add(new UserNotification
                        {
                            UserId = delivery.UserId,
                            CustomNotificationId = entity.Id,
                            Locale = delivery.Locale,
                            Title = Truncate(delivery.Subject, CustomNotificationRules.TitleMaxLength),
                            Body = delivery.Body,
                            CreatedAtUtc = DateTime.UtcNow,
                        });
                        sent++;
                    }

                    if (++pending >= SaveEvery)
                    {
                        await _context.SaveChangesAsync();
                        pending = 0;
                    }
                }

                entity.Status = CustomNotificationRules.ComputeFinalStatus(sent, failed);
            }
            catch
            {
                entity.Status = CustomNotificationStatuses.Failed;
                throw;
            }
            finally
            {
                entity.SentCount = sent;
                entity.FailedCount = failed;
                entity.SkippedCount = skipped;
                entity.ProcessedAtUtc = DateTime.UtcNow;
                entity.UpdatedAtUtc = entity.ProcessedAtUtc.Value;
                await _context.SaveChangesAsync();
            }
        }

        private async Task<(
                List<CustomNotificationRules.RecipientUser> Users,
                List<CustomNotificationRules.RecipientOrganization> Orgs,
                List<CustomNotificationRules.RecipientUnregisteredJobSeeker> JobSeekers)>
            ResolveRecipientsAsync(IReadOnlyList<CustomNotificationTargetDto> targets)
        {
            var userIds = targets
                .Where(t => t.TargetType == CustomNotificationTargetTypes.User)
                .Select(t => t.UserId)
                .Distinct()
                .ToList();
            var groupIds = targets
                .Where(t => t.TargetType == CustomNotificationTargetTypes.UserGroup && t.UserGroupId.HasValue)
                .Select(t => t.UserGroupId.Value)
                .Distinct()
                .ToList();
            var orgIds = targets
                .Where(t => t.TargetType == CustomNotificationTargetTypes.Organization && t.OrganizationId.HasValue)
                .Select(t => t.OrganizationId.Value)
                .Distinct()
                .ToList();
            var allOrgsTarget = targets.FirstOrDefault(t => t.TargetType == CustomNotificationTargetTypes.AllOrganizations);
            var allOrgsFilter = allOrgsTarget?.RegistrationFilter;
            var allOrgsRegistered = allOrgsTarget != null && CustomNotificationRegistrationFilters.IncludesRegistered(allOrgsFilter);
            var allOrgsUnregistered = allOrgsTarget != null && CustomNotificationRegistrationFilters.IncludesNotRegistered(allOrgsFilter);

            var jobSeekersTarget = targets.FirstOrDefault(t => t.TargetType == CustomNotificationTargetTypes.JobSeekers);
            var jobSeekersRegistered = jobSeekersTarget != null
                && CustomNotificationRegistrationFilters.IncludesRegistered(jobSeekersTarget.RegistrationFilter);
            var jobSeekersUnregistered = jobSeekersTarget != null
                && CustomNotificationRegistrationFilters.IncludesNotRegistered(jobSeekersTarget.RegistrationFilter);
            const int jobSeekerProfileType = (int)Enums.ProfileTypes.JobSeeker;

            var users = await _context.Users.AsNoTracking()
                .Where(u => u.IsActive)
                .Where(u => userIds.Contains(u.Id)
                    || (u.UserGroupId != null && groupIds.Contains(u.UserGroupId.Value))
                    || (u.OrganizationId != null && (allOrgsRegistered || orgIds.Contains(u.OrganizationId.Value)))
                    || (jobSeekersRegistered
                        && u.Profile != null
                        && u.Profile.ProfileType == jobSeekerProfileType
                        && u.Profile.IsRegistered
                        && u.Profile.IsActive))
                .Select(u => new
                {
                    u.Id,
                    u.Email,
                    u.PhoneNumber,
                    u.ProfileId,
                    PreferredLanguage = u.Profile != null ? u.Profile.PreferredLanguage : null,
                    ProfilePhone = u.Profile != null ? u.Profile.PhoneNumber : null,
                })
                .ToListAsync();

            var recipients = users
                .Select(u => new CustomNotificationRules.RecipientUser
                {
                    UserId = u.Id,
                    ProfileId = u.ProfileId,
                    Email = u.Email,
                    Phone = string.IsNullOrWhiteSpace(u.ProfilePhone) ? u.PhoneNumber : u.ProfilePhone,
                    PreferredLanguage = u.PreferredLanguage,
                })
                .ToList();

            var orgs = new List<CustomNotificationRules.RecipientOrganization>();
            if (allOrgsUnregistered || orgIds.Count > 0)
            {
                orgs = await _context.Organizations.AsNoTracking()
                    .Where(o => o.IsActive)
                    .Where(o => allOrgsUnregistered || orgIds.Contains(o.Id))
                    .Where(o => !_context.Users.Any(u => u.OrganizationId == o.Id && u.IsActive))
                    .Select(o => new CustomNotificationRules.RecipientOrganization
                    {
                        OrganizationId = o.Id,
                        Email = o.Email,
                        Name = o.CompanyNameEnglish ?? o.CompanyNameArabic,
                    })
                    .ToListAsync();
            }

            var unregisteredJobSeekers = new List<CustomNotificationRules.RecipientUnregisteredJobSeeker>();
            if (jobSeekersUnregistered)
            {
                unregisteredJobSeekers = await _context.Profiles.AsNoTracking()
                    .Where(p => p.ProfileType == jobSeekerProfileType && !p.IsRegistered && p.IsActive)
                    .Select(p => new CustomNotificationRules.RecipientUnregisteredJobSeeker
                    {
                        ProfileId = p.Id,
                        Email = p.EmailAddress,
                        Phone = p.PhoneNumber,
                        Name = p.NameAr ?? p.Name,
                    })
                    .ToListAsync();
            }

            return (recipients, orgs, unregisteredJobSeekers);
        }

        private async Task<(List<CustomNotificationTargetDto> Targets, DateTime ScheduledAtUtc)> ValidateRequestAsync(
            UpsertCustomNotificationRequestDto body,
            DateTime utcNow)
        {
            var error = CustomNotificationRules.Validate(body, utcNow, out var scheduledAtUtc);
            if (error != null)
            {
                throw new SafeExecutionHelper.BadRequestException(error);
            }

            var targets = CustomNotificationRules.NormalizeTargets(body.Targets);
            var preview = await PreviewRecipientsAsync(targets);
            if (preview.Total == 0)
            {
                throw new SafeExecutionHelper.BadRequestException("The selected targets do not match any recipient.");
            }

            var reachError = CustomNotificationRules.ValidateReachability(preview, body.SendEmail, body.SendSms);
            if (reachError != null)
            {
                throw new SafeExecutionHelper.BadRequestException(reachError);
            }

            return (targets, scheduledAtUtc);
        }

        private static void Apply(
            CustomNotification entity,
            UpsertCustomNotificationRequestDto body,
            IEnumerable<CustomNotificationTargetDto> targets,
            DateTime scheduledAtUtc,
            DateTime utcNow)
        {
            entity.Title_en = body.Title_en.Trim();
            entity.Title_ar = body.Title_ar.Trim();
            entity.Body_en = body.Body_en.Trim();
            entity.Body_ar = body.Body_ar.Trim();
            entity.SendEmail = body.SendEmail;
            entity.SendSms = body.SendSms;
            entity.SendSystem = body.SendSystem;
            entity.ScheduledAtUtc = scheduledAtUtc;
            entity.UpdatedAtUtc = utcNow;
            foreach (var t in targets)
            {
                entity.Targets.Add(new CustomNotificationTarget
                {
                    TargetType = t.TargetType,
                    UserId = t.UserId,
                    UserGroupId = t.UserGroupId,
                    OrganizationId = t.OrganizationId,
                    Label = Truncate(t.Label, 256),
                    RegistrationFilter = t.RegistrationFilter,
                });
            }
        }

        private static void EnsureEditable(CustomNotification entity)
        {
            if (!CustomNotificationRules.CanEdit(entity.Status))
            {
                throw new SafeExecutionHelper.BadRequestException(
                    "Only scheduled notifications that have not been sent yet can be changed.");
            }
        }

        private static void EnsureCaller(string userId)
        {
            if (string.IsNullOrWhiteSpace(userId))
            {
                throw new SafeExecutionHelper.UnauthorizedException("A signed-in user is required.");
            }
        }

        private static (int Index, int Size) NormalizePaging(int? pageIndex, int? pageSize)
        {
            var size = pageSize.HasValue && pageSize.Value > 0 ? Math.Min(pageSize.Value, MaxPageSize) : DefaultPageSize;
            var index = pageIndex.HasValue && pageIndex.Value > 0 ? pageIndex.Value : 0;
            return (index, size);
        }

        private static T MapRow<T>(CustomNotification n, T dto) where T : CustomNotificationRowDto
        {
            var targets = n.Targets ?? new List<CustomNotificationTarget>();
            dto.Id = n.Id;
            dto.Title_en = n.Title_en;
            dto.Title_ar = n.Title_ar;
            dto.SendEmail = n.SendEmail;
            dto.SendSms = n.SendSms;
            dto.SendSystem = n.SendSystem;
            dto.ScheduledAtUtc = Iso(n.ScheduledAtUtc);
            dto.Status = n.Status;
            dto.TotalRecipients = n.TotalRecipients;
            dto.SentCount = n.SentCount;
            dto.FailedCount = n.FailedCount;
            dto.SkippedCount = n.SkippedCount;
            dto.UserTargetCount = targets.Count(t => t.TargetType == CustomNotificationTargetTypes.User);
            dto.GroupTargetCount = targets.Count(t => t.TargetType == CustomNotificationTargetTypes.UserGroup);
            dto.OrganizationTargetCount = targets.Count(t => t.TargetType == CustomNotificationTargetTypes.Organization);
            var allOrgsTarget = targets.FirstOrDefault(t => t.TargetType == CustomNotificationTargetTypes.AllOrganizations);
            var jobSeekersTarget = targets.FirstOrDefault(t => t.TargetType == CustomNotificationTargetTypes.JobSeekers);
            dto.AllOrganizations = allOrgsTarget != null;
            dto.AllOrganizationsFilter = allOrgsTarget != null
                ? CustomNotificationRegistrationFilters.Normalize(allOrgsTarget.RegistrationFilter)
                : null;
            dto.JobSeekersFilter = jobSeekersTarget != null
                ? CustomNotificationRegistrationFilters.Normalize(jobSeekersTarget.RegistrationFilter)
                : null;
            dto.CanEdit = CustomNotificationRules.CanEdit(n.Status);
            dto.CreatedAtUtc = Iso(n.CreatedAtUtc);
            dto.ProcessedAtUtc = n.ProcessedAtUtc.HasValue ? Iso(n.ProcessedAtUtc.Value) : null;
            return dto;
        }

        private static CustomNotificationDto MapDetail(CustomNotification n)
        {
            var dto = MapRow(n, new CustomNotificationDto());
            dto.Body_en = n.Body_en;
            dto.Body_ar = n.Body_ar;
            dto.Targets = (n.Targets ?? new List<CustomNotificationTarget>()).Select(ToDto).ToList();
            return dto;
        }

        private static CustomNotificationTargetDto ToDto(CustomNotificationTarget t)
        {
            return new CustomNotificationTargetDto
            {
                TargetType = t.TargetType,
                UserId = t.UserId,
                UserGroupId = t.UserGroupId,
                OrganizationId = t.OrganizationId,
                Label = t.Label,
                RegistrationFilter = t.RegistrationFilter,
            };
        }

        private static UserNotificationDto MapInbox(UserNotification n)
        {
            return new UserNotificationDto
            {
                Id = n.Id,
                Title = n.Title,
                Body = n.Body,
                Locale = n.Locale,
                IsRead = n.IsRead,
                CreatedAtUtc = Iso(n.CreatedAtUtc),
                ReadAtUtc = n.ReadAtUtc.HasValue ? Iso(n.ReadAtUtc.Value) : null,
            };
        }

        private static string Iso(DateTime value)
        {
            return DateTime.SpecifyKind(value, DateTimeKind.Utc).ToString("o");
        }

        private static string Truncate(string value, int max)
        {
            var v = (value ?? string.Empty).Trim();
            return v.Length <= max ? v : v.Substring(0, max);
        }
    }
}
