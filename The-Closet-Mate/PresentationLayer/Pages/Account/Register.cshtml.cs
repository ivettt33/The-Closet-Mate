using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using BusinessLogicLayer.Interfaces.Services;

public class RegisterModel : PageModel
{
    private readonly IUserService _userService;

    public RegisterModel(IUserService userService)
    {
        _userService = userService;
    }

    [BindProperty]
    public InputModel Input { get; set; } = new(); 

    public class InputModel
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "Passwords do not match.")]
        public string ConfirmPassword { get; set; }
    }

    public async Task<IActionResult> OnPostAsync()
    {
    if (!ModelState.IsValid)
        return Page();

    var (succeeded, errors) = await _userService.RegisterAsync(Input.Email, Input.Password);

    if (succeeded)
        return RedirectToPage("/Index");

    foreach (var error in errors)
    {
        ModelState.AddModelError(string.Empty, error);
    }

    return Page();
    }


}
