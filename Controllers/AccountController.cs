using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using StudyRoomBooking.ViewModels;

namespace StudyRoomBooking.Controllers;

// Handles registration, login and logout (based on the course demo)
public class AccountController : Controller
{
    private readonly UserManager<IdentityUser> _userManager;
    private readonly SignInManager<IdentityUser> _signInManager;
    private readonly ILogger<AccountController> _logger;

    public AccountController(UserManager<IdentityUser> userManager,
        SignInManager<IdentityUser> signInManager, ILogger<AccountController> logger)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _logger = logger;
    }

    // GET: /Account/Register
    [HttpGet]
    public IActionResult Register() => View();

    // POST: /Account/Register
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Register(RegisterViewModel model)
    {
        if (ModelState.IsValid)
        {
            var user = new IdentityUser { UserName = model.Email, Email = model.Email };
            var result = await _userManager.CreateAsync(user, model.Password);

            if (result.Succeeded)
            {
                await _userManager.AddToRoleAsync(user, "Student");
                _logger.LogInformation("[AccountController] Ny bruker registrert: {Email}", model.Email);
                await _signInManager.SignInAsync(user, isPersistent: false);
                return RedirectToAction("Index", "Home");
            }

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }
        }

        _logger.LogWarning("[AccountController] Registrering feilet for {Email}", model.Email);
        return View(model);
    }

    // GET: /Account/Login
    [HttpGet]
    public IActionResult Login(string? returnUrl = null)
    {
        return View(new LoginViewModel { ReturnUrl = returnUrl });
    }

    // POST: /Account/Login
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(LoginViewModel model)
    {
        // Only redirect to pages in our own app (prevents open redirect attacks)
        string targetUrl = !string.IsNullOrEmpty(model.ReturnUrl) && Url.IsLocalUrl(model.ReturnUrl)
            ? model.ReturnUrl
            : Url.Action("Index", "Home") ?? "~/";

        if (ModelState.IsValid)
        {
            var result = await _signInManager.PasswordSignInAsync(
                model.Email, model.Password, model.RememberMe, lockoutOnFailure: true);

            if (result.Succeeded)
            {
                _logger.LogInformation("[AccountController] {Email} logget inn", model.Email);
                return Redirect(targetUrl);
            }

            if (result.IsLockedOut)
            {
                _logger.LogWarning("[AccountController] Kontoen {Email} er låst etter for mange forsøk", model.Email);
                ModelState.AddModelError(string.Empty, "Too many failed attempts. Try again in 5 minutes.");
                return View(model);
            }
              ModelState.AddModelError(string.Empty,"Invalid login attempt.");
        }  

        _logger.LogWarning("[AccountController] Mislykket innlogging for {Email}", model.Email);
        return View(model);
    }

    // POST: /Account/Logout
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Logout()
    {
        await _signInManager.SignOutAsync();
        return RedirectToAction("Index", "Home");
    }
}