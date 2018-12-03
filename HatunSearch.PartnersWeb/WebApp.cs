// Hatun Search | Layer: PartnersWeb || Version: 2018.11.16.810
// (c) 2018 Hatun Search. All rights reserved.

// 'Using' directive
using HatunSearch.Business.Helpers;
using HatunSearch.Data.Databases;
using Stripe;
using System.Collections.Specialized;
using System.Configuration;
using System.Net.Mail;

namespace HatunSearch.PartnersWeb
{
	public static class WebApp
	{
		private readonly static string connectionString = null;

		static WebApp()
		{
			connectionString = ConfigurationManager.ConnectionStrings["SQLServer"].ConnectionString;
			NameValueCollection appSettings = ConfigurationManager.AppSettings;
			EmailSender.DefaultAddress = new MailAddress("noreply@hatunsearch.me", "Hatun Search");
			LanguageCategory = appSettings["LanguageCategory"];
			PrimaryColor = appSettings["PrimaryColor"];
			SecondaryColor = appSettings["SecondaryColor"];
			StripePublishableKey = appSettings["StripePublishableKey"];
			StripeConfiguration.SetApiKey(appSettings["StripeSecretKey"]);
			Theme = appSettings["Theme"];
		}

		public static Connector Connector => new SQLServerConnector(connectionString);
		public static string LanguageCategory { get; private set; }
		public static string PrimaryColor { get; private set; }
		public static string SecondaryColor { get; private set; }
		public static string StripePublishableKey { get; private set; }
		public static string Theme { get; private set; }
	}
}