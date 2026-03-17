using BusinessLogicLayer.DTOs;

namespace BusinessLogicLayer.Interfaces.Services
{
    public interface IOutfitQueryService
    {
        Task<List<OutfitWithItemsDto>> GetOutfitsWithItemsByUserAsync(string userId);
    }
}
