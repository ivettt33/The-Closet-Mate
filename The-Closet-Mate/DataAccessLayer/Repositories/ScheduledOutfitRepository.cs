using BusinessLogicLayer.DomainModels;
using BusinessLogicLayer.Interfaces.Repositories;
using Microsoft.Data.SqlClient;

namespace DataAccessLayer.Repositories
{
    public class ScheduledOutfitRepository : IScheduledOutfitRepository
    {
        private readonly string _connectionString;

        public ScheduledOutfitRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        public async Task AddAsync(ScheduledOutfit scheduledOutfit)
        {
            using var conn = new SqlConnection(_connectionString);
            await conn.OpenAsync();
            using var cmd = new SqlCommand("INSERT INTO ScheduledOutfits (UserId, OutfitId, ScheduledDate) VALUES (@userId, @outfitId, @date)", conn);
            cmd.Parameters.AddWithValue("@userId", scheduledOutfit.UserId);
            cmd.Parameters.AddWithValue("@outfitId", scheduledOutfit.OutfitId);
            cmd.Parameters.AddWithValue("@date", scheduledOutfit.ScheduledDate);
            await cmd.ExecuteNonQueryAsync();
        }

        public async Task<List<ScheduledOutfit>> GetByOutfitIdAsync(int outfitId)
        {
            var result = new List<ScheduledOutfit>();
            using var conn = new SqlConnection(_connectionString);
            await conn.OpenAsync();
            using var cmd = new SqlCommand("SELECT * FROM ScheduledOutfits WHERE OutfitId = @outfitId", conn);
            cmd.Parameters.AddWithValue("@outfitId", outfitId);
            using var reader = await cmd.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                result.Add(new ScheduledOutfit
                {
                    Id = reader.GetInt32(0),
                    UserId = reader.GetString(1),
                    OutfitId = reader.GetInt32(2),
                    ScheduledDate = reader.GetDateTime(3)
                });
            }
            return result;
        }

        public async Task<ScheduledOutfit?> GetByUserAndOutfitAsync(string userId, int outfitId)
        {
            using var conn = new SqlConnection(_connectionString);
            await conn.OpenAsync();

            using var cmd = new SqlCommand(@"
                SELECT Id, UserId, OutfitId, ScheduledDate
                FROM ScheduledOutfits
                WHERE UserId = @userId AND OutfitId = @outfitId", conn);

            cmd.Parameters.AddWithValue("@userId", userId);
            cmd.Parameters.AddWithValue("@outfitId", outfitId);

            using var reader = await cmd.ExecuteReaderAsync();
            if (await reader.ReadAsync())
            {
                return new ScheduledOutfit
                {
                    Id = reader.GetInt32(0),
                    UserId = reader.GetString(1),
                    OutfitId = reader.GetInt32(2),
                    ScheduledDate = reader.GetDateTime(3)
                };
            }

            return null;
        }

        public async Task UpdateAsync(ScheduledOutfit scheduledOutfit)
        {
            using var conn = new SqlConnection(_connectionString);
            await conn.OpenAsync();

            using var cmd = new SqlCommand(@"
                UPDATE ScheduledOutfits
                SET ScheduledDate = @date
                WHERE UserId = @userId AND OutfitId = @outfitId", conn);

            cmd.Parameters.AddWithValue("@date", scheduledOutfit.ScheduledDate);
            cmd.Parameters.AddWithValue("@userId", scheduledOutfit.UserId);
            cmd.Parameters.AddWithValue("@outfitId", scheduledOutfit.OutfitId);

            await cmd.ExecuteNonQueryAsync();
        }

        public async Task DeleteByOutfitIdAsync(int outfitId)
        {
            using var conn = new SqlConnection(_connectionString);
            await conn.OpenAsync();
            using var cmd = new SqlCommand("DELETE FROM ScheduledOutfits WHERE OutfitId = @outfitId", conn);
            cmd.Parameters.AddWithValue("@outfitId", outfitId);
            await cmd.ExecuteNonQueryAsync();
        }

        public async Task<List<(ScheduledOutfit, string)>> GetAllWithOutfitNamesAsync()
        {
            var result = new List<(ScheduledOutfit, string)>();

            using var conn = new SqlConnection(_connectionString);
            await conn.OpenAsync();

            using var cmd = new SqlCommand(@"
                SELECT so.Id, so.UserId, so.OutfitId, so.ScheduledDate, o.Name
                FROM ScheduledOutfits so
                INNER JOIN Outfits o ON so.OutfitId = o.Id", conn);

            using var reader = await cmd.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                var scheduled = new ScheduledOutfit
                {
                    Id = reader.GetInt32(0),
                    UserId = reader.GetString(1),
                    OutfitId = reader.GetInt32(2),
                    ScheduledDate = reader.GetDateTime(3)
                };

                string outfitName = reader.GetString(4);
                result.Add((scheduled, outfitName));
            }

            return result;
        }
        public async Task<List<(ScheduledOutfit, string)>> GetAllWithOutfitNamesByUserAsync(string userId)
        {
            var result = new List<(ScheduledOutfit, string)>();

            using var conn = new SqlConnection(_connectionString);
            await conn.OpenAsync();

            using var cmd = new SqlCommand(@"
        SELECT so.Id, so.UserId, so.OutfitId, so.ScheduledDate, o.Name
        FROM ScheduledOutfits so
        INNER JOIN Outfits o ON so.OutfitId = o.Id
        WHERE so.UserId = @userId", conn);

            cmd.Parameters.AddWithValue("@userId", userId);

            using var reader = await cmd.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                var scheduled = new ScheduledOutfit
                {
                    Id = reader.GetInt32(0),
                    UserId = reader.GetString(1),
                    OutfitId = reader.GetInt32(2),
                    ScheduledDate = reader.GetDateTime(3)
                };

                string outfitName = reader.GetString(4);
                result.Add((scheduled, outfitName));
            }

            return result;
        }
    }
}
