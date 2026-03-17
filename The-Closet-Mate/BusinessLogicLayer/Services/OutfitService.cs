
using BusinessLogicLayer.DTOs;
using BusinessLogicLayer.Interfaces.Repositories;
using BusinessLogicLayer.Interfaces.Services;
using BusinessLogicLayer.DomainModels;

namespace BusinessLogicLayer.Services
{
    public class OutfitService : IOutfitService
    {
        private readonly IOutfitRepository _outfitRepo;
        private readonly IScheduledOutfitService _scheduledOutfitService;

        public OutfitService(IOutfitRepository outfitRepo, IScheduledOutfitService scheduledOutfitService)
        {
            _outfitRepo = outfitRepo;
            _scheduledOutfitService = scheduledOutfitService;
        }

        public async Task AddOutfitAsync(OutfitDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.Name))
                throw new ArgumentException("Outfit name is required");
            if (dto.ClothingItemIds == null || dto.ClothingItemIds.Count == 0)
                throw new ArgumentException("Please select at least one clothing item.");

            await ValidateOutfitItemsAsync(dto.ClothingItemIds);

            var outfit = new Outfit
            {
                Name = dto.Name,
                UserId = dto.UserId,
                CreatedAt = DateTime.Now
            };

            await _outfitRepo.AddOutfitAsync(outfit, dto.ClothingItemIds);
        }

        public async Task UpdateOutfitAsync(OutfitDto dto)
        {
            if (dto.ClothingItemIds == null || dto.ClothingItemIds.Count == 0)
                throw new ArgumentException("Please select at least one clothing item.");

            await ValidateOutfitItemsAsync(dto.ClothingItemIds);

            var outfit = new Outfit
            {
                Id = dto.Id,
                Name = dto.Name,
                UserId = dto.UserId
            };

            await _outfitRepo.UpdateOutfitAsync(outfit, dto.ClothingItemIds);
        }

        public async Task DeleteOutfitAsync(int id, string userId)
        {
            await _scheduledOutfitService.DeleteByOutfitAsync(id);
            await _outfitRepo.DeleteOutfitAsync(id, userId);
        }

        public async Task<OutfitDto?> GetOutfitByIdAsync(int id, string userId)
        {
            var outfit = await _outfitRepo.GetOutfitByIdAsync(id, userId);
            if (outfit == null) return null;

            return new OutfitDto
            {
                Id = outfit.Id,
                Name = outfit.Name,
                UserId = outfit.UserId,
                ClothingItemIds = outfit.Items.Select(i => i.ClothingItemId).ToList()
            };
        }

        public async Task<List<OutfitDto>> GetByUserIdAsync(string userId)
        {
            var outfits = await _outfitRepo.GetByUserIdAsync(userId);
            return outfits.Select(o => new OutfitDto
            {
                Id = o.Id,
                Name = o.Name,
                UserId = o.UserId,
                ClothingItemIds = o.Items.Select(i => i.ClothingItemId).ToList()
            }).ToList();
        }

        public async Task<List<ScheduledOutfitCalendarDto>> GetScheduledForCalendarAsync(string userId)
        {
            var scheduledWithNames = await _scheduledOutfitService.GetAllWithOutfitNamesByUserAsync(userId);
            var result = new List<ScheduledOutfitCalendarDto>();

            foreach (var (entry, name) in scheduledWithNames)
            {
                var outfit = await _outfitRepo.GetOutfitByIdAsync(entry.OutfitId, entry.UserId);

                var clothingItems = new List<ClothingItemDto>();

                if (outfit != null && outfit.Items != null)
                {
                    clothingItems = outfit.Items
                        .Where(i => i.ClothingItem != null)
                        .Select(i => new ClothingItemDto
                        {
                            Name = i.ClothingItem.Name,
                            ImagePath = i.ClothingItem.ImagePath
                        }).ToList();
                }

                result.Add(new ScheduledOutfitCalendarDto
                {
                    Title = name,
                    Start = entry.ScheduledDate.ToString("yyyy-MM-dd"),
                    Items = clothingItems
                });
            }

            return result;
        }

        private async Task ValidateOutfitItemsAsync(List<int> clothingItemIds)
        {
            var allItems = await _outfitRepo.GetClothingItemsByIdsAsync(clothingItemIds);

            bool hasShirtOrOuterwear = allItems.Any(i =>
                i.Category.Equals("Shirts", StringComparison.OrdinalIgnoreCase) ||
                i.Category.Equals("Outerwear", StringComparison.OrdinalIgnoreCase));
            bool hasPants = allItems.Any(i => i.Category.Equals("Pants", StringComparison.OrdinalIgnoreCase));
            bool hasDress = allItems.Any(i => i.Category.Equals("Dresses", StringComparison.OrdinalIgnoreCase));
            bool isValid = hasDress || (hasShirtOrOuterwear && hasPants);

            if (!isValid)
                throw new ArgumentException("Outfit must contain either a Dress, or both a Top (Shirt/Outerwear) and a Bottom (Pants).");
        }
    }
}
