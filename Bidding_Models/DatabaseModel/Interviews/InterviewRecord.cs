using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Bidding_Models.DatabaseModel.Interviews
{
    /// <summary>
    /// Persisted interview session (replaces in-memory <c>InterviewManagementRecordDto</c> over time).
    /// Skill links mirror credential definition junction tables.
    /// </summary>
    [Table("InterviewRecords")]
    public class InterviewRecord
    {
        public int Id { get; set; }

        /// <summary>Stable API id (e.g. IR-5001).</summary>
        [Required]
        [MaxLength(64)]
        public string Reference { get; set; }

        [Required]
        [MaxLength(512)]
        public string TitleEn { get; set; }

        [Required]
        [MaxLength(512)]
        public string TitleAr { get; set; }

        [MaxLength(64)]
        public string OpportunityReference { get; set; }

        [MaxLength(32)]
        public string InterviewType { get; set; }

        [MaxLength(1024)]
        public string LocationOrLink { get; set; }

        public string InstructionsEn { get; set; }

        public string InstructionsAr { get; set; }

        public string NotesEn { get; set; }

        public string NotesAr { get; set; }

        [Required]
        [MaxLength(32)]
        public string Status { get; set; }

        /// <summary>Used when snapshotting weight onto <see cref="Assessments.AssessmentResult"/>.</summary>
        public int Weight { get; set; }

        public DateTime CreatedAt { get; set; }

        public List<InterviewRecordSkill> Skills { get; set; } = new List<InterviewRecordSkill>();

        public List<InterviewRecordBasicSkill> BasicSkills { get; set; } = new List<InterviewRecordBasicSkill>();

        public List<InterviewRecordTechStack> TechStacks { get; set; } = new List<InterviewRecordTechStack>();

        public List<InterviewEnrollment> Enrollments { get; set; } = new List<InterviewEnrollment>();
    }
}
