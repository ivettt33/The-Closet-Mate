using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using BusinessLogicLayer.DomainModels;

namespace BusinessLogicLayer.Interfaces.Services

{
    public interface IScheduledOutfitService
    {
        Task DeleteByOutfitAsync(int outfitId);
        Task AddAsync(ScheduledOutfit scheduledOutfit);
        Task<ScheduledOutfit?> GetByUserAndOutfitAsync(string userId, int outfitId);
        Task<List<ScheduledOutfit>> GetByOutfitIdAsync(int outfitId);
        Task UpsertScheduledDateAsync(string userId, int outfitId, DateTime scheduledDate);
        Task UpdateAsync(ScheduledOutfit scheduledOutfit);


        Task<List<(ScheduledOutfit, string)>> GetAllWithOutfitNamesAsync();
        Task<List<(ScheduledOutfit, string)>> GetAllWithOutfitNamesByUserAsync(string userId);

    }
}
