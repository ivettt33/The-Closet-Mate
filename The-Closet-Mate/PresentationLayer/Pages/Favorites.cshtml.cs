using BusinessLogicLayer.Services;
using BusinessLogicLayer.DTOs;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Collections.Generic;
using BusinessLogicLayer.Interfaces.Services; 

namespace PresentationLayer.Pages
{
    public class FavoritesModel : PageModel
    {
        private readonly IClothingItemService _clothingItemService;

        public FavoritesModel(IClothingItemService clothingItemService)
        {
            _clothingItemService = clothingItemService;
        }

        public List<ClothingItemDto> FavoriteItems { get; set; } = new();

        public async Task OnGetAsync()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (!string.IsNullOrEmpty(userId))
            {
                FavoriteItems = await _clothingItemService.GetFavoritesByUserAsync(userId);
            }
        }
    }
}
