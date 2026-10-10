using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace Bidding_Models.DatabaseModel.JobOpportunities
{
    [Table("JobOpportunityRequirements")]
    public class JobOpportunityRequirement
    {
        public int Id { get; set; }

        public int JobOpportunityId { get; set; }

        public JobOpportunity JobOpportunity { get; set; }

        public int YearsOfExperience { get; set; }

        public int YearsOfExperienceMargin { get; set; }

        public string EducationLevelValueKey { get; set; }

        public string TargetApplicantValueKey { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime UpdatedAt { get; set; }

        public List<JobOpportunityRequirementSkill> Skills { get; set; } =
            new List<JobOpportunityRequirementSkill>();

        public List<JobOpportunityRequirementTechStack> TechStacks { get; set; } =
            new List<JobOpportunityRequirementTechStack>();

        public List<JobOpportunityRequirementFieldOfStudy> FieldOfStudies { get; set; } =
            new List<JobOpportunityRequirementFieldOfStudy>();

        public List<JobOpportunityRequirementLanguage> Languages { get; set; } =
            new List<JobOpportunityRequirementLanguage>();

        public List<JobOpportunityRequirementCertification> Certifications { get; set; } =
            new List<JobOpportunityRequirementCertification>();
    }
}
