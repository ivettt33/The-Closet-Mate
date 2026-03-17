using BusinessLogicLayer.Interfaces.Repositories;
using BusinessLogicLayer.Interfaces.Services;
using BusinessLogicLayer.DomainModels;



namespace BusinessLogicLayer.Services
{
    public class ScheduledOutfitService : IScheduledOutfitService
    {
        private readonly IScheduledOutfitRepository _scheduledOutfitRepository;

        public ScheduledOutfitService(IScheduledOutfitRepository scheduledOutfitRepository)
        {
            _scheduledOutfitRepository = scheduledOutfitRepository;
        }

        public async Task DeleteByOutfitAsync(int outfitId)
        {
            await _scheduledOutfitRepository.DeleteByOutfitIdAsync(outfitId);
        }

        public async Task AddAsync(ScheduledOutfit scheduledOutfit)
        {
            await _scheduledOutfitRepository.AddAsync(scheduledOutfit);
        }

        public async Task<List<ScheduledOutfit>> GetByOutfitIdAsync(int outfitId)
        {
            return await _scheduledOutfitRepository.GetByOutfitIdAsync(outfitId);
        }

        public async Task<ScheduledOutfit?> GetByUserAndOutfitAsync(string userId, int outfitId)
        {
            return await _scheduledOutfitRepository.GetByUserAndOutfitAsync(userId, outfitId);
        }

        public async Task UpsertScheduledDateAsync(string userId, int outfitId, DateTime scheduledDate)
        {
            var existing = await GetByUserAndOutfitAsync(userId, outfitId);
            if (existing != null)
            {
                existing.ScheduledDate = scheduledDate;
                await _scheduledOutfitRepository.UpdateAsync(existing);
            }
            else
            {
                var newScheduled = new ScheduledOutfit
                {
                    UserId = userId,
                    OutfitId = outfitId,
                    ScheduledDate = scheduledDate
                };
                await _scheduledOutfitRepository.AddAsync(newScheduled);
            }
        }

        public async Task UpdateAsync(ScheduledOutfit scheduledOutfit)
        {
            await _scheduledOutfitRepository.UpdateAsync(scheduledOutfit);
        }

        public async Task<List<(ScheduledOutfit, string)>> GetAllWithOutfitNamesAsync()
        {
            return await _scheduledOutfitRepository.GetAllWithOutfitNamesAsync();
        }
        public async Task<List<(ScheduledOutfit, string)>> GetAllWithOutfitNamesByUserAsync(string userId)
        {
            return await _scheduledOutfitRepository.GetAllWithOutfitNamesByUserAsync(userId);
        }
    }
}
