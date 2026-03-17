using BusinessLogicLayer.DTOs;

namespace BusinessLogicLayer.Interfaces.Services

{
    public interface IOutfitService
    {
        Task AddOutfitAsync(OutfitDto dto);
        Task UpdateOutfitAsync(OutfitDto dto);
        Task DeleteOutfitAsync(int id, string userId);
        Task<OutfitDto?> GetOutfitByIdAsync(int id, string userId);
        Task<List<OutfitDto>> GetByUserIdAsync(string userId);
        Task<List<ScheduledOutfitCalendarDto>> GetScheduledForCalendarAsync(string userId);


    }
}
