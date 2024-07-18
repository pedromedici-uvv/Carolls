using IdentityModel;
using IdentityService.Models;
using IdentityService.Pages.Account.Register;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Security.Claims;

namespace IdentityService.Pages.Register;
[SecurityHeaders]
[AllowAnonymous]
public class Index : PageModel
{
    private UserManager<ApplicationUser> _userManager;

    public Index(UserManager<ApplicationUser> userManager)
    {
        _userManager = userManager;
    }
    [BindProperty]
    public RegisterViewModel Input { get; set; }

    [BindProperty]
    public bool RegisterSucess { get; set; }
    public ActionResult OnGet(string returnUrl)
    {
        Input = new RegisterViewModel
        {
            ReturnUrl = returnUrl,
        };

        return Page();
    }

    public async Task<IActionResult> OnPost()
    {
        if (Input.Button != "register") return Redirect("~/");

        if (ModelState.IsValid)
        {
            var user = new ApplicationUser
            {
                UserName = Input.Username,
                Email = Input.Email,
                EmailConfirmed = true,
            };

            var result = await _userManager.CreateAsync(user, Input.Password);

            if (result.Succeeded)
            {
                await _userManager.AddClaimAsync(user, new Claim(JwtClaimTypes.Name, user.UserName));
                RegisterSucess = true;
            }
        }

        return Page();

    }
    
}
