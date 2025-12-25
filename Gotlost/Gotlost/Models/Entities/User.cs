using System.ComponentModel.DataAnnotations;
using System.Security.Claims;

namespace Gotlost.Models.Entities
{
    public class User
    {
        [Key]
        public int UserId { get; set; }

        [Required]
        public string FullName { get; set; }

        public string Email { get; set; }

        public string PhoneNumber { get; set; }

        public string ProfilePicturePath { get; set; }

        public string Password { get; set; }

        // Navigation
        public ICollection<FoundItem> FoundItems { get; set; }
        public ICollection<Claim> Claims { get; set; }
       
    }
}
