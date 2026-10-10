using Microsoft.AspNetCore.Mvc;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Bidding_Models.DatabaseModel
{
	public class TRATraining
	{
		public int Id { get; set; }
		[Required(ErrorMessage = "isRequired.")]
		[Remote(action: "IsCivilNoExist", controller: "Training", AdditionalFields = "IsAlreadyRegisterd,TrainingId", ErrorMessage = "IsRegistrationNumberExist")]
		public string CivilNumber { get; set; }
		public int? ProfileId { get; set; }
		public Profile Profile { get; set; }
		public string NameEn { get; set; }
		public string NameAr { get; set; }
		public string Password { get; set; }
		public string Gender { get; set; }
		//[Remote(action: "IsEmailExist", controller: "Training", ErrorMessage = "IsPhoneNumberExist")]
		[Remote(action: "IsEmailExist", controller: "Training", AdditionalFields = "IsAlreadyRegisterd,TrainingId", ErrorMessage = "IsEmailExist")]
		public string EmailAddress { get; set; }
		//[Remote(action: "IsPhoneNumberExist", controller: "Training", ErrorMessage = "IsPhoneNumberExist")]
		[Remote(action: "IsPhoneNumberExist", controller: "Training", AdditionalFields = "IsAlreadyRegisterd,TrainingId", ErrorMessage = "IsPhoneNumberExist")]
		public string PhoneNumber { get; set; }
		public string University { get; set; }
		public string Specialization { get; set; }
		public string YearofGraduation { get; set; }
		public string GPA { get; set; }
		public string Score { get; set; }
		public string IsIELTS { get; set; }
		public string IsJobTraining { get; set; }
		public string NoofJobTraining { get; set; }
		public string languages { get; set; }

		//[Required(ErrorMessage = "isRequired.")]
		public string PreferLanguages { get; set; }
		public string programingskills { get; set; }
		public string Area { get; set; }
		public string databasesExperience { get; set; }
		public string programParticipate { get; set; }
		public int? TrainingMasterId { get; set; }
		public TrainingMaster TrainingMaster { get; set; }
		public string webServerEngine { get; set; }
		public string CVURL { get; set; }
		[Required(ErrorMessage = "isRequired.")]
		public string Confirmation { get; set; } = "";
		public DateTime SubmittedDate { get; set; }
		[NotMapped]
		public DocumentWrapper CVWrapper { get; set; }
		[NotMapped]
		public string SubmittedDateString { get; set; }
		public string Skills { get; set; }
		public string ITRating { get; set; }
		public string SecurityAssessmentRating { get; set; }

		///////// Extra fields for job application
		public string CoverLetterUrl { get; set; }
		public bool? NeedVisa { get; set; }
		public int? MonthlySalaryExpectation { get; set; }
		public string CurrentCompany { get; set; }
		public string NoticePeriod { get; set; }
		public string SocialLinkedin { get; set; }
		public string SocialTwitter { get; set; }
		public string SocialGithub { get; set; }
		public string Achievements { get; set; }
		public string Other { get; set; }
		public string SpecializationAr { get; set; }
		public string GeneralDiplomaScoreRange { get; set; }
		public string ArabicSubjectDiplomaScoreRange { get; set; }
		public int? ArabicSubjectDiplomaScore { get; set; }
		public string EnglishSubjectDiplomaScoreRange { get; set; }
		public int? EnglishSubjectDiplomaScore { get; set; }
		public string MathSubjectDiplomaScoreRange { get; set; }
		public int? MathSubjectDiplomaScore { get; set; }
		public bool? HasIelts { get; set; }
		public string EmploymentStatus { get; set; }
		public string DegreeUrl { get; set; }
		public string DegreeTranscriptUrl { get; set; }
	}
}
