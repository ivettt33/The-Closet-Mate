using BusinessLogicLayer.DTOs;

public class OutfitWithItemsDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string UserId { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }

    public List<ClothingItemDto> Items { get; set; } = new();
    public DateTime? ScheduledDate { get; set; }

    public string? ScheduledDateText => ScheduledDate.HasValue
        ? ScheduledDate.Value.ToString("yyyy-MM-dd")
        : null;
}
