using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Security.Claims;
using BusinessLogicLayer.Services;
using BusinessLogicLayer.DTOs;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;
using BusinessLogicLayer.Interfaces.Services;
using BusinessLogicLayer.DomainModels;
using System.Linq;

public class EditOutfitModel : PageModel
{
    private readonly IOutfitService _outfitService;
    private readonly IClothingItemService _clothingService;
    private readonly IScheduledOutfitService _scheduledOutfitService;

    public EditOutfitModel(
        IOutfitService outfitService,
        IClothingItemService clothingService,
        IScheduledOutfitService scheduledOutfitService)
    {
        _outfitService = outfitService;
        _clothingService = clothingService;
        _scheduledOutfitService = scheduledOutfitService;
    }

    [BindProperty]
    public OutfitDto Outfit { get; set; } = new();

    [BindProperty]
    public List<int> SelectedClothingItemIds { get; set; } = new();

    [BindProperty]
    public DateTime? ScheduledDate { get; set; }

    public List<ClothingItemDto> AllClothingItems { get; set; } = new();

    public async Task<IActionResult> OnGetAsync(int id)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var outfit = await _outfitService.GetOutfitByIdAsync(id, userId);
        if (outfit == null) return NotFound();

        Outfit = outfit;
        SelectedClothingItemIds = outfit.ClothingItemIds;
        AllClothingItems = await _clothingService.GetAllClothingItemsAsync(userId);

        var existing = await _scheduledOutfitService.GetByOutfitIdAsync(outfit.Id);
        ScheduledDate = existing.FirstOrDefault()?.ScheduledDate;

        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        Outfit.UserId = userId;
        Outfit.ClothingItemIds = SelectedClothingItemIds;

        try
        {
            await _outfitService.UpdateOutfitAsync(Outfit);
        }
        catch (ArgumentException ex)
        {
            ModelState.AddModelError(string.Empty, ex.Message);
            await OnGetAsync(Outfit.Id); 
            return Page();
        }

        if (ScheduledDate.HasValue)
        {
            if (ScheduledDate.Value.Date < DateTime.Today)
            {
                ModelState.AddModelError("ScheduledDate", "Date cannot be in the past.");
                await OnGetAsync(Outfit.Id);
                return Page();
            }

            await _scheduledOutfitService.DeleteByOutfitAsync(Outfit.Id);
            await _scheduledOutfitService.AddAsync(new ScheduledOutfit
            {
                UserId = userId,
                OutfitId = Outfit.Id,
                ScheduledDate = ScheduledDate.Value
            });
        }
        else
        {
            await _scheduledOutfitService.DeleteByOutfitAsync(Outfit.Id);
        }

        return RedirectToPage("/Outfits/MyOutfits");
    }
}
