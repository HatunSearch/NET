// Hatun Search | Layer: PartnersWeb || Version: 2018.11.16.810
// (c) 2018 Hatun Search. All rights reserved.

// 'Using' directive
using System;
using System.Text;
using System.Web.Mvc;

namespace HatunSearch.PartnersWeb.Controllers
{
	public sealed class ErrorsController : HybridBaseController
	{
		public ErrorsController() : base("Errors") { }

		[AcceptVerbs(HttpVerbs.Get | HttpVerbs.Post)]
		public ActionResult Error500()
		{
			if (ViewBag.Exception is Exception exception)
			{
				ViewBag.ExceptionMessage = PrettyPrintException(exception);
				return View();
			}
			else return RedirectToAction("Login", "Accounts");
		}

		private string PrettyPrintException(Exception exception)
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append(exception.Message);
			string stackTrace = exception.StackTrace;
			string[] stackTraceLines = stackTrace.Split(new string[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries);
			foreach (string stackTraceLine in stackTraceLines)
				stringBuilder.Append($"<br>&emsp;{stackTraceLine.Trim()}");
			return stringBuilder.ToString();
		}
	}
}