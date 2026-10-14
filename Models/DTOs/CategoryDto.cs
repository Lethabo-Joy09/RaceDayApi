using System.ComponentModel.DataAnnotations;

namespace RaceDayApi.Models.DTOs
{
    public class CategoryDto
    {
        public int CategoryId { get; set; }
        public string CategoryName { get; set; } = string.Empty;
        public string? Description { get; set; }
        public decimal DistanceKM { get; set; }
    }

    public class CreateCategoryDto
    {
        [Required, MaxLength(50)]
        public string CategoryName { get; set; } = string.Empty;

        [MaxLength(200)]
        public string? Description { get; set; }

        [Required, Range(0.1, 999.99)]
        public decimal DistanceKM { get; set; }
    }

    public class UpdateCategoryDto
    {
        [MaxLength(50)]
        public string? CategoryName { get; set; }

        [MaxLength(200)]
        public string? Description { get; set; }

        [Range(0.1, 999.99)]
        public decimal? DistanceKM { get; set; }
    }
}