using System;
using System.Collections.Generic;
using System.Linq;
using Bidding_API.Helpers;
using Bidding_Core.Helpers;
using Bidding_Core.Services.Interfaces;
using Bidding_Models.Jobseekers;

namespace Bidding_Core.Services.Implementations
{
    /// <summary>
    /// In-memory mock for the UI showcase pages. Registered as a singleton, so data lives until the API restarts.
    /// Replace with a CHDBContext-backed service when building a real feature.
    /// </summary>
    public class ShowcaseMockService : IShowcaseService
    {
        private const int DefaultPageSize = 5;
        private const int MaxPageSize = 100;

        private readonly object _gate = new object();
        private readonly List<ShowcaseItemDto> _items;
        private long _nextId;

        public ShowcaseMockService()
        {
            _items = Seed();
            _nextId = _items.Max(i => i.Id) + 1;
        }

        public PagedListResponseDto<ShowcaseItemDto> GetPage(int? pageIndex, int? pageSize, string q, string category, string status)
        {
            lock (_gate)
            {
                IEnumerable<ShowcaseItemDto> query = _items;

                var search = (q ?? string.Empty).Trim();
                if (search.Length > 0)
                {
                    query = query.Where(i =>
                        Contains(i.Title_en, search) || Contains(i.Title_ar, search) || Contains(i.ContactEmail, search));
                }

                var categoryFilter = (category ?? string.Empty).Trim();
                if (categoryFilter.Length > 0) query = query.Where(i => i.Category == categoryFilter);

                var statusFilter = (status ?? string.Empty).Trim();
                if (statusFilter.Length > 0) query = query.Where(i => i.Status == statusFilter);

                var filtered = query.OrderByDescending(i => i.CreatedAtUtc).ThenByDescending(i => i.Id).ToList();

                var size = pageSize.GetValueOrDefault(DefaultPageSize);
                if (size <= 0) size = DefaultPageSize;
                size = Math.Min(size, MaxPageSize);
                var maxIndex = Math.Max(0, (int)Math.Ceiling(filtered.Count / (double)size) - 1);
                var index = Math.Min(Math.Max(0, pageIndex.GetValueOrDefault()), maxIndex);

                return new PagedListResponseDto<ShowcaseItemDto>
                {
                    rows = filtered.Skip(index * size).Take(size).Select(Copy).ToList(),
                    total = filtered.Count,
                    pageIndex = index,
                    pageSize = size,
                };
            }
        }

        public ShowcaseItemDto Get(long id)
        {
            lock (_gate)
            {
                return Copy(Find(id));
            }
        }

        public ShowcaseItemDto Create(UpsertShowcaseItemRequestDto body)
        {
            ThrowIfInvalid(body);
            lock (_gate)
            {
                var item = new ShowcaseItemDto { Id = _nextId++, CreatedAtUtc = DateTime.UtcNow };
                Apply(item, body);
                _items.Add(item);
                return Copy(item);
            }
        }

        public ShowcaseItemDto Update(long id, UpsertShowcaseItemRequestDto body)
        {
            ThrowIfInvalid(body);
            lock (_gate)
            {
                var item = Find(id);
                if (!ShowcaseRules.CanEdit(item.Status))
                {
                    throw new SafeExecutionHelper.BadRequestException("Approved records cannot be edited.");
                }
                Apply(item, body);
                return Copy(item);
            }
        }

        public void Delete(long id)
        {
            lock (_gate)
            {
                var item = Find(id);
                if (!ShowcaseRules.CanEdit(item.Status))
                {
                    throw new SafeExecutionHelper.BadRequestException("Approved records cannot be deleted.");
                }
                _items.Remove(item);
            }
        }

        private ShowcaseItemDto Find(long id) =>
            _items.FirstOrDefault(i => i.Id == id)
            ?? throw new SafeExecutionHelper.NotFoundException("Record not found.");

        private static void ThrowIfInvalid(UpsertShowcaseItemRequestDto body)
        {
            var error = ShowcaseRules.Validate(body);
            if (error != null) throw new SafeExecutionHelper.BadRequestException(error);
        }

        private static void Apply(ShowcaseItemDto item, UpsertShowcaseItemRequestDto body)
        {
            item.Title_en = body.Title_en.Trim();
            item.Title_ar = body.Title_ar.Trim();
            item.Category = body.Category.Trim();
            item.Status = body.Status.Trim();
            item.ContactEmail = body.ContactEmail.Trim();
            item.PhoneNumber = (body.PhoneNumber ?? string.Empty).Trim();
            item.Amount = body.Amount.GetValueOrDefault();
            item.StartDate = body.StartDate.GetValueOrDefault().Date;
            item.EndDate = body.EndDate.GetValueOrDefault().Date;
            item.IsUrgent = body.IsUrgent;
            item.Description = (body.Description ?? string.Empty).Trim();
        }

        private static ShowcaseItemDto Copy(ShowcaseItemDto s) => new ShowcaseItemDto
        {
            Id = s.Id,
            Title_en = s.Title_en,
            Title_ar = s.Title_ar,
            Category = s.Category,
            Status = s.Status,
            ContactEmail = s.ContactEmail,
            PhoneNumber = s.PhoneNumber,
            Amount = s.Amount,
            StartDate = s.StartDate,
            EndDate = s.EndDate,
            IsUrgent = s.IsUrgent,
            Description = s.Description,
            CreatedAtUtc = s.CreatedAtUtc,
            CanEdit = ShowcaseRules.CanEdit(s.Status),
        };

        private static bool Contains(string value, string search) =>
            (value ?? string.Empty).IndexOf(search, StringComparison.OrdinalIgnoreCase) >= 0;

        private static List<ShowcaseItemDto> Seed()
        {
            var titles = new[]
            {
                ("Leadership workshop", "ورشة القيادة"),
                ("Laptop replacement", "استبدال حاسوب محمول"),
                ("Conference travel", "سفر لحضور مؤتمر"),
                ("Safety training", "تدريب السلامة"),
                ("Office printer", "طابعة المكتب"),
                ("Field visit", "زيارة ميدانية"),
                ("Excel advanced course", "دورة إكسل متقدمة"),
                ("Monitor upgrade", "ترقية الشاشة"),
                ("Regional meeting trip", "رحلة اجتماع إقليمي"),
                ("First aid certification", "شهادة الإسعافات الأولية"),
                ("Network switch", "محول الشبكة"),
                ("Customer site visit", "زيارة موقع العميل"),
            };

            var today = DateTime.UtcNow.Date;
            var list = new List<ShowcaseItemDto>();
            for (var i = 0; i < 23; i++)
            {
                var (en, ar) = titles[i % titles.Length];
                var round = i / titles.Length;
                list.Add(new ShowcaseItemDto
                {
                    Id = i + 1,
                    Title_en = round == 0 ? en : $"{en} {round + 1}",
                    Title_ar = round == 0 ? ar : $"{ar} {round + 1}",
                    Category = ShowcaseRules.Categories[i % ShowcaseRules.Categories.Length],
                    Status = ShowcaseRules.Statuses[i % ShowcaseRules.Statuses.Length],
                    ContactEmail = $"sample{i + 1}@example.com",
                    PhoneNumber = $"+9689{(1000000 + i * 7919) % 10000000:0000000}",
                    Amount = 50 + (i * 37 % 900),
                    StartDate = today.AddDays(i),
                    EndDate = today.AddDays(i + 3 + i % 5),
                    IsUrgent = i % 4 == 0,
                    Description = "Sample record for the UI showcase.",
                    CreatedAtUtc = DateTime.UtcNow.AddHours(-i * 5),
                });
            }
            return list;
        }
    }
}
