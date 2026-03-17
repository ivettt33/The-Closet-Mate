namespace BusinessLogicLayer.DTOs
{
    public class OutfitItemDto
    {
        public int ClothingItemId { get; set; }
        public string ClothingItemName { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;
        public string? ImagePath { get; set; }
    }
}
