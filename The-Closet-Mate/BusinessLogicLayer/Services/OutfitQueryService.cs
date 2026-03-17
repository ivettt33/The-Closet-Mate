using BusinessLogicLayer.DTOs;
using BusinessLogicLayer.Interfaces.Services;
using BusinessLogicLayer.Interfaces.Repositories;

namespace BusinessLogicLayer.Services
{
    public class OutfitQueryService : IOutfitQueryService
    {
        private readonly IOutfitRepository _repository;

        public OutfitQueryService(IOutfitRepository repository)
        {
            _repository = repository;
        }

        public async Task<List<OutfitWithItemsDto>> GetOutfitsWithItemsByUserAsync(string userId)
        {
            return await _repository.GetOutfitsWithItemsByUserAsync(userId);
        }
    }
}
