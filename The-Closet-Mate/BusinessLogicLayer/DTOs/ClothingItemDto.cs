namespace BusinessLogicLayer.DTOs
{
    public class ClothingItemDto
    {
        public int Id { get; set; }

        public string Name { get; set; } = string.Empty;

        public string Category { get; set; } = string.Empty;

        public string Color { get; set; } = string.Empty;

        public string Season { get; set; } = string.Empty;

        public string? ImagePath { get; set; }

        public bool IsFavorite { get; set; }
    }
}
