using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Gotlost.Models.Entities;

namespace Gotlost.Models.Entities
{
    public class FoundItem
    {
        [Key]
        public int FoundItemId { get; set; }

        [Required]
        public string ItemName { get; set; }

        public string Description { get; set; }

        public DateTime FoundDate { get; set; }

        public string Location { get; set; }

        public string ImagePath { get; set; }

        // Foreign Key to User
        public int UserId { get; set; }
        [ForeignKey("UserId")]
        public User User { get; set; }

        // Navigation Property to Claims
        public ICollection<Claim> Claims { get; set; }
    }
}
