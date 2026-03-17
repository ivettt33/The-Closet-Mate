using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using BusinessLogicLayer.DomainModels;
using BusinessLogicLayer.DTOs;
using BusinessLogicLayer.Interfaces.Repositories;
using DataAccessLayer.Database;

namespace DataAccessLayer.Repositories
{
    public class OutfitRepository : IOutfitRepository
    {
        private readonly ClosetContext _context;

        public OutfitRepository(ClosetContext context)
        {
            _context = context;
        }

        public async Task AddOutfitAsync(Outfit outfit, List<int> clothingItemIds)
        {
            await _context.Outfits.AddAsync(outfit);
            await _context.SaveChangesAsync();

            var outfitItems = clothingItemIds.Select(id => new OutfitItem
            {
                OutfitId = outfit.Id,
                ClothingItemId = id
            }).ToList();

            await _context.OutfitItems.AddRangeAsync(outfitItems);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateOutfitAsync(Outfit outfit, List<int> clothingItemIds)
        {
            var existingOutfit = await _context.Outfits
                .Include(o => o.Items)
                .FirstOrDefaultAsync(o => o.Id == outfit.Id && o.UserId == outfit.UserId);

            if (existingOutfit == null) return;

            existingOutfit.Name = outfit.Name;

            _context.OutfitItems.RemoveRange(existingOutfit.Items);

            var updatedItems = clothingItemIds.Select(id => new OutfitItem
            {
                OutfitId = outfit.Id,
                ClothingItemId = id
            }).ToList();

            await _context.OutfitItems.AddRangeAsync(updatedItems);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteOutfitAsync(int id, string userId)
        {
            var outfit = await _context.Outfits
                .Include(o => o.Items)
                .FirstOrDefaultAsync(o => o.Id == id && o.UserId == userId);

            if (outfit != null)
            {
                _context.OutfitItems.RemoveRange(outfit.Items);
                _context.Outfits.Remove(outfit);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<Outfit?> GetOutfitByIdAsync(int id, string userId)
        {
            return await _context.Outfits
                .Include(o => o.Items)
                    .ThenInclude(oi => oi.ClothingItem) 
                .FirstOrDefaultAsync(o => o.Id == id && o.UserId == userId);
        }

        public async Task<List<Outfit>> GetByUserIdAsync(string userId)
        {
            return await _context.Outfits
                .Include(o => o.Items)
                .Where(o => o.UserId == userId)
                .ToListAsync();
        }

        public async Task<List<ClothingItem>> GetClothingItemsByIdsAsync(List<int> itemIds)
        {
            return await _context.ClothingItems
                .Where(i => itemIds.Contains(i.Id))
                .ToListAsync();
        }

        public async Task<List<OutfitWithItemsDto>> GetOutfitsWithItemsByUserAsync(string userId)
        {
            return await _context.Outfits
                .Where(o => o.UserId == userId)
                .Include(o => o.Items)
                    .ThenInclude(oi => oi.ClothingItem)
                .Select(o => new OutfitWithItemsDto
                {
                    Id = o.Id,
                    Name = o.Name,
                    UserId = o.UserId,
                    CreatedAt = o.CreatedAt,
                    Items = o.Items.Select(i => new ClothingItemDto
                    {
                        Id = i.ClothingItemId,
                        Name = i.ClothingItem.Name,
                        ImagePath = i.ClothingItem.ImagePath
                    }).ToList()
                })
                .ToListAsync();
        }
    }
}
