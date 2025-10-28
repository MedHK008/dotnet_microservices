using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Frontend.Models;
using Frontend.Services;

namespace Frontend.Pages;

public class RegisterModel : PageModel
{
    private readonly AuthService _authService;

    public RegisterModel(AuthService authService)
    {
        _authService = authService;
    }

    [BindProperty]
    public RegisterRequest RegisterRequest { get; set; } = new();

    public string? ErrorMessage { get; set; }

    public IActionResult OnGet()
    {
        // Check if user is already logged in - redirect to products if authenticated
        if (Request.Cookies["AuthToken"] != null)
        {
            return RedirectToPage("/Products");
        }
        
        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
        {
            ErrorMessage = "Please fill in all fields";
            return Page();
        }

        // Call auth service to register
        var result = await _authService.RegisterAsync(RegisterRequest);

        if (result != null)
        {
            // Store JWT token in cookie
            var cookieOptions = new CookieOptions
            {
                Expires = DateTime.Now.AddDays(7),
                HttpOnly = true,
                Secure = false,
                SameSite = SameSiteMode.Lax
            };

            Response.Cookies.Append("AuthToken", result.Token, cookieOptions);
            Response.Cookies.Append("UserEmail", result.Email, cookieOptions);

            return RedirectToPage("/Products");
        }

        ErrorMessage = "User already exists or registration failed";
        return Page();
    }
}
