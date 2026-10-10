using System;
using System.Collections.Generic;
using System.Linq;
using Bidding_Models.Jobseekers;

namespace Bidding_Core.Helpers
{
    internal static class AdminSystemPaging
    {
        internal static bool ShouldApplyPaging(int? pageIndex, int? pageSize)
        {
            return pageSize.HasValue
                && pageSize.Value > 0
                && pageIndex.HasValue
                && pageIndex.Value >= 0;
        }

        internal static AdminSystemPagedListDto<T> SlicePage<T>(IReadOnlyList<T> rows, int? pageIndex, int? pageSize)
        {
            var total = rows.Count;
            if (!ShouldApplyPaging(pageIndex, pageSize))
            {
                return new AdminSystemPagedListDto<T>
                {
                    rows = rows.ToList(),
                    total = total,
                    pageIndex = 0,
                    pageSize = total
                };
            }

            var allowed = new[] { 5, 10, 15 };
            var size = allowed.Contains(pageSize.Value) ? pageSize.Value : 5;
            var totalPages = Math.Max(1, (int)Math.Ceiling(total / (double)size));
            var safeIndex = Math.Min(Math.Max(0, pageIndex.Value), totalPages - 1);
            var start = safeIndex * size;

            return new AdminSystemPagedListDto<T>
            {
                rows = rows.Skip(start).Take(size).ToList(),
                total = total,
                pageIndex = safeIndex,
                pageSize = size
            };
        }

        internal static string NormalizeSearch(string value) =>
            (value ?? string.Empty).Trim().ToLowerInvariant();
    }
}
