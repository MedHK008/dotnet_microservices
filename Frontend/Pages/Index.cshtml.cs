using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Frontend.Pages;

public class IndexModel : PageModel
{
    public IActionResult OnGet()
    {
        // Redirect to products if logged in, otherwise to login
        var token = Request.Cookies["AuthToken"];
        if (string.IsNullOrEmpty(token))
        {
            return RedirectToPage("/Login");
        }
        return RedirectToPage("/Products");
    }
}
