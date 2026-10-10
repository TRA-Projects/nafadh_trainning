namespace Bidding_Models.DatabaseModel.Employment
{
    /// <summary>
    /// PostEmploymentRecord.Status values, exactly as written by PostEmploymentRepository
    /// (Active on contract registration and on voided termination, Evaluation Pending on final
    /// evaluation submit, Terminated on termination).
    /// </summary>
    public static class PostEmploymentRecordStatuses
    {
        public const string Active = "Active";
        public const string EvaluationPending = "Evaluation Pending";
        public const string Terminated = "Terminated";
    }
}
