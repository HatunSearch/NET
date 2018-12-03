// Hatun Search | Layer: PartnersWeb || Version: 2018.11.16.810
// (c) 2018 Hatun Search. All rights reserved.

// 'Using' directive
using HatunSearch.Business;
using HatunSearch.Entities;
using HatunSearch.PartnersWeb.Globalization;
using System.Collections.Generic;
using System.Web.Mvc;
using System.Web.Routing;

namespace HatunSearch.PartnersWeb
{
	public static class RouteConfig
	{
		public static void RegisterRoutes(RouteCollection routes)
		{
			routes.IgnoreRoute("{resource}.axd/{*pathInfo}");
			CountryBLL countryBLL = new CountryBLL(WebApp.Connector);
			IEnumerable<CountryDTO> countries = countryBLL.ReadAll();
			routes.MapMvcAttributeRoutes(new LocalizableRouteProvider(countries));
		}
	}
}