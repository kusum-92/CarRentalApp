using Microsoft.AspNetCore.Mvc;
using CarRentalApp.Models;
using Microsoft.AspNetCore.Identity;



namespace CarRentalApp.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<IdentityUser> _user;
        private readonly SignInManager<IdentityUser> _sign;

        public AccountController(UserManager<IdentityUser> user, SignInManager<IdentityUser> sign)
        {
            _sign = sign;
            _user = user;
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = new IdentityUser
                {
                    UserName = model.Email,
                    Email = model.Email
                };

                var result = await _user.CreateAsync(user, model.Password);
                if (result.Succeeded)
                {
                    // ✅ Automatically add role "User" after registration
                    await _user.AddToRoleAsync(user, "User");

                    // Log them in right after registration
                    await _sign.SignInAsync(user, isPersistent: false);

                    return RedirectToAction("Index", "Home");
                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }
            }
            return View(model);
        }
        [HttpGet]
        public IActionResult Login(string returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model, string returnUrl = null)
        {
            if (ModelState.IsValid)
            {
                var result = await _sign.PasswordSignInAsync(model.Email, model.Password, model.RememberMe, false);

                if (result.Succeeded)
                {
                    var user = await _user.FindByEmailAsync(model.Email);
                    if (await _user.IsInRoleAsync(user, "Admin"))
                    {
                        return RedirectToAction("Index", "Car"); // Admin goes to Car management
                    }
                    else
                    {
                        return RedirectToAction("Index", "Rental"); // User goes to Rental section
                    }
                }

                ModelState.AddModelError("", "Invalid login attempt");
            }

            return View(model);
        }

        // --- LOGOUT ---
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await _sign.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }
    }
}
