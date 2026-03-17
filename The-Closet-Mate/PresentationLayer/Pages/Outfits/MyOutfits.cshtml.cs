using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Security.Claims;
using BusinessLogicLayer.DTOs;
using BusinessLogicLayer.Services;
using BusinessLogicLayer.Interfaces.Services;

namespace PresentationLayer.Pages.Outfits
{
    public class MyOutfitsModel : PageModel
    {
        private readonly IOutfitQueryService _queryService;
        private readonly IOutfitService _outfitService;

        public MyOutfitsModel(IOutfitQueryService queryService, IOutfitService outfitService)
        {
            _queryService = queryService;
            _outfitService = outfitService;
        }

        public List<OutfitWithItemsDto> Outfits { get; set; } = new();

        public async Task OnGetAsync()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            Outfits = await _queryService.GetOutfitsWithItemsByUserAsync(userId);
        }

        public async Task<IActionResult> OnPostDeleteAsync(int id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            await _outfitService.DeleteOutfitAsync(id, userId);
            return RedirectToPage("/Outfits/MyOutfits");
        }
    }
}
