// Hatun Search | Layer: PartnersWeb || Version: 2018.11.16.810
// (c) 2018 Hatun Search. All rights reserved.

// 'Using' directive
using HatunSearch.Business;
using HatunSearch.Data.Helpers;
using HatunSearch.Entities;
using System;
using System.Collections.Generic;
using System.Web;
using System.Web.Mvc;

namespace HatunSearch.PartnersWeb.Controllers
{
	public abstract class AuthenticationRequiredHybridBaseController : HybridBaseController
	{
		public AuthenticationRequiredHybridBaseController(string name) : base(name) { }

		protected override void OnActionExecuting(ActionExecutingContext filterContext)
		{
			base.OnActionExecuting(filterContext);
			HttpCookie cookie = Request.Cookies["Session"];
			if (cookie != null)
			{
				string cookieValue = cookie.Value;
				if (!string.IsNullOrEmpty(cookieValue))
				{
					try
					{
						byte[] sessionId = FormatHelper.FromHexStringToArray(cookieValue);
						PartnerSessionBLL sessionBLL = new PartnerSessionBLL(WebApp.Connector);
						PartnerSessionDTO session = sessionBLL.ReadById(sessionId);
						DateTime utcNow = DateTime.UtcNow;
						if (session?.ExpiresOn > utcNow && session.IsActive)
						{
							if (session.Partner.HasEmailAddressBeenVerified)
							{
								sessionBLL.UpdateExpiration(sessionId, utcNow.AddMinutes(15));
								CurrentSession = session;
							}
							else ReturnToLogin(filterContext, "EmailAddressHasNotBeenVerified");
						}
						else ReturnToLogin(filterContext, "YourSessionHasExpired");
					}
					catch { ReturnToLogin(filterContext, "YouShouldLogInFirst"); }
				}
				else ReturnToLogin(filterContext, "YouShouldLogInFirst");
			}
			else ReturnToLogin(filterContext, "YouShouldLogInFirst");
		}

		private void ReturnToLogin(ActionExecutingContext filterContext, string error)
		{
			TempData["Errors"] = new Dictionary<string, string>() { { "Username", error } };
			filterContext.Result = RedirectToAction("Login", "Accounts");
		}

		protected PartnerSessionDTO CurrentSession { get; set; }
	}
}