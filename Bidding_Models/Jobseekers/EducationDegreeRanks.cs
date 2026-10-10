using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Newtonsoft.Json;

namespace Bidding_Models.Jobseekers
{
    /// <summary>
    /// One degree rank for matching Nafath <c>EDUCATION_DESC_ARB</c> (and optional code).
    /// Rank 1 is the highest.
    /// </summary>
    public sealed class EducationDegreeRankLevel
    {
        [JsonProperty("rank")]
        public int Rank { get; set; }

        [JsonProperty("labels")]
        public List<string> Labels { get; set; } = new List<string>();
    }

    /// <summary>Parse, serialize, and match education degree rank JSON stored in system variables.</summary>
    public static class EducationDegreeRanks
    {
        public static readonly string DefaultJson = Serialize(CreateDefaultLevels());

        public static IReadOnlyList<EducationDegreeRankLevel> Parse(string json)
        {
            return TryParse(json, out var levels, out _) ? levels : Array.Empty<EducationDegreeRankLevel>();
        }

        public static bool TryParse(string json, out IReadOnlyList<EducationDegreeRankLevel> levels, out string error)
        {
            levels = Array.Empty<EducationDegreeRankLevel>();
            error = null;
            var raw = (json ?? string.Empty).Trim();
            if (string.IsNullOrEmpty(raw))
            {
                return true;
            }

            List<EducationDegreeRankLevel> parsed;
            try
            {
                parsed = JsonConvert.DeserializeObject<List<EducationDegreeRankLevel>>(raw);
            }
            catch (JsonException)
            {
                error = "Invalid education degree ranks JSON.";
                return false;
            }

            if (parsed == null)
            {
                error = "Invalid education degree ranks JSON.";
                return false;
            }

            var cleaned = new List<EducationDegreeRankLevel>();
            foreach (var row in parsed.OrderBy(r => r?.Rank ?? int.MaxValue))
            {
                var labels = DistinctLabels(row?.Labels);
                if (labels.Count == 0)
                {
                    continue;
                }

                cleaned.Add(new EducationDegreeRankLevel
                {
                    Rank = cleaned.Count + 1,
                    Labels = labels,
                });
            }

            levels = cleaned;
            return true;
        }

        public static bool TryCanonicalize(string json, out string canonical, out string error)
        {
            canonical = null;
            if (!TryParse(json, out var levels, out error))
            {
                return false;
            }

            if (levels.Count == 0)
            {
                error = "At least one rank with a label is required.";
                return false;
            }

            canonical = Serialize(levels);
            return true;
        }

        public static string Serialize(IReadOnlyList<EducationDegreeRankLevel> levels)
        {
            var rows = (levels ?? Array.Empty<EducationDegreeRankLevel>())
                .Select((row, i) => new EducationDegreeRankLevel
                {
                    Rank = i + 1,
                    Labels = DistinctLabels(row?.Labels),
                })
                .Where(row => row.Labels.Count > 0)
                .ToList();
            return JsonConvert.SerializeObject(rows, Formatting.None);
        }

        /// <summary>
        /// Rank for a Nafath education label/code. Exact alias wins; otherwise the longest alias contained in the text.
        /// </summary>
        public static int? RankForLabel(IReadOnlyList<EducationDegreeRankLevel> levels, params string[] texts)
        {
            if (levels == null || levels.Count == 0)
            {
                return null;
            }

            string needle = null;
            if (texts != null)
            {
                foreach (var text in texts)
                {
                    needle = Normalize(text);
                    if (needle != null)
                    {
                        break;
                    }
                }
            }

            if (needle == null)
            {
                return null;
            }

            int? exactRank = null;
            foreach (var level in levels)
            {
                if (level?.Labels == null)
                {
                    continue;
                }

                foreach (var label in level.Labels)
                {
                    var alias = Normalize(label);
                    if (alias == null || alias != needle)
                    {
                        continue;
                    }

                    if (exactRank == null || level.Rank < exactRank.Value)
                    {
                        exactRank = level.Rank;
                    }
                }
            }

            if (exactRank != null)
            {
                return exactRank;
            }

            int bestLen = -1;
            int? containsRank = null;
            foreach (var level in levels)
            {
                if (level?.Labels == null)
                {
                    continue;
                }

                foreach (var label in level.Labels)
                {
                    var alias = Normalize(label);
                    if (alias == null || alias.Length < 2)
                    {
                        continue;
                    }

                    if (needle.IndexOf(alias, StringComparison.Ordinal) < 0)
                    {
                        continue;
                    }

                    if (alias.Length > bestLen
                        || (alias.Length == bestLen && containsRank != null && level.Rank < containsRank.Value))
                    {
                        bestLen = alias.Length;
                        containsRank = level.Rank;
                    }
                }
            }

            return containsRank;
        }

        public static int CountRanks(string json)
        {
            return Parse(json).Count;
        }

        private static List<string> DistinctLabels(IEnumerable<string> labels)
        {
            var result = new List<string>();
            var seen = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            if (labels == null)
            {
                return result;
            }

            foreach (var label in labels)
            {
                var trimmed = (label ?? string.Empty).Trim();
                if (trimmed.Length == 0 || !seen.Add(trimmed))
                {
                    continue;
                }

                result.Add(trimmed);
            }

            return result;
        }

        private static string Normalize(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                return null;
            }

            var collapsed = string.Join(
                " ",
                value.Trim().Split((char[])null, StringSplitOptions.RemoveEmptyEntries));
            return collapsed.ToLower(CultureInfo.InvariantCulture);
        }

        private static IReadOnlyList<EducationDegreeRankLevel> CreateDefaultLevels()
        {
            return new[]
            {
                Level(1, "دكتوراه", "PhD", "Doctorate (PhD)"),
                Level(2, "ماجستير", "Master's degree", "Master"),
                Level(3, "بكالوريوس", "Bachelor's degree", "Bachelor"),
                Level(4, "الدبلوم العالي", "Higher Diploma"),
                Level(5, "دبلوم تعليم العام", "دبلوم", "Diploma"),
                Level(6, "الثانوية العامة", "High School"),
            };
        }

        private static EducationDegreeRankLevel Level(int rank, params string[] labels)
        {
            return new EducationDegreeRankLevel
            {
                Rank = rank,
                Labels = labels.ToList(),
            };
        }
    }
}
