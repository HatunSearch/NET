// Hatun Search | Layer: PartnersWeb || Version: 2018.11.16.810
// (c) 2018 Hatun Search. All rights reserved.

// 'Using' directive
using HatunSearch.Entities;
using System.Collections.Generic;
using System.Text;
using System.Web.Mvc;
using System.Web.Mvc.Routing;

namespace HatunSearch.PartnersWeb.Globalization
{
	public sealed class LocalizableRouteProvider : DefaultDirectRouteProvider
	{
		private readonly string prefix = null;

		public LocalizableRouteProvider(IEnumerable<CountryDTO> countries) => prefix = CreatePrefix(countries);

		private string CreatePrefix(IEnumerable<CountryDTO> countries)
		{
			ICollection<string> supportedCultures = new List<string>();
			foreach (CountryDTO country in countries)
			{
				string countryId = country.Id.ToLower();
				foreach (LanguageDTO supportedLanguage in country.SupportedLanguages)
					supportedCultures.Add($"{supportedLanguage.Id.ToLower()}-{countryId}");
			}
			StringBuilder stringBuilder = new StringBuilder();
			bool hasACultureBeenAddedBefore = false;
			foreach (string supportedCulture in supportedCultures)
			{
				if (hasACultureBeenAddedBefore) stringBuilder.Append('|');
				stringBuilder.Append(supportedCulture);
				hasACultureBeenAddedBefore = true;
			}
			return $"{{culture:regex({stringBuilder.ToString()})}}";
		}
		protected override string GetRoutePrefix(ControllerDescriptor controllerDescriptor)
		{
			string routePrefix = base.GetRoutePrefix(controllerDescriptor);
			return $"{prefix}/{routePrefix}";
		}
	}
}