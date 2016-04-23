using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Threading.Tasks;
using GkWeb.ViewModels;
using Microsoft.AspNet.Authorization;
using Microsoft.AspNet.Http.Authentication;
using Microsoft.AspNet.Mvc;
using Microsoft.AspNet.Routing;

// For more information on enabling Web API for empty projects, visit http://go.microsoft.com/fwlink/?LinkID=397860

namespace GkWeb.Controllers
{
    public class AuthController : Controller
    {
		[HttpGet]
		[AllowAnonymous]
		[Route("api/Auth/Login")]
		public async Task<bool> Login(SignInViewModel model) {
			//if (model == null)
			//	return false;
			//if (model.User != "Admin" && model.Password != "1") {
			//	return false;
			//}
			var authProperties = new AuthenticationProperties();
			var identity = new ClaimsIdentity("Automatic");
			identity.AddClaim(new Claim(ClaimTypes.NameIdentifier, "1"));
			identity.AddClaim(new Claim(ClaimTypes.Name, "Admin1"));
			var principal = new ClaimsPrincipal(identity);
			await HttpContext.Authentication.SignInAsync(
						"Automatic",
						principal,
						authProperties);
			return true;
		}

		[HttpGet]
		[Authorize]
		[Route("api/Auth/LoginCheck")]
		public string LoginCheck() {
			return "you are logged in" + HttpContext.User.Identity.Name;
		}

	}
}
