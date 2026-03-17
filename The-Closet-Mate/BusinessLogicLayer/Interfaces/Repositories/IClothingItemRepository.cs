using System.Collections.Generic;
using System.Threading.Tasks;
using BusinessLogicLayer.DomainModels;

namespace BusinessLogicLayer.Interfaces.Repositories
{
    public interface IClothingItemRepository
    {
        Task<List<ClothingItem>> GetAllByUserAsync(string userId);
        Task<ClothingItem?> GetByIdAsync(int id);
        Task AddAsync(ClothingItem item);
        Task DeleteAsync(int id, string userId);
        Task SetFavoriteStatusAsync(int itemId, string userId);
        Task<List<ClothingItem>> GetFavoritesByUserAsync(string userId);
    }
}
