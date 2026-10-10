namespace Bidding_Models.DTOs
{
    public class FreelancerProfileFromMolDto
    {
        public int ProfileId { get; set; }
        // Personal Details
        public string FullName { get; set; }
        public string Gender { get; set; }
        public string Age { get; set; }
        public string DateOfBirth { get; set; }
        public string Governorate { get; set; }
        public string Wilayat { get; set; }
        public string Address { get; set; }
        public string MaritalStatus { get; set; }
        public string HealthStatus { get; set; }
        public string AttachmentFileName { get; set; }
        // Searcher Status
        public string SearcherStatus { get; set; }
        public string JobSearchDuration { get; set; }
        public string DateOfFirstRegistrationAsJobSeeker { get; set; }
        public string DateOfLastRegistrationAsJobSeeker { get; set; }

        // Education
        public string UniversityMajor { get; set; }
        public string EducationalInstitutions { get; set; }
        public string Grade { get; set; }
        public string EducationLevel { get; set; }
        public string CountryOfGraduation { get; set; }
        public string YearOfGraduation { get; set; }

        // General Diploma Details
        public string CompleteGrade { get; set; }
        public string Mathematics { get; set; }
        public string Physics { get; set; }
        public string Chemistry { get; set; }
        public string Biology { get; set; }
        public string Arabic { get; set; }
        public string English { get; set; }

        // Job Nomination
        public string InstitutionNominated { get; set; }
        public string Job { get; set; }
        public string DateOfNomination { get; set; }
        public string Note { get; set; }
        public string NumberOfNominations { get; set; }

        // Work Experience
        public string WorkStatus { get; set; }
        public string WorkExperience { get; set; }
        public string NameOfOrganizationWorked { get; set; }
        public string ContractStartDate { get; set; }
        public string ContractExpiryDate { get; set; }
        public string JobSecurityAllowance { get; set; }
    }

}