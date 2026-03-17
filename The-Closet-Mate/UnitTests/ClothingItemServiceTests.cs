using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Xunit;
using BusinessLogicLayer.DTOs;
using BusinessLogicLayer.DomainModels;
using BusinessLogicLayer.Services;
using BusinessLogicLayer.Interfaces.Repositories;

namespace UnitTests
{
    public class ClothingItemServiceTests
    {
        [Fact]
        public async Task AddClothingItemAsync_SavesItemWithImage()
        {
            // Arrange
            var repo = new FakeRepo();
            var service = new ClothingItemService(repo);

            var dto = new ClothingItemDto
            {
                Name = "T-Shirt",
                Category = "Top",
                Color = "Red",
                Season = "Summer"
            };

            var image = new FakeFormFile("shirt.jpg");
            var userId = "user123";
            var webRoot = Path.GetTempPath(); 

            // Act
            await service.AddClothingItemAsync(dto, image, userId, webRoot);

            // Assert
            Assert.Single(repo.SavedItems);
            var saved = repo.SavedItems[0];
            Assert.Equal("T-Shirt", saved.Name);
            Assert.Equal("user123", saved.UserId);
            Assert.False(string.IsNullOrEmpty(saved.ImagePath));
        }

        [Fact]
        public async Task AddClothingItemAsync_SavesItemWithoutImage()
        {
            // Arrange
            var repo = new FakeRepo();
            var service = new ClothingItemService(repo);

            var dto = new ClothingItemDto
            {
                Name = "Jacket",
                Category = "Outerwear",
                Color = "Black",
                Season = "Winter"
            };

            var userId = "user123";
            var webRoot = Path.GetTempPath(); 

            // Act
            await service.AddClothingItemAsync(dto, null, userId, webRoot);

            // Assert
            Assert.Single(repo.SavedItems);
            var saved = repo.SavedItems[0];
            Assert.Equal("Jacket", saved.Name);
            Assert.Null(saved.ImagePath);
        }

        [Theory]
        [InlineData("", "Top", "Red", "Summer", "Name is required")]
        [InlineData("Shirt", "", "Red", "Summer", "Category is required")]
        [InlineData("Shirt", "Top", "", "Summer", "Color is required")]
        [InlineData("Shirt", "Top", "Red", "", "Season is required")]
        public async Task AddClothingItemAsync_ThrowsIfFieldsMissing(
            string name, string category, string color, string season, string expectedError)
        {
            // Arrange
            var repo = new FakeRepo();
            var service = new ClothingItemService(repo);

            var dto = new ClothingItemDto
            {
                Name = name,
                Category = category,
                Color = color,
                Season = season
            };

            var userId = "user123";
            var webRoot = Path.GetTempPath(); 

            // Act
            var ex = await Assert.ThrowsAsync<ArgumentException>(() =>
                service.AddClothingItemAsync(dto, null, userId, webRoot));

            // Assert
            Assert.Equal(expectedError, ex.Message);
        }

        // === Fake Repository ===
        private class FakeRepo : IClothingItemRepository
        {
            public List<ClothingItem> SavedItems { get; } = new();

            public Task AddAsync(ClothingItem item)
            {
                SavedItems.Add(item);
                return Task.CompletedTask;
            }

            public Task DeleteAsync(int id, string userId) => Task.CompletedTask;
            public Task<List<ClothingItem>> GetAllByUserAsync(string userId) => Task.FromResult(new List<ClothingItem>());
            public Task<ClothingItem?> GetByIdAsync(int id) => Task.FromResult<ClothingItem?>(null);
            public Task<List<ClothingItem>> GetFavoritesByUserAsync(string userId) => Task.FromResult(new List<ClothingItem>());
            public Task SetFavoriteStatusAsync(int itemId, string userId) => Task.CompletedTask;
        }

        // === Fake Form File ===
        private class FakeFormFile : IFormFile
        {
            private readonly MemoryStream _stream;

            public FakeFormFile(string fileName)
            {
                _stream = new MemoryStream();
                var writer = new StreamWriter(_stream);
                writer.Write("fake content");
                writer.Flush();
                _stream.Position = 0;

                FileName = fileName;
                Length = _stream.Length;
            }

            public string ContentType => "image/jpeg";
            public string ContentDisposition => "";
            public IHeaderDictionary Headers => new HeaderDictionary();
            public long Length { get; }
            public string Name => "file";
            public string FileName { get; }
            public Stream OpenReadStream() => _stream;
            public void CopyTo(Stream target) => _stream.CopyTo(target);
            public Task CopyToAsync(Stream target, CancellationToken cancellationToken = default)
                => _stream.CopyToAsync(target, cancellationToken);
        }
    }
}
