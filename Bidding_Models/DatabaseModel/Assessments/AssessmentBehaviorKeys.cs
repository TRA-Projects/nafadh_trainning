using System;
using System.Collections.Generic;

namespace Bidding_Models.DatabaseModel.Assessments
{
    /// <summary>Normalizes system-config value keys stored on an assessment or interview behavior selection.</summary>
    public static class AssessmentBehaviorKeys
    {
        public const int MaxLength = 64;

        public static List<string> Normalize(IEnumerable<string> keys)
        {
            var result = new List<string>();
            if (keys == null)
            {
                return result;
            }

            var seen = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            foreach (var raw in keys)
            {
                var key = raw?.Trim();
                if (string.IsNullOrEmpty(key) || key.Length > MaxLength)
                {
                    continue;
                }

                if (!seen.Add(key))
                {
                    continue;
                }

                result.Add(key);
            }

            return result;
        }
    }
}
