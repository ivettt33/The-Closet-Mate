namespace BusinessLogicLayer.DTOs
{
    public class OutfitDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string UserId { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }

        public DateTime? ScheduledDate { get; set; } 

        public List<int> ClothingItemIds { get; set; } = new();
    }
}
