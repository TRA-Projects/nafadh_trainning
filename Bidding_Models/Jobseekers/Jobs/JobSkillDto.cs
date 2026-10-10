namespace Bidding_Models.Jobseekers.Jobs
{
    public class JobSkillDto
    {
        public int Id { get; set; }
        public string NameEn { get; set; }
        public string NameAr { get; set; }
        /// <summary>Legacy alias for <see cref="NameEn"/>.</summary>
        public string Name { get; set; }
    }
}
