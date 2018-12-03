// Hatun Search | Layer: PartnersWeb || Version: 2018.11.16.810
// (c) 2018 Hatun Search. All rights reserved.

// 'Using' directive
using HatunSearch.Entities;
using HatunSearch.PartnersWeb.Controllers;
using HatunSearch.PartnersWeb.Http.ModelBinding;
using System;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace HatunSearch.PartnersWeb
{
	public abstract class MvcApplication : HttpApplication
	{
		protected void Application_Error(object sender, EventArgs eventArgs)
		{
			Exception exception = Server.GetLastError();
			Server.ClearError();
			Response.TrySkipIisCustomErrors = true;
			Response.Clear();
			ErrorsController errorsController = new ErrorsController();
			errorsController.ViewBag.Exception = exception;
			RouteData routeData = new RouteData();
			routeData.Values["Culture"] = HttpContext.Current.Request.RequestContext.RouteData.Values["culture"];
			routeData.Values["Controller"] = "Errors";
			routeData.Values["Action"] = "Error500";
			(errorsController as IController).Execute(new RequestContext(new HttpContextWrapper(Context), routeData));
		}
		protected void Application_Start()
		{
			AreaRegistration.RegisterAllAreas();
			RouteConfig.RegisterRoutes(RouteTable.Routes);
			RegisterModelBinders();
		}
		protected void Session_End(object sender, EventArgs eventArgs)
		{
			HttpContext currentContext = HttpContext.Current;
			currentContext?.Session?.Clear();
			currentContext?.Response.SetCookie(new HttpCookie("ASP.NET_SessionId") { Expires = DateTime.UtcNow });
		}

		private void RegisterModelBinders()
		{
			ModelBinderDictionary binders = ModelBinders.Binders;
			DTOModelBinder modelBinder = new DTOModelBinder();
			binders.Add(typeof(PartnerCompanyInfoDTO), modelBinder);
			binders.Add(typeof(PartnerCredentialDTO), modelBinder);
			binders.Add(typeof(PartnerPersonalInfoDTO), modelBinder);
			binders.Add(typeof(PartnerPreferencesDTO), modelBinder);
			binders.Add(typeof(PropertyDTO), modelBinder);
			binders.Add(typeof(PropertyFeatureDetailDTO), modelBinder);
			binders.Add(typeof(PropertyPictureDTO), modelBinder);
			binders.Add(typeof(ManagementSalesController.PublishPropertyDTO), modelBinder);
		}
	}
}