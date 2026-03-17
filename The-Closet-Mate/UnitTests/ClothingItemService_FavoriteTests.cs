using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;
using BusinessLogicLayer.Services;
using BusinessLogicLayer.DomainModels;
using BusinessLogicLayer.Interfaces.Repositories;

namespace UnitTests
{
    public class ClothingItemService_FavoriteTests
    {
        [Fact]
        public async Task SetFavoriteStatusAsync_ShouldToggleFavorite_WhenValid()
        {
            // Arrange
            var repo = new FakeClothingItemRepository();
            var service = new ClothingItemService(repo);

            // Act
            await service.SetFavoriteStatusAsync(1, "user123");

            // Assert
            Assert.True(repo.FavoriteToggled);
        }

        [Fact]
        public async Task SetFavoriteStatusAsync_ShouldThrow_WhenRepositoryFails()
        {
            // Arrange
            var repo = new FakeClothingItemRepository
            {
                SetFavoriteShouldThrow = true
            };
            var service = new ClothingItemService(repo);

            // Act & Assert
            await Assert.ThrowsAsync<Exception>(() => service.SetFavoriteStatusAsync(1, "user123"));
        }

        // Minimal Fake Repository
        private class FakeClothingItemRepository : IClothingItemRepository
        {
            public bool FavoriteToggled { get; private set; }
            public bool SetFavoriteShouldThrow { get; set; }

            public Task SetFavoriteStatusAsync(int itemId, string userId)
            {
                if (SetFavoriteShouldThrow)
                    throw new Exception("Forced failure");

                FavoriteToggled = true;
                return Task.CompletedTask;
            }

            public Task AddAsync(ClothingItem item) => Task.CompletedTask;
            public Task DeleteAsync(int id, string userId) => Task.CompletedTask;
            public Task<List<ClothingItem>> GetAllByUserAsync(string userId) => Task.FromResult(new List<ClothingItem>());
            public Task<ClothingItem?> GetByIdAsync(int id) => Task.FromResult<ClothingItem?>(null);
            public Task<List<ClothingItem>> GetFavoritesByUserAsync(string userId) => Task.FromResult(new List<ClothingItem>());
        }
    }
}
