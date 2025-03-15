using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SupportTicketingSystem.Data;
using System.Threading.Tasks;

public class AccountController : Controller
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly SignInManager<ApplicationUser> _signInManager;

    public AccountController(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager)
    {
        _userManager = userManager;
        _signInManager = signInManager;
    }

    // GET: /Account/Register
    public IActionResult Register()
    {
        return View();
    }

    //POST : /Account/Register

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Register(RegisterViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var user = new ApplicationUser
        {
            UserName = model.Email,
            Email = model.Email,
            FullName = model.FullName,
            Team = model.Team
        };

        var result = await _userManager.CreateAsync(user, model.Password);

        if (result.Succeeded)
        {
            await _signInManager.SignInAsync(user, isPersistent: false);
            return RedirectToAction("Index", "Home"); 
        }

        foreach (var error in result.Errors)
        {
            ModelState.AddModelError("", error.Description);
        }

        return View(model);
    }


    // GET: /Account/Login
    public IActionResult Login()
    {
        return View();
    }

    // POST: /Account/Login
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(LoginViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var user = await _userManager.FindByEmailAsync(model.Email);
        if (user == null)
        {
            ModelState.AddModelError(string.Empty, "This email is not registered in the system.");
            return View(model);
        }

        var result = await _signInManager.PasswordSignInAsync(user, model.Password, false, true);

        if (result.Succeeded)
        {
            return RedirectToAction("Index", "Home");
        }
        else if (result.IsLockedOut)
        {
            ModelState.AddModelError(string.Empty, "Your account is locked due to multiple failed login attempts. Please try again later.");
            return View(model);
        }

        ModelState.AddModelError(string.Empty, "Incorrect password. Please try again.");
        return View(model);
    }

    // GET: /Account/Logout
    public async Task<IActionResult> Logout()
    {
        await _signInManager.SignOutAsync();
        return RedirectToAction("Index", "Home");
    }
}
