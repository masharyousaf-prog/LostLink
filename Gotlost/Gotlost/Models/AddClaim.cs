using System.ComponentModel.DataAnnotations;

namespace Gotlost.Models
{
    public class AddClaim
    {
        [Required]
        public int FoundItemId { get; set; }

        [Required]
        [StringLength(1000, ErrorMessage = "Description is too long.")]
        public string Description { get; set; }
    }
}
