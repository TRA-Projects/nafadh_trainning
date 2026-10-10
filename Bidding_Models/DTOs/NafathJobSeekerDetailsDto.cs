using System.Collections.Generic;
using Newtonsoft.Json;

namespace Bidding_Models.DTOs
{
    /// <summary>
    /// Response body from Nafath JobSeeker MOL integration:
    /// GET via <c>NafathJobSeeker:BaseUrl</c> template (<c>{civil_id}</c>, <c>{user}</c>, <c>{password}</c>).
    /// </summary>
    public class NafathJobSeekerDetailsDto
    {
        [JsonProperty("active_Date")]
        public string ActiveDate { get; set; }

        [JsonProperty("active_Desc_Arb")]
        public string ActiveDescArb { get; set; }

        [JsonProperty("active_Flag")]
        public string ActiveFlag { get; set; }

        [JsonProperty("act_Activity_Id")]
        public string ActActivityId { get; set; }

        [JsonProperty("category_Code")]
        public string CategoryCode { get; set; }

        [JsonProperty("category_Desc_Arb")]
        public string CategoryDescArb { get; set; }

        [JsonProperty("current_Emp_Date")]
        public string CurrentEmpDate { get; set; }

        [JsonProperty("date_Of_Birth")]
        public string DateOfBirth { get; set; }

        [JsonProperty("education_Code")]
        public string EducationCode { get; set; }

        [JsonProperty("education_Desc_Arb")]
        public string EducationDescArb { get; set; }

        [JsonProperty("education_Id")]
        public string EducationId { get; set; }

        [JsonProperty("email_Address")]
        public string EmailAddress { get; set; }

        [JsonProperty("expire_Date")]
        public string ExpireDate { get; set; }

        [JsonProperty("exp_Active_Date")]
        public string ExpActiveDate { get; set; }

        [JsonProperty("gsm_No")]
        public string GsmNo { get; set; }

        [JsonProperty("handicap_Type")]
        public string HandicapType { get; set; }

        [JsonProperty("health_Desc_Arb")]
        public string HealthDescArb { get; set; }

        [JsonProperty("health_Status")]
        public string HealthStatus { get; set; }

        [JsonProperty("issue_Date")]
        public string IssueDate { get; set; }

        [JsonProperty("job_Date")]
        public string JobDate { get; set; }

        [JsonProperty("job_Status")]
        public string JobStatus { get; set; }

        [JsonProperty("job_Status_Desc")]
        public string JobStatusDesc { get; set; }

        [JsonProperty("tranEducationDet")]
        public List<NafathEducationDetDto> TranEducationDet { get; set; }

        [JsonProperty("maritial_Desc_Arb")]
        public string MaritialDescArb { get; set; }

        [JsonProperty("maritial_Status")]
        public string MaritialStatus { get; set; }

        [JsonProperty("nationality_Code")]
        public string NationalityCode { get; set; }

        [JsonProperty("nationality_Desc_Arb")]
        public string NationalityDescArb { get; set; }

        [JsonProperty("nationality_Id")]
        public string NationalityId { get; set; }

        [JsonProperty("national_Id")]
        public string NationalId { get; set; }

        [JsonProperty("nmr_First_Name")]
        public string NmrFirstName { get; set; }

        [JsonProperty("nmr_First_Name_Eng")]
        public string NmrFirstNameEng { get; set; }

        [JsonProperty("nmr_Second_Name")]
        public string NmrSecondName { get; set; }

        [JsonProperty("nmr_Second_Name_Eng")]
        public string NmrSecondNameEng { get; set; }

        [JsonProperty("nmr_Third_Name")]
        public string NmrThirdName { get; set; }

        [JsonProperty("nmr_Third_Name_Eng")]
        public string NmrThirdNameEng { get; set; }

        [JsonProperty("nmr_Tribe_Name")]
        public string NmrTribeName { get; set; }

        [JsonProperty("nmr_Tribe_Name_Eng")]
        public string NmrTribeNameEng { get; set; }

        [JsonProperty("per_Region_Code")]
        public string PerRegionCode { get; set; }

        [JsonProperty("pER_REGION_Desc")]
        public string PerRegionDesc { get; set; }

        [JsonProperty("per_Village_Code")]
        public string PerVillageCode { get; set; }

        [JsonProperty("per_Village_Desc_Arb")]
        public string PerVillageDescArb { get; set; }

        [JsonProperty("per_Wilayat_Code")]
        public string PerWilayatCode { get; set; }

        [JsonProperty("per_Wilayat_Desc_Arb")]
        public string PerWilayatDescArb { get; set; }

        [JsonProperty("place_Of_Birth")]
        public string PlaceOfBirth { get; set; }

        [JsonProperty("registration_Date")]
        public string RegistrationDate { get; set; }

        [JsonProperty("registration_No")]
        public string RegistrationNo { get; set; }

        [JsonProperty("res_Region_Code")]
        public string ResRegionCode { get; set; }

        [JsonProperty("rES_REGION_Desc")]
        public string ResRegionDesc { get; set; }

        [JsonProperty("res_Village_Code")]
        public string ResVillageCode { get; set; }

        [JsonProperty("res_Village_Desc_Arb")]
        public string ResVillageDescArb { get; set; }

        [JsonProperty("res_Wilayat_Code")]
        public string ResWilayatCode { get; set; }

        [JsonProperty("res_Wilayat_Desc_Arb")]
        public string ResWilayatDescArb { get; set; }

        [JsonProperty("sex")]
        public string Sex { get; set; }

        [JsonProperty("sex_Desc_Arb")]
        public string SexDescArb { get; set; }

        [JsonProperty("sex_Desc_Eng")]
        public string SexDescEng { get; set; }

        [JsonProperty("social_Benefit_Number")]
        public string SocialBenefitNumber { get; set; }

        [JsonProperty("sponsor_No")]
        public string SponsorNo { get; set; }

        [JsonProperty("sub_Category_Code")]
        public string SubCategoryCode { get; set; }

        [JsonProperty("sub_Category_Desc_Arb")]
        public string SubCategoryDescArb { get; set; }

        [JsonProperty("telno1")]
        public string TelNo1 { get; set; }

        [JsonProperty("telno2")]
        public string TelNo2 { get; set; }

        [JsonProperty("listOfExp")]
        public List<NafathExperienceDto> ListOfExp { get; set; }

        [JsonProperty("listOfLang")]
        public List<NafathLanguageDto> ListOfLang { get; set; }

        [JsonProperty("listOfTrain")]
        public List<NafathTrainingDto> ListOfTrain { get; set; }
    }

    /// <summary>One row from Nafath <c>listOfExp</c>.</summary>
    public class NafathExperienceDto
    {
        [JsonProperty("crNo")]
        public string CrNo { get; set; }

        [JsonProperty("designation_Code")]
        public string DesignationCode { get; set; }

        [JsonProperty("designation_Desc")]
        public string DesignationDesc { get; set; }

        [JsonProperty("enddate")]
        public string EndDate { get; set; }

        [JsonProperty("expmonth")]
        public string ExpMonth { get; set; }

        [JsonProperty("expyear")]
        public string ExpYear { get; set; }

        [JsonProperty("occupation_Code")]
        public string OccupationCode { get; set; }

        [JsonProperty("occupdesc")]
        public string OccupDesc { get; set; }

        [JsonProperty("registration_No")]
        public string RegistrationNo { get; set; }

        [JsonProperty("sponsorname")]
        public string SponsorName { get; set; }

        [JsonProperty("startdate")]
        public string StartDate { get; set; }
    }

    /// <summary>One row from Nafath <c>listOfLang</c>.</summary>
    public class NafathLanguageDto
    {
        [JsonProperty("language_Desc_Arb")]
        public string LanguageDescArb { get; set; }

        [JsonProperty("language_Id")]
        public string LanguageId { get; set; }

        [JsonProperty("lang_Read")]
        public string LangRead { get; set; }

        [JsonProperty("lang_Read_Desc_Arb")]
        public string LangReadDescArb { get; set; }

        [JsonProperty("lang_Speak")]
        public string LangSpeak { get; set; }

        [JsonProperty("lang_Speak_Desc_Arb")]
        public string LangSpeakDescArb { get; set; }

        [JsonProperty("lang_Write")]
        public string LangWrite { get; set; }

        [JsonProperty("lang_Write_Desc_Arb")]
        public string LangWriteDescArb { get; set; }
    }

    /// <summary>One row from Nafath <c>listOfTrain</c>.</summary>
    public class NafathTrainingDto
    {
        [JsonProperty("TRAINING_COUNTRY_DESC_ARB")]
        public string TrainingCountryDescArb { get; set; }

        [JsonProperty("TRAINING_COURSE_DESC_ARB")]
        public string TrainingCourseDescArb { get; set; }

        [JsonProperty("TRAINING_FROM")]
        public string TrainingFrom { get; set; }

        [JsonProperty("TRAINING_TO")]
        public string TrainingTo { get; set; }
    }

    /// <summary>One row from Nafath <c>tranEducationDet</c>.</summary>
    public class NafathEducationDetDto
    {
        [JsonProperty("EDUCATION_CODE")]
        public string EducationCode { get; set; }

        [JsonProperty("EDUCATION_DESC_ARB")]
        public string EducationDescArb { get; set; }

        [JsonProperty("EDUCATION_ID")]
        public string EducationId { get; set; }

        [JsonProperty("GPA_SCH")]
        public string GpaSch { get; set; }

        [JsonProperty("GRADUATION_COUNTRY_CODE")]
        public string GraduationCountryCode { get; set; }

        [JsonProperty("GRADUATION_COUNTRY_DESC_ARB")]
        public string GraduationCountryDescArb { get; set; }

        [JsonProperty("GRADUATION_DATE")]
        public string GraduationDate { get; set; }

        [JsonProperty("start_Dt")]
        public string StartDt { get; set; }

        [JsonProperty("NIMR_MOHE_MAJOR_QLFN_DESC")]
        public string NimrMoheMajorQlfnDesc { get; set; }

        [JsonProperty("NIMR_MOHE_MINOR_QLFN_DESC")]
        public string NimrMoheMinorQlfnDesc { get; set; }

        [JsonProperty("NIMR_MOHE_MAJOR_QLFN_ID")]
        public string NimrMoheMajorQlfnId { get; set; }

        [JsonProperty("nimr_Mohe_Minor_Qlfn_Id")]
        public string NimrMoheMinorQlfnId { get; set; }

        [JsonProperty("nimr_Mohe_University_Desc")]
        public string NimrMoheUniversityDesc { get; set; }

        [JsonProperty("nimr_Mohe_University_Id")]
        public string NimrMoheUniversityId { get; set; }

        [JsonProperty("NIMR_MOHE_GRADS_DESC")]
        public string NimrMoheGradsDesc { get; set; }

        [JsonProperty("nmr_Grades_Desc_Arb")]
        public string NmrGradesDescArb { get; set; }

        [JsonProperty("orderid")]
        public string OrderId { get; set; }
    }

    /// <summary>Outcome of a Nafath JobSeeker details fetch (live or mock).</summary>
    public class NafathJobSeekerFetchResult
    {
        /// <summary>False when the remote call failed (network/HTTP). Mock always succeeds.</summary>
        public bool Success { get; set; }

        public string ErrorMessage { get; set; }

        public NafathJobSeekerDetailsDto Data { get; set; }

        /// <summary>True when the payload has usable jobseeker fields (not the all-null empty shape).</summary>
        public bool HasData { get; set; }

        /// <summary>True when the record passes <c>NAFATH_VALIDITY_CHECK</c> (flag and/or expiry date).</summary>
        public bool IsActiveJobSeeker { get; set; }

        /// <summary>User-facing detail when <see cref="IsActiveJobSeeker"/> is false (flag and expiry found).</summary>
        public string InactiveDetail { get; set; }

        /// <summary>Which mock fixture was used, when UseMock=true.</summary>
        public string MockFixture { get; set; }
    }
}
