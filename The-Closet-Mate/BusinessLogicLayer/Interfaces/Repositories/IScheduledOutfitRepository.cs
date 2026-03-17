using System.Collections.Generic;
using System.Threading.Tasks;
using BusinessLogicLayer.DomainModels;

namespace BusinessLogicLayer.Interfaces.Repositories
{
    public interface IScheduledOutfitRepository
    {
        Task AddAsync(ScheduledOutfit scheduledOutfit);
        Task DeleteByOutfitIdAsync(int outfitId);
        Task<List<ScheduledOutfit>> GetByOutfitIdAsync(int outfitId);
        Task<ScheduledOutfit?> GetByUserAndOutfitAsync(string userId, int outfitId);
        Task UpdateAsync(ScheduledOutfit scheduledOutfit);
        Task<List<(ScheduledOutfit, string)>> GetAllWithOutfitNamesAsync();
        Task<List<(ScheduledOutfit, string)>> GetAllWithOutfitNamesByUserAsync(string userId);

    }
}
