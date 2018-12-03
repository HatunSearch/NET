// Hatun Search | Layer: Entities || Version: 2018.11.16.810
// (c) 2018 Hatun Search. All rights reserved.

// 'Using' directive
using HatunSearch.Entities.Globalization;
using HatunSearch.Entities.Patterns;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace HatunSearch.Entities
{
	public sealed class CountryDTO : DTO<string>
	{
		public LocalizationDictionary DisplayName { get; set; }
		public CurrencyDTO PredominantCurrency { get; set; }
		public LanguageDTO PredominantLanguage { get; set; }

		[JsonIgnore]
		public IEnumerable<RegionDTO> Regions { get; set; }
		[JsonIgnore]
		public IEnumerable<CurrencyDTO> SupportedCurrencies { get; set; }
		[JsonIgnore]
		public IEnumerable<LanguageDTO> SupportedLanguages { get; set; }
	}
}