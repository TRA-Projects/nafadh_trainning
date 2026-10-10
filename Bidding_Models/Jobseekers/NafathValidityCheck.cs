using System;

namespace Bidding_Models.Jobseekers
{
    /// <summary>How registration treats Nafath active-status fields.</summary>
    public enum NafathValidityCheckMode
    {
        None = 0,
        ExpiryDate = 1,
        ActiveFlag = 2,
        Both = 3,
    }

    /// <summary>Parse and store <see cref="SystemVariableCodes.NafathValidityCheck"/> values.</summary>
    public static class NafathValidityCheck
    {
        public const string None = "NONE";
        public const string ExpiryDate = "EXPIRY_DATE";
        public const string ActiveFlag = "ACTIVE_FLAG";
        public const string Both = "BOTH";
        public const string DefaultCode = Both;

        public static readonly string[] Codes = { None, ExpiryDate, ActiveFlag, Both };

        public static NafathValidityCheckMode Parse(string raw)
        {
            return TryCanonicalize(raw, out var code, out _)
                ? FromCode(code)
                : NafathValidityCheckMode.Both;
        }

        public static bool TryCanonicalize(string raw, out string canonical, out string error)
        {
            canonical = null;
            error = null;
            var value = (raw ?? string.Empty).Trim();
            if (string.IsNullOrEmpty(value))
            {
                canonical = DefaultCode;
                return true;
            }

            switch (value.ToUpperInvariant())
            {
                case None:
                case "OFF":
                case "SKIP":
                    canonical = None;
                    return true;
                case ExpiryDate:
                case "DATE":
                case "EXPIRY":
                    canonical = ExpiryDate;
                    return true;
                case ActiveFlag:
                case "FLAG":
                    canonical = ActiveFlag;
                    return true;
                case Both:
                case "ALL":
                    canonical = Both;
                    return true;
                default:
                    error = "Invalid Nafath validity check. Use NONE, EXPIRY_DATE, ACTIVE_FLAG, or BOTH.";
                    return false;
            }
        }

        public static NafathValidityCheckMode FromCode(string code)
        {
            switch ((code ?? string.Empty).Trim().ToUpperInvariant())
            {
                case None:
                    return NafathValidityCheckMode.None;
                case ExpiryDate:
                    return NafathValidityCheckMode.ExpiryDate;
                case ActiveFlag:
                    return NafathValidityCheckMode.ActiveFlag;
                default:
                    return NafathValidityCheckMode.Both;
            }
        }

        public static string ToCode(NafathValidityCheckMode mode)
        {
            switch (mode)
            {
                case NafathValidityCheckMode.None:
                    return None;
                case NafathValidityCheckMode.ExpiryDate:
                    return ExpiryDate;
                case NafathValidityCheckMode.ActiveFlag:
                    return ActiveFlag;
                default:
                    return Both;
            }
        }
    }
}
