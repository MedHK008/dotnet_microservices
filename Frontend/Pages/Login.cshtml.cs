using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Frontend.Models;
using Frontend.Services;

namespace Frontend.Pages;

public class LoginModel : PageModel
{
    private readonly AuthService _authService;

    public LoginModel(AuthService authService)
    {
        _authService = authService;
    }

    [BindProperty]
    public LoginRequest LoginRequest { get; set; } = new();

    public string? ErrorMessage { get; set; }

    public IActionResult OnGet()
    {
        // Check if user is already logged in - redirect to products if authenticated
        if (Request.Cookies["AuthToken"] != null)
        {
            return RedirectToPage("Products");
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

        // Call auth service to login
        var result = await _authService.LoginAsync(LoginRequest);

        if (result != null)
        {
            // Store JWT token in cookie
            var cookieOptions = new CookieOptions
            {
                Expires = DateTime.Now.AddDays(7),
                HttpOnly = true,
                Secure = false,
                SameSite = SameSiteMode.Lax,
                Path = "/"
            };

            Response.Cookies.Append("AuthToken", result.Token, cookieOptions);
            Response.Cookies.Append("UserEmail", result.Email, cookieOptions);

            return RedirectToPage("Products");
        }

        ErrorMessage = "Invalid email or password";
        return Page();
    }
}
