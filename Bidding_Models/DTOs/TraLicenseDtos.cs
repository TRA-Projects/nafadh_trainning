using System.Collections.Generic;
using Newtonsoft.Json;

namespace Bidding_Models.DTOs
{
    public class TraLicensedCompaniesApiResponse
    {
        [JsonProperty("data")]
        public TraLicensedCompaniesData Data { get; set; }

        [JsonProperty("status")]
        public int? Status { get; set; }

        [JsonProperty("message")]
        public string Message { get; set; }
    }

    public class TraLicensedCompaniesData
    {
        [JsonProperty("LisensedCompanys")]
        public List<TraLicensedCompanyItem> LisensedCompanys { get; set; }
    }

    public class TraLicensedCompanyItem
    {
        [JsonProperty("crNumber")]
        public string CrNumber { get; set; }
    }

    public class TraActiveLicenseItem
    {
        [JsonProperty("crNo")]
        public string CrNo { get; set; }

        [JsonProperty("CompanyNameAr")]
        public string CompanyNameAr { get; set; }

        [JsonProperty("CompanyNameEn")]
        public string CompanyNameEn { get; set; }

        [JsonProperty("Service")]
        public string Service { get; set; }

        [JsonProperty("class_DC")]
        public string ClassDc { get; set; }
    }

    public class TraLicenseGridResult
    {
        public bool Success { get; set; }
        public string ErrorCode { get; set; }
        public string ErrorMessage { get; set; }
        public List<TraLicenseGridRowDto> Rows { get; set; } = new List<TraLicenseGridRowDto>();
    }

    public class TraLicenseGridRowDto
    {
        public string CrNumber { get; set; }
        public string CompanyNameEn { get; set; }
        public string CompanyNameAr { get; set; }
        public string Service { get; set; }
        public string ClassDc { get; set; }
        /// <summary>True when CR is present in TelecomMobileAPI LisensedCompanys.</summary>
        public bool Licensed { get; set; }
    }
}
