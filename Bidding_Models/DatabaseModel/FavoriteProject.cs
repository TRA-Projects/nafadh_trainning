using System.ComponentModel.DataAnnotations;

namespace Bidding_Models.DatabaseModel
{
    public class FavoriteProject
    {
        public int ProfileId { get; set; }
        public int ProjectId { get; set; }
        public Profile Profile { get; set; }
        public ProfileProject Project { get; set; }
    }

}
