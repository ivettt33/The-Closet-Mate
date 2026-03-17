using BusinessLogicLayer.DTOs;
using BusinessLogicLayer.Interfaces.Repositories;
using Microsoft.AspNetCore.Http;
using BusinessLogicLayer.DomainModels;
using BusinessLogicLayer.Interfaces.Services;
using Microsoft.AspNetCore.Components.Forms.Mapping;


namespace BusinessLogicLayer.Services
{
    public class ClothingItemService : IClothingItemService
    {
        private readonly IClothingItemRepository _repository;

        public ClothingItemService(IClothingItemRepository repository)
        {
            _repository = repository;
        }

        public async Task<List<ClothingItemDto>> GetAllClothingItemsAsync(string userId)
        {
            var items = await _repository.GetAllByUserAsync(userId);
            return items.Select(x => new ClothingItemDto
            {
                Id = x.Id,
                Name = x.Name,
                Category = x.Category,
                Color = x.Color,
                Season = x.Season,
                ImagePath = x.ImagePath,
                IsFavorite = x.IsFavorite
            }).ToList();
        }

        public async Task AddClothingItemAsync(ClothingItemDto dto, IFormFile? imageFile, string userId, string webRootPath)
        {
            if (string.IsNullOrWhiteSpace(dto.Name))
                throw new ArgumentException("Name is required");
            if (string.IsNullOrWhiteSpace(dto.Category))
                throw new ArgumentException("Category is required");
            if (string.IsNullOrWhiteSpace(dto.Color))
                throw new ArgumentException("Color is required");
            if (string.IsNullOrWhiteSpace(dto.Season))
                throw new ArgumentException("Season is required");

            string? imagePath = null;
            if (imageFile != null && imageFile.Length > 0)
            {
                var uploadsFolder = Path.Combine(webRootPath, "uploads");
                Directory.CreateDirectory(uploadsFolder);
                var uniqueFileName = Guid.NewGuid() + Path.GetExtension(imageFile.FileName);
                var filePath = Path.Combine(uploadsFolder, uniqueFileName);
                using var stream = new FileStream(filePath, FileMode.Create);
                await imageFile.CopyToAsync(stream);
                imagePath = "/uploads/" + uniqueFileName;
            }

            var item = new ClothingItem
            {
                Name = dto.Name,
                Category = dto.Category,
                Color = dto.Color,
                Season = dto.Season,
                ImagePath = imagePath,
                UserId = userId
            };

            await _repository.AddAsync(item);
        }

        public async Task DeleteClothingItemAsync(int id, string userId)
        {
            await _repository.DeleteAsync(id, userId);
        }

        public async Task SetFavoriteStatusAsync(int itemId, string userId)
        {
            await _repository.SetFavoriteStatusAsync(itemId, userId);
        }

        public async Task<List<ClothingItemDto>> GetFavoritesByUserAsync(string userId)
        {
            var favorites = await _repository.GetFavoritesByUserAsync(userId);
            return favorites.Select(x => new ClothingItemDto
            {
                Id = x.Id,
                Name = x.Name,
                Category = x.Category,
                Color = x.Color,
                Season = x.Season,
                ImagePath = x.ImagePath,
                IsFavorite = x.IsFavorite
            }).ToList();
        }

        public async Task<List<ClothingItemDto>> GetFilteredClothingItemsAsync(string userId, string? search, string? category, string? season)
        {
            var items = await _repository.GetAllByUserAsync(userId);
            if (!string.IsNullOrWhiteSpace(search))
                items = items.Where(d => d.Name.Contains(search, StringComparison.OrdinalIgnoreCase)).ToList();
            if (!string.IsNullOrWhiteSpace(category))
                items = items.Where(d => d.Category.Equals(category, StringComparison.OrdinalIgnoreCase)).ToList();
            if (!string.IsNullOrWhiteSpace(season))
                items = items.Where(d => d.Season.Equals(season, StringComparison.OrdinalIgnoreCase)).ToList();

            return items.Select(x => new ClothingItemDto
            {
                Id = x.Id,
                Name = x.Name,
                Category = x.Category,
                Color = x.Color,
                Season = x.Season,
                ImagePath = x.ImagePath,
                IsFavorite = x.IsFavorite
            }).ToList();
        }
    }
}

