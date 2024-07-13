using EpicockX.Models;
using EpicockX.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EpicockX.Controllers
{
    public class AuthController : Controller
    {
        private readonly UserService _userSvc;
        private readonly AuthService _authSvc;

        public AuthController(UserService service, AuthService authService)
        {
            _userSvc = service;
            _authSvc = authService;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Index([Bind("Username,Password")] LoginDto model)
        {
            if (!ModelState.IsValid)
            {
                TempData["error"] = "Errore nei dati inseriti";
                return View();
            }
            // cerca l'utente nel database con username e password inseriti
            // se non esiste, ritorna alla pagina di login con un messaggio di errore
            var user = _userSvc.GetUser(model);

            if (user == null)
            {
                TempData["error"] = "Account non esistente";
                return View();
            }
            _authSvc.Login(user);

            TempData["success"] = "Login effettuato con successo";

            return RedirectToAction("Index", "Home");
        }

        [Authorize]
        public async Task<IActionResult> Logout()
        {
            _authSvc.Logout();

            TempData["success"] = "Sei stato disconnesso";

            return RedirectToAction("Index", "Home");
        }
    }
}
