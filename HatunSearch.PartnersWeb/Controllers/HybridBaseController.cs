// Hatun Search | Layer: PartnersWeb || Version: 2018.11.16.810
// (c) 2018 Hatun Search. All rights reserved.

// 'Using' directive
using HatunSearch.PartnersWeb.Globalization;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web.Hosting;
using System.Web.Mvc;
using System.Web.Routing;

namespace HatunSearch.PartnersWeb.Controllers
{
	public abstract class HybridBaseController : JsonBasedController
	{
		public HybridBaseController(string name) => ControllerName = name;

		protected override void Initialize(RequestContext requestContext)
		{
			base.Initialize(requestContext);
			LocalizationProvider = GetLocalizationProvider("WebApp");
			LocalizationProvider.Join(GetLocalizationProvider("Entities"));
			LocalizationProvider.Join(GetLocalizationProvider(ControllerName));
		}
		protected override void OnActionExecuting(ActionExecutingContext filterContext)
		{
			base.OnActionExecuting(filterContext);
			ViewBag.CurrentLanguage = LocalizationProvider.CurrentLanguage;
			if (TempData.ContainsKey("Errors")) ViewBag.Errors = TempData["Errors"] as IDictionary<string, string>;
			ViewBag.LocalizationProvider = LocalizationProvider;
		}

		protected void AddError(string key, string value)
		{
			IDictionary<string, string> errors = ViewBag.Errors as IDictionary<string, string>;
			if (errors == null)
			{
				errors = new Dictionary<string, string>();
				ViewBag.Errors = errors;
			}
			errors.Add(key, value);
		}
		protected HttpStatusCodeResult BadRequest() => new HttpStatusCodeResult(HttpStatusCode.BadRequest);
		public ActionResult BadRequestWithErrors(object model = null)
		{
			IEnumerable<KeyValuePair<string, string>> errors = GetErrors();
			ViewBag.Errors = errors;
			return model != null ? View(model) : View();
		}
		protected IDictionary<string, string> GetErrors()
		{
			IDictionary<string, string> result = ViewBag.Errors as IDictionary<string, string> ?? new Dictionary<string, string>();
			foreach (KeyValuePair<string, ModelState> modelState in ModelState)
			{
				string key = modelState.Key;
				ModelErrorCollection errors = modelState.Value.Errors;
				if (errors.Count > 0) result.Add(key, errors.First().ErrorMessage);
			}
			return result;
		}
		protected LocalizationProvider GetLocalizationProvider(string name) => new LocalizationProvider(HostingEnvironment.MapPath($"~/App_Data/Localization/{name}.xml"));

		protected string ControllerName { get; private set; }
		protected LocalizationProvider LocalizationProvider { get; private set; }
	}
}