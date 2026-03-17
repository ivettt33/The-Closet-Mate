using System.Collections.Generic;
using System.Threading.Tasks;
using BusinessLogicLayer.DomainModels;
using BusinessLogicLayer.DTOs;

namespace BusinessLogicLayer.Interfaces.Repositories
{
    public interface IOutfitRepository
    {
        Task AddOutfitAsync(Outfit outfit, List<int> clothingItemIds);
        Task UpdateOutfitAsync(Outfit outfit, List<int> clothingItemIds);
        Task DeleteOutfitAsync(int id, string userId);
        Task<Outfit?> GetOutfitByIdAsync(int id, string userId);
        Task<List<Outfit>> GetByUserIdAsync(string userId);
        Task<List<ClothingItem>> GetClothingItemsByIdsAsync(List<int> itemIds);

        Task<List<OutfitWithItemsDto>> GetOutfitsWithItemsByUserAsync(string userId);
    }
}
