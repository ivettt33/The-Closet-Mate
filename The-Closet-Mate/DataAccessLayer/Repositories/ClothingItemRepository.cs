using BusinessLogicLayer.DomainModels;
using BusinessLogicLayer.Interfaces.Repositories;
using DataAccessLayer.Database;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace DataAccessLayer.Repositories
{
    public class ClothingItemRepository : IClothingItemRepository
    {
        private readonly ClosetContext _context;

        public ClothingItemRepository(ClosetContext context)
        {
            _context = context;
        }

        public async Task<List<ClothingItem>> GetAllByUserAsync(string userId)
        {
            return await _context.ClothingItems
                .Where(c => c.UserId == userId)
                .ToListAsync();
        }

        public async Task<ClothingItem?> GetByIdAsync(int id)
        {
            return await _context.ClothingItems.FindAsync(id);
        }

        public async Task AddAsync(ClothingItem item)
        {
            _context.ClothingItems.Add(item);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id, string userId)
        {
            var item = await _context.ClothingItems
                .FirstOrDefaultAsync(i => i.Id == id && i.UserId == userId);

            if (item != null)
            {
                _context.ClothingItems.Remove(item);
                await _context.SaveChangesAsync();
            }
        }

        public async Task SetFavoriteStatusAsync(int itemId, string userId)
        {
            var sql = @"
                UPDATE Items
                SET IsFavorite = 
                    CASE 
                        WHEN IsFavorite = 1 THEN 0 
                        ELSE 1 
                    END
                WHERE Id = @Id AND UserId = @UserId;
            ";

            var parameters = new[]
            {
                new SqlParameter("@Id", itemId),
                new SqlParameter("@UserId", userId)
            };

            await _context.Database.ExecuteSqlRawAsync(sql, parameters);
        }

        public async Task<List<ClothingItem>> GetFavoritesByUserAsync(string userId)
        {
            var sql = @"
                SELECT * FROM Items
                WHERE UserId = @userId AND IsFavorite = 1";

            var parameter = new SqlParameter("@userId", userId);

            return await _context.ClothingItems
                .FromSqlRaw(sql, parameter)
                .ToListAsync();
        }
    }
}
