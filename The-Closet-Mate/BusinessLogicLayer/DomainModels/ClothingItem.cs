using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BusinessLogicLayer.DomainModels

{
    [Table("Items")]
    public class ClothingItem
    {
        public int Id { get; set; }

        [Required]
        public string Name { get; set; } = string.Empty;

        [Required]
        public string Category { get; set; } = string.Empty;

        public string Color { get; set; } = string.Empty;

        public string Season { get; set; } = string.Empty;

        public string? ImagePath { get; set; }

        public string? UserId { get; set; }

        public bool IsFavorite { get; set; }

    }
    
}