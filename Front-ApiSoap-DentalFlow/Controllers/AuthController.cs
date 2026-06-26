using Front_ApiSoap_DentalFlow.ViewModels;
using Microsoft.AspNetCore.Mvc;
using servicioAuth;

namespace Front_ApiSoap_DentalFlow.Controllers
{
    public class AuthController : Controller
    {
        private readonly AuthEndpoint _authEndpoint;

        public AuthController(AuthEndpoint authEndpoint)
        {
            _authEndpoint = authEndpoint;
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View(new LoginViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model)
            {
            try
            {
                if (!ModelState.IsValid)
                    return View(model);

                var request = new loginRequest
                {
                    username = model.Codigo,
                    password = model.Clave
                };

                var response = await _authEndpoint.loginAsync(request);

                var jwt = response.@return;

                if (string.IsNullOrWhiteSpace(jwt))
                {
                    ModelState.AddModelError("", "Credenciales inválidas");
                    return View(model);
                }

                Response.Cookies.Append("jwt_token", jwt, new CookieOptions
                {
                    HttpOnly = true,
                    Secure = false,
                    SameSite = SameSiteMode.Strict,
                    Expires = DateTimeOffset.UtcNow.AddHours(8)
                });

                return RedirectToAction("Index", "Dashboard");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"Error de autenticación: {ex.Message}");
                return View(model);
            }
        }

        [HttpPost]
        public IActionResult Logout()
        {
            Response.Cookies.Delete("jwt_token");
            return RedirectToAction("Login");
        }
    }
}