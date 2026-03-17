using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;
using BusinessLogicLayer.DomainModels;
using BusinessLogicLayer.Services;
using BusinessLogicLayer.Interfaces.Repositories;

namespace TheClosetMate.Tests.Services
{
    public class ScheduledOutfitServiceTests
    {
        [Fact]
        public async Task AddAsync_ShouldSaveItem()
        {
            // Arrange
            var repo = new FakeRepo();
            var service = new ScheduledOutfitService(repo);
            var item = new ScheduledOutfit { UserId = "user1", OutfitId = 10, ScheduledDate = DateTime.Today };

            // Act
            await service.AddAsync(item);

            // Assert
            var saved = await repo.GetByUserAndOutfitAsync("user1", 10);
            Assert.NotNull(saved);
        }

        [Fact]
        public async Task Upsert_ShouldAdd_WhenNotExists()
        {
            // Arrange
            var repo = new FakeRepo();
            var service = new ScheduledOutfitService(repo);

            // Act
            await service.UpsertScheduledDateAsync("newUser", 5, DateTime.Today);

            // Assert
            var result = await repo.GetByUserAndOutfitAsync("newUser", 5);
            Assert.NotNull(result);
        }

        [Fact]
        public async Task Upsert_ShouldUpdate_WhenExists()
        {
            // Arrange
            var repo = new FakeRepo();
            var service = new ScheduledOutfitService(repo);
            await service.UpsertScheduledDateAsync("userX", 1, DateTime.Today);

            // Act
            await service.UpsertScheduledDateAsync("userX", 1, new DateTime(2025, 12, 31));

            // Assert
            var result = await repo.GetByUserAndOutfitAsync("userX", 1);
            Assert.Equal(new DateTime(2025, 12, 31), result!.ScheduledDate);
        }

        [Fact]
        public async Task DeleteByOutfit_ShouldRemoveEntries()
        {
            // Arrange
            var repo = new FakeRepo();
            var service = new ScheduledOutfitService(repo);
            await service.UpsertScheduledDateAsync("user2", 99, DateTime.Today);

            // Act
            await service.DeleteByOutfitAsync(99);

            // Assert
            var result = await repo.GetByOutfitIdAsync(99);
            Assert.Empty(result);
        }

        [Fact]
        public async Task UpdateAsync_ShouldChangeDate()
        {
            // Arrange
            var repo = new FakeRepo();
            var service = new ScheduledOutfitService(repo);
            await service.UpsertScheduledDateAsync("u1", 100, new DateTime(2025, 1, 1));

            var updated = new ScheduledOutfit
            {
                UserId = "u1",
                OutfitId = 100,
                ScheduledDate = new DateTime(2025, 2, 2)
            };

            // Act
            await service.UpdateAsync(updated);

            // Assert
            var result = await repo.GetByUserAndOutfitAsync("u1", 100);
            Assert.Equal(new DateTime(2025, 2, 2), result!.ScheduledDate);
        }

        [Fact]
        public async Task GetByOutfitIdAsync_ShouldReturnAll()
        {
            // Arrange
            var repo = new FakeRepo();
            var service = new ScheduledOutfitService(repo);
            await service.AddAsync(new ScheduledOutfit { UserId = "A", OutfitId = 200, ScheduledDate = DateTime.Today });
            await service.AddAsync(new ScheduledOutfit { UserId = "B", OutfitId = 200, ScheduledDate = DateTime.Today });

            // Act
            var result = await service.GetByOutfitIdAsync(200);

            // Assert
            Assert.Equal(2, result.Count);
        }

        [Fact]
        public async Task GetByUserAndOutfitAsync_ShouldReturnNull_IfNotFound()
        {
            // Arrange
            var repo = new FakeRepo();
            var service = new ScheduledOutfitService(repo);

            // Act
            var result = await service.GetByUserAndOutfitAsync("ghost", 123);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task UpdateAsync_ShouldDoNothing_IfNotExists()
        {
            // Arrange
            var repo = new FakeRepo();
            var service = new ScheduledOutfitService(repo);

            var update = new ScheduledOutfit
            {
                UserId = "nobody",
                OutfitId = 999,
                ScheduledDate = DateTime.Today
            };

            // Act
            await service.UpdateAsync(update);

            // Assert
            var result = await repo.GetByUserAndOutfitAsync("nobody", 999);
            Assert.Null(result);
        }

        [Fact]
        public async Task DeleteByOutfitAsync_ShouldNotThrow_IfNoneExist()
        {
            // Arrange
            var repo = new FakeRepo();
            var service = new ScheduledOutfitService(repo);

            // Act
            var exception = await Record.ExceptionAsync(() => service.DeleteByOutfitAsync(404));

            // Assert
            Assert.Null(exception);
        }
    }

    // === Simple Fake Repository ===
    public class FakeRepo : IScheduledOutfitRepository
    {
        private readonly List<ScheduledOutfit> _storage = new();

        public Task AddAsync(ScheduledOutfit item)
        {
            _storage.Add(item);
            return Task.CompletedTask;
        }

        public Task DeleteByOutfitIdAsync(int outfitId)
        {
            _storage.RemoveAll(x => x.OutfitId == outfitId);
            return Task.CompletedTask;
        }

        public Task<List<ScheduledOutfit>> GetByOutfitIdAsync(int outfitId)
        {
            return Task.FromResult(_storage.Where(x => x.OutfitId == outfitId).ToList());
        }

        public Task<ScheduledOutfit?> GetByUserAndOutfitAsync(string userId, int outfitId)
        {
            var match = _storage.FirstOrDefault(x => x.UserId == userId && x.OutfitId == outfitId);
            return Task.FromResult(match);
        }

        public Task UpdateAsync(ScheduledOutfit updated)
        {
            var existing = _storage.FirstOrDefault(x => x.UserId == updated.UserId && x.OutfitId == updated.OutfitId);
            if (existing != null)
            {
                existing.ScheduledDate = updated.ScheduledDate;
            }
            return Task.CompletedTask;
        }

        public Task<List<(ScheduledOutfit, string)>> GetAllWithOutfitNamesAsync()
        {
            return Task.FromResult(new List<(ScheduledOutfit, string)>());
        }
    }
}
