using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Bidding_Models.DatabaseModel.Credentials
{
    /// <summary>
    /// Admin-defined evaluation, exam, or course template.
    /// <see cref="Reference"/> is the stable API id (e.g. EV-1001, EX-2001, CR-3001).
    /// </summary>
    [Table("CredentialDefinitions")]
    public class CredentialDefinition
    {
        public int Id { get; set; }

        /// <summary>Stable business / API identifier — not the surrogate <see cref="Id"/>.</summary>
        [Required]
        [MaxLength(64)]
        public string Reference { get; set; }

        /// <summary><c>evaluation</c>, <c>exam</c>, or <c>course</c>.</summary>
        [Required]
        [MaxLength(16)]
        public string Kind { get; set; }

        [Required]
        [MaxLength(512)]
        public string TitleEn { get; set; }

        [Required]
        [MaxLength(512)]
        public string TitleAr { get; set; }

        public string DescriptionEn { get; set; }

        public string DescriptionAr { get; set; }

        public string InstructionsEn { get; set; }

        public string InstructionsAr { get; set; }

        public int Weight { get; set; }

        /// <summary>System config group <c>CRDT_STAT</c> value key (Draft, Active, Completed).</summary>
        [Required]
        [MaxLength(64)]
        public string StatusValueKey { get; set; }

        /// <summary>System config group <c>EXAM_TYPE</c> — only when <see cref="Kind"/> is <c>exam</c>.</summary>
        [MaxLength(64)]
        public string ExamTypeValueKey { get; set; }

        /// <summary>System config group <c>CRDT_PROV</c> — only when <see cref="Kind"/> is <c>course</c>.</summary>
        [MaxLength(64)]
        public string ProviderValueKey { get; set; }

        public DateTime CreatedAt { get; set; }

        public List<CredentialDefinitionSkill> Skills { get; set; } = new List<CredentialDefinitionSkill>();

        public List<CredentialDefinitionBasicSkill> BasicSkills { get; set; } =
            new List<CredentialDefinitionBasicSkill>();

        public List<CredentialDefinitionTechStack> TechStacks { get; set; } =
            new List<CredentialDefinitionTechStack>();

        public List<CredentialEnrollment> Enrollments { get; set; } = new List<CredentialEnrollment>();
    }
}
