using BusinessLogicLayer.DTOs;
public class ScheduledOutfitCalendarDto
{
    public string Title { get; set; } = string.Empty;
    public string Start { get; set; } = string.Empty;
    public List<ClothingItemDto> Items { get; set; } = new();
}
