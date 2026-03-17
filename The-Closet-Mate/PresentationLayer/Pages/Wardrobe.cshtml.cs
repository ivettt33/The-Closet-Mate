using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Hosting;
using System.Collections.Generic;
using System.Threading.Tasks;
using BusinessLogicLayer.DTOs;
using BusinessLogicLayer.Services;
using PresentationLayer.ViewModels;
using System.Linq;
using BusinessLogicLayer.Interfaces.Services;

namespace PresentationLayer.Pages
{
    public class WardrobeModel : PageModel
    {
        private readonly IClothingItemService _clothingItemService;
        private readonly IWebHostEnvironment _env;

        public WardrobeModel(IClothingItemService clothingItemService, IWebHostEnvironment env)
        {
            _clothingItemService = clothingItemService;
            _env = env;
        }

        [BindProperty]
        public ClothingItemViewModel NewClothingItem { get; set; } = new();

        public List<ClothingItemViewModel> ClothingItems { get; set; } = new();

        [TempData]
        public string? FavoriteMessage { get; set; }

        public async Task OnGetAsync(string? search, string? category, string? season)
        {
            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (userId != null)
            {
                var dtos = await _clothingItemService.GetFilteredClothingItemsAsync(userId, search, category, season);

                ClothingItems = dtos.Select(d => new ClothingItemViewModel
                {
                    Id = d.Id,
                    Name = d.Name,
                    Category = d.Category,
                    Color = d.Color,
                    Season = d.Season,
                    ImagePath = d.ImagePath,
                    IsFavorite = d.IsFavorite
                }).ToList();
            }
        }


        public async Task<IActionResult> OnPostAddAsync(IFormFile? ImageFile)
        {
            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (userId == null) return Challenge();

            if (!ModelState.IsValid)
            {
                await OnGetAsync(null, null, null);
                return Page();
            }

            var dto = new ClothingItemDto
            {
                Name = NewClothingItem.Name,
                Category = NewClothingItem.Category,
                Color = NewClothingItem.Color,
                Season = NewClothingItem.Season
            };

            await _clothingItemService.AddClothingItemAsync(dto, ImageFile, userId, _env.WebRootPath);
            return RedirectToPage();
        }

        public async Task<IActionResult> OnPostDeleteAsync(int id)
        {
            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (userId != null)
            {
                await _clothingItemService.DeleteClothingItemAsync(id, userId);
            }

            return RedirectToPage();
        }

        public async Task<IActionResult> OnPostFavoriteAsync(int id)
        {
            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (!string.IsNullOrEmpty(userId))
            {
                await _clothingItemService.SetFavoriteStatusAsync(id, userId);
                FavoriteMessage = "Favorite status updated!";
            }

            return RedirectToPage();
        }
    }
}
