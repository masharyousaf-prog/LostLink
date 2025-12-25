using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Gotlost.Models.Entities
{
    public class Claim
    {
        public int ClaimId { get; set; }
        public string Status { get; set; }
        public DateTime ClaimDate { get; set; }
        public string Description { get; set; }
        // FK for LostItem

        // FK for FoundItem
        public int FoundItemId { get; set; }
        [ForeignKey("FoundItemId")]
        public FoundItem FoundItem { get; set; }

        // FK for User
        public int UserId { get; set; }
        [ForeignKey("UserId")]
        public User User { get; set; }
    }


}
