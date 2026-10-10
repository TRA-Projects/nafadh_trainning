namespace Bidding_Models.DatabaseModel.Employment
{
    /// <summary>
    /// Single contracts row lifecycle phase: Offer (offers UI) → Contract (after candidate accepts).
    /// </summary>
    public static class EmploymentContractPhases
    {
        public const string Offer = "Offer";
        public const string Contract = "Contract";
    }
}
