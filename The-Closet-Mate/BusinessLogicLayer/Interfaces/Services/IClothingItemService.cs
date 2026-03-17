using BusinessLogicLayer.DTOs;
using Microsoft.AspNetCore.Http;


namespace BusinessLogicLayer.Interfaces.Services

{
    public interface IClothingItemService
    {
        Task<List<ClothingItemDto>> GetAllClothingItemsAsync(string userId);
        Task AddClothingItemAsync(ClothingItemDto item, IFormFile? imageFile, string userId, string webRootPath);
        Task DeleteClothingItemAsync(int id, string userId);
        Task SetFavoriteStatusAsync(int itemId, string userId);
        Task<List<ClothingItemDto>> GetFavoritesByUserAsync(string userId);
        Task<List<ClothingItemDto>> GetFilteredClothingItemsAsync(string userId, string? search, string? category, string? season);

    }

}