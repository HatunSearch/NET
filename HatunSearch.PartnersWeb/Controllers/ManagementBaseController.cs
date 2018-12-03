// Hatun Search | Layer: PartnersWeb || Version: 2018.11.16.810
// (c) 2018 Hatun Search. All rights reserved.

// 'Using' directive
using HatunSearch.Business;
using HatunSearch.Entities;
using HatunSearch.PartnersWeb.Globalization;
using System.Web.Mvc;
using System.Web.Routing;

namespace HatunSearch.PartnersWeb.Controllers
{
	public abstract class ManagementBaseController : AuthenticationRequiredHybridBaseController
	{
		public ManagementBaseController(string name) : base(name) { }

		protected override void Initialize(RequestContext requestContext)
		{
			base.Initialize(requestContext);
			if (ControllerName != "Management")
			{
				LocalizationProvider managementLocalizationProvider = GetLocalizationProvider("Management");
				LocalizationProvider.Join(managementLocalizationProvider);
			}
		}
		protected override void OnActionExecuting(ActionExecutingContext filterContext)
		{
			base.OnActionExecuting(filterContext);
			ViewBag.ControllerName = ControllerName;
			ViewBag.ActionName = filterContext.ActionDescriptor.ActionName;
			if (filterContext.Result == null)
			{
				PartnerBLL partnerBLL = new PartnerBLL(WebApp.Connector);
				PartnerDTO partner = partnerBLL.ReadById(CurrentSession.Partner.Id);
				string result = TempData["Result"] as string;
				Account = partner;
				ViewBag.Account = partner;
				if (result != null) ViewBag.Result = result;
			}
		}

		protected PartnerDTO Account { get; set; }
	}
}