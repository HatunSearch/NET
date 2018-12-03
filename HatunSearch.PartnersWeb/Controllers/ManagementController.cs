// Hatun Search | Layer: PartnersWeb || Version: 2018.11.16.810
// (c) 2018 Hatun Search. All rights reserved.

// 'Using' directive
using System.Web.Mvc;

namespace HatunSearch.PartnersWeb.Controllers
{
	[RoutePrefix("management")]
	public sealed class ManagementController : ManagementBaseController
	{
		public ManagementController() : base("Management") { }

		[HttpGet]
		[Route("home")]
		public ActionResult Home() => View();
	}
}