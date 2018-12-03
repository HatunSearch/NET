// Hatun Search | Layer: PartnersWeb || Version: 2018.11.16.810
// (c) 2018 Hatun Search. All rights reserved.

// 'Using' directive
using System;
using System.Web;

namespace HatunSearch.PartnersWeb.Http
{
	public sealed class CORSEnabledStaticFileModule : IHttpModule
	{
		public string ModuleName => "CORSEnabledStaticFileModule";

		private void Application_BeginRequest(object source, EventArgs eventArgs)
		{
			HttpApplication application = source as HttpApplication;
			HttpContext context = application.Context;
			string filePath = context.Request.FilePath;
			string fileExtension = VirtualPathUtility.GetExtension(filePath);
			if (fileExtension == ".ttf") context.Response.Headers.Add("Access-Control-Allow-Origin", "*");
		}

		public void Dispose() { }
		public void Init(HttpApplication context) => context.BeginRequest += Application_BeginRequest;
	}
}