using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Collections.Generic;
using System.Threading.Tasks;
using BusinessLogicLayer.DTOs;
using BusinessLogicLayer.Interfaces.Services;

namespace PresentationLayer.Pages
{
    public class PlannerModel : PageModel
    {
        private readonly IOutfitService _outfitService;

        public PlannerModel(IOutfitService outfitService)
        {
            _outfitService = outfitService;
        }

        public List<ScheduledOutfitCalendarDto> ScheduledOutfits { get; set; } = new();

        public async Task OnGetAsync()
        {
            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (userId != null)
            {
                ScheduledOutfits = await _outfitService.GetScheduledForCalendarAsync(userId);
            }
        }
    }
}
