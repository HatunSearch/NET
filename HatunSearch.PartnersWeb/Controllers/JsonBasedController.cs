// Hatun Search | Layer: PartnersWeb || Version: 2018.11.16.810
// (c) 2018 Hatun Search. All rights reserved.

// 'Using' directive
using Newtonsoft.Json;
using System.Globalization;
using System.Net;
using System.Text;
using System.Threading;
using System.Web.Mvc;

namespace HatunSearch.PartnersWeb.Controllers
{
	public abstract class JsonBasedController : Controller
	{
		protected override void OnActionExecuting(ActionExecutingContext filterContext)
		{
			base.OnActionExecuting(filterContext);
			CultureInfo currentCulture = new CultureInfo(RouteData.Values["culture"] as string);
			Thread.CurrentThread.CurrentCulture = currentCulture;
		}

		protected ActionResult Json<T>(T content) => File(Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(content)), "application/json");
		protected ActionResult Json<T>(HttpStatusCode statusCode, T content) => File(Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(content)), "application/json");
	}
}