namespace Bidding_Models.DatabaseModel.SystemCatalog
{
    /// <summary>Discriminator for rows in the merged <see cref="CatalogSkill"/> table.</summary>
    public enum CatalogSkillKind
    {
        Professional = 1,
        Basic = 2,
        Personal = 3,
        TechStack = 4
    }
}
