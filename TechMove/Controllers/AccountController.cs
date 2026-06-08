using Microsoft.AspNetCore.Mvc;
using TechMove.ApiClients;
using TechMove.ApiDtos;

namespace TechMove.Controllers
{
    public class AccountController : Controller
    {
        private readonly IFirebaseAuthClient _firebaseAuthClient;

        public AccountController(IFirebaseAuthClient firebaseAuthClient)
        {
            _firebaseAuthClient = firebaseAuthClient;
        }

        [HttpGet]
        public IActionResult Login()
        {
            if (!string.IsNullOrWhiteSpace(HttpContext.Session.GetString("Username")))
            {
                return RedirectToAction("Index", "Home");
            }

            return View(new LoginViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var firebaseUser = await _firebaseAuthClient.LoginAsync(model.Email, model.Password);

            if (firebaseUser == null || string.IsNullOrWhiteSpace(firebaseUser.IdToken))
            {
                ModelState.AddModelError(string.Empty, "Invalid email or password.");
                return View(model);
            }

            var role = GetRoleFromEmail(firebaseUser.Email);

            if (role == null)
            {
                ModelState.AddModelError(string.Empty, "This Firebase user is not authorised to access TechMove.");
                return View(model);
            }

            var displayName = role == "Admin" ? "Admin" : "User";

            HttpContext.Session.SetString("Username", displayName);
            HttpContext.Session.SetString("Role", role);
            HttpContext.Session.SetString("FirebaseEmail", firebaseUser.Email);
            HttpContext.Session.SetString("FirebaseIdToken", firebaseUser.IdToken);

            return RedirectToAction("Index", "Home");
        }

        public IActionResult Logout()
        {
            HttpContext.Session.Clear();

            return RedirectToAction(nameof(Login));
        }

        private static string? GetRoleFromEmail(string email)
        {
            var normalisedEmail = email.Trim().ToLowerInvariant();

            return normalisedEmail switch
            {
                "admin@gmail.com" => "Admin",
                "user@gmail.com" => "User",
                _ => null
            };
        }
    }
}