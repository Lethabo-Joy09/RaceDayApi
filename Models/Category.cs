using System.ComponentModel.DataAnnotations;

namespace RaceDayApi.Models
{
    public class Category
    {
        [Key]
        public int CategoryID { get; set; }

        [Required, MaxLength(50)]
        public string CategoryName { get; set; } = string.Empty;

        [MaxLength(200)]
        public string? Description { get; set; }

        [Required]
        public decimal DistanceKM { get; set; }

        // Navigation
        public ICollection<EventCategory> EventCategories { get; set; } = new List<EventCategory>();
    }
}