using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using BusinessLogicLayer.DTOs;
using BusinessLogicLayer.Services;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Collections.Generic;
using System;
using System.Linq;
using BusinessLogicLayer.Interfaces.Services;
using BusinessLogicLayer.DomainModels;

namespace PresentationLayer.Pages.Outfits
{
    public class CreateModel : PageModel
    {
        private readonly IClothingItemService _clothingItemService;
        private readonly IOutfitService _outfitService;
        private readonly IScheduledOutfitService _scheduledOutfitService;

        public CreateModel(
            IClothingItemService clothingItemService,
            IOutfitService outfitService,
            IScheduledOutfitService scheduledOutfitService)
        {
            _clothingItemService = clothingItemService;
            _outfitService = outfitService;
            _scheduledOutfitService = scheduledOutfitService;
        }

        [BindProperty]
        public string OutfitName { get; set; } = string.Empty;

        [BindProperty]
        public List<int> SelectedItemIds { get; set; } = new();

        [BindProperty]
        public DateTime? ScheduledDate { get; set; }

        public List<ClothingItemDto> AvailableItems { get; set; } = new();

        [TempData]
        public string? FeedbackMessage { get; set; }

        public async Task OnGetAsync()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            AvailableItems = await _clothingItemService.GetAllClothingItemsAsync(userId);
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrWhiteSpace(OutfitName) || SelectedItemIds.Count == 0)
            {
                await OnGetAsync();
                ModelState.AddModelError(string.Empty, "Please enter a name and select at least one item.");
                return Page();
            }

            if (ScheduledDate.HasValue && ScheduledDate.Value.Date < DateTime.Today)
            {
                await OnGetAsync();
                ModelState.AddModelError("ScheduledDate", "Date cannot be in the past.");
                return Page();
            }

            var dto = new OutfitDto
            {
                Name = OutfitName,
                UserId = userId,
                ClothingItemIds = SelectedItemIds
            };

            try
            {
                await _outfitService.AddOutfitAsync(dto);

                if (ScheduledDate.HasValue)
                {
                    var outfits = await _outfitService.GetByUserIdAsync(userId);
                    var newOutfit = outfits.OrderByDescending(o => o.Id).FirstOrDefault();

                    if (newOutfit != null)
                    {
                        await _scheduledOutfitService.AddAsync(new ScheduledOutfit
                        {
                            UserId = userId,
                            OutfitId = newOutfit.Id,
                            ScheduledDate = ScheduledDate.Value
                        });
                    }
                }

                FeedbackMessage = "Outfit successfully created!";
                return RedirectToPage("/Outfits/MyOutfits");
            }
            catch (ArgumentException ex)
            {
                await OnGetAsync();
                ModelState.AddModelError(string.Empty, ex.Message);
                return Page();
            }
        }
    }
}
