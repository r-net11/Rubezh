using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using GkWeb.Services;
using GkWeb.ViewModels;
using Microsoft.AspNet.Authorization;
using Microsoft.AspNet.Http.Authentication;
using Microsoft.AspNet.Mvc;
using Microsoft.Extensions.Logging;
//using RubezhClient;

// For more information on enabling MVC for empty projects, visit http://go.microsoft.com/fwlink/?LinkID=397860

namespace GkWeb.Controllers
{
	[Authorize]
	public class LogonController : Controller
	{
		private readonly ILogger _logger;
		private readonly ClientManager _clientManager;

		public LogonController(ILoggerFactory loggerFactory, ClientManager clientManager) {
			_logger = loggerFactory.CreateLogger<LogonController>();
			_clientManager = clientManager;

		}

		[HttpGet]
		[AllowAnonymous]
		public IActionResult Login(string returnUrl = null) {
			ViewData["ReturnUrl"] = returnUrl;
			return View();
		}

		[HttpPost]
		[AllowAnonymous]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Login(SignInViewModel model, string returnUrl = null) {
			if (ModelState.IsValid) {
				ViewData["ReturnUrl"] = returnUrl;

				if (!_clientManager.CheckPass(model.User, model.Password ?? string.Empty)) {
					return View(model);
				}

				var authProperties = new AuthenticationProperties();
				var identity = new ClaimsIdentity("Automatic");
				identity.AddClaim(new Claim(ClaimTypes.NameIdentifier, "1"));
				identity.AddClaim(new Claim(ClaimTypes.Name, model.User));
				var principal = new ClaimsPrincipal(identity);
				await HttpContext.Authentication.SignInAsync(
					"Automatic",
					principal,
					authProperties);

				_logger.LogInformation(1, "User logged in.");
				return RedirectToLocal(returnUrl);
			}
			return View(model);
		}

		private IActionResult RedirectToLocal(string returnUrl) {
			if (Url.IsLocalUrl(returnUrl)) {
				return Redirect(returnUrl);
			}
			else {
				return RedirectToAction(nameof(HomeController.Index), "Home");
			}
		}

				public async Task<IActionResult> Logout() {
			await HttpContext.Authentication.SignOutAsync("Automatic");
			return RedirectToAction("Login");
		}
	}
}
