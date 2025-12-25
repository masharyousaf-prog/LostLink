using System.ComponentModel.DataAnnotations;

namespace Gotlost.Models.Entities
{
    public class ContactUs
    {
        [Key]
        public int ContactId { get; set; }

        [Required]
        public string Name { get; set; }

        [EmailAddress]
        public string Email { get; set; }

        public string Message { get; set; }

        public DateTime SentAt { get; set; } = DateTime.Now;
    }
}
