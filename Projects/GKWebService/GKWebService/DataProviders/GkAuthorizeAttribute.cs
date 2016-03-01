using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace GKWebService.DataProviders
{
	[AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, Inherited = true, AllowMultiple = true)]
	public class GkAuthorizeAttribute : AuthorizeAttribute
	{
		protected override bool AuthorizeCore(HttpContextBase httpContext)
		{
			var isAuthenticated = base.AuthorizeCore(httpContext);
			if (isAuthenticated)
			{
			//	string cookieName = FormsAuthentication.FormsCookieName;
			//	if (!httpContext.User.Identity.IsAuthenticated ||
			//		httpContext.Request.Cookies == null ||
			//		httpContext.Request.Cookies[cookieName] == null)
			//	{
			//		return false;
			//	}

			//	var authCookie = httpContext.Request.Cookies[cookieName];
			//	var authTicket = FormsAuthentication.Decrypt(authCookie.Value);

			//	string webServiceToken = authTicket.UserData;

			//	IPrincipal userPrincipal = ...create some custom implementation
			//								  and store the web service token as property
	
			//// Inject the custom principal in the HttpContext
			//	httpContext.User = userPrincipal;
			}
			return isAuthenticated;
		}
	}
}