using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Threading.Tasks;
using BusinessLogicLayer.Interfaces.Services;

public class LogoutModel : PageModel
{
    private readonly IUserService _userService;

    public LogoutModel(IUserService userService)
    {
        _userService = userService;
    }

    public async Task<IActionResult> OnGet()
    {
        await _userService.LogoutAsync();
        return RedirectToPage("/Index");
    }
}
