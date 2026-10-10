using System.Collections.Generic;

namespace Bidding_Models.Jobseekers
{
    public class JobSeekerServiceOfferingOptionDto
    {
        public int Id { get; set; }
        public string Name_en { get; set; }
        public string Name_ar { get; set; }
    }

    public class JobSeekerCvAttachmentDto
    {
        public int attachmentId { get; set; }
        public string fileName { get; set; }
        public string fileType { get; set; }
        public int fileSize { get; set; }
        public string dataUrl { get; set; }
    }

    public class JobSeekerProfileGeneralInfoPutDto
    {
        public int ProfileId { get; set; }
        public string About { get; set; }
        public string Website { get; set; }
        public string Address { get; set; }
        public string MissionVision { get; set; }
        public string CompanyName { get; set; }
        public string EmailAddress { get; set; }
        /// <summary>Preferred notification language: <c>en</c> or <c>ar</c>.</summary>
        public string PreferredLanguage { get; set; }
        public string CrNumber { get; set; }
        public object ProfileImage { get; set; }
        public List<ServiceOfferingIdDto> ServicesOffered { get; set; }
        public JobSeekerCvAttachmentDto CvAttachment { get; set; }

        /// <summary>
        /// Current/User-owned location (UAT-07 Phase 5) — distinct from Official/MOL location, which
        /// is read-only and lives on ProfileMolSummary/Profile.Mol_Willaya, never here. <c>null</c>
        /// means "not part of this request" (covers both an old client that predates this field and an
        /// explicit JSON <c>null</c>) — NOT a clear-Current-Location command; the save path must leave
        /// Profile.GovernorateId/WilayatId/CityId/AddressName untouched in that case. When present, it
        /// must describe a COMPLETE Current Location (see JobseekerProfileGeneralService.ApplyCurrentLocation).
        /// </summary>
        public CurrentLocationDto CurrentLocation { get; set; }
    }

    /// <summary>
    /// Current/User-owned location for a Jobseeker (UAT-07 Phase 5) — Governorate/Wilayat are always
    /// required together; Village/Area is exactly one of a catalog City (<see cref="CityId"/>) or free
    /// text, never both. See JobseekerProfileGeneralService.ApplyCurrentLocation for the validation/save
    /// rules.
    /// </summary>
    /// <remarks>
    /// UAT-07 Phase 6D.2 — bilingual Other/Not-listed free text (additive, backend contract stays
    /// backward compatible):
    /// <list type="bullet">
    /// <item><see cref="AddressNameAr"/>/<see cref="AddressNameEn"/> are the authoritative pair for NEW
    /// Other-mode saves — both required together when Other mode is used.</item>
    /// <item><see cref="AddressName"/> is retained only as legacy read/mirror compatibility for older
    /// records and for external consumers (AuthService, ProfileV2Service, ProjectV2Service) that still
    /// read the single plain-text field — it is never the authoritative bilingual value for new saves.</item>
    /// </list>
    /// </remarks>
    public class CurrentLocationDto
    {
        public int? GovernorateId { get; set; }
        public int? WilayatId { get; set; }
        public int? CityId { get; set; }

        /// <summary>Legacy compatibility only — see remarks on <see cref="CurrentLocationDto"/>.</summary>
        public string AddressName { get; set; }

        /// <summary>New bilingual Other/Not-listed free text — Arabic. Required together with <see cref="AddressNameEn"/> when Other mode is used.</summary>
        public string AddressNameAr { get; set; }

        /// <summary>New bilingual Other/Not-listed free text — English. Required together with <see cref="AddressNameAr"/> when Other mode is used.</summary>
        public string AddressNameEn { get; set; }

        /// <summary>
        /// READ-ONLY signal (UAT-07 Final Consolidation) — ignored on write. <c>true</c> when the
        /// values above represent a real, previously-saved Jobseeker Current Location override.
        /// <c>false</c> means the values are Official/MOL location resolved as a DISPLAY-ONLY
        /// fallback (best-effort Governorate/Wilayat only — Village/Area can never be resolved from
        /// MOL, so <see cref="CityId"/> stays null in that case) — nothing has actually been saved,
        /// and the client must not persist these values unless the user explicitly changes something.
        /// See JobseekerProfileGeneralService.BuildCurrentLocationForReadAsync.
        /// </summary>
        public bool HasOverride { get; set; }
    }

    public class ServiceOfferingIdDto
    {
        public int Id { get; set; }
    }
}
