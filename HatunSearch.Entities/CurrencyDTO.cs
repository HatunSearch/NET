// Hatun Search | Layer: Entities || Version: 2018.11.16.810
// (c) 2018 Hatun Search. All rights reserved.

// 'Using' directive
using HatunSearch.Entities.Globalization;
using HatunSearch.Entities.Patterns;

namespace HatunSearch.Entities
{
	public sealed class CurrencyDTO : DTO<string>
	{
		public LocalizationDictionary DisplayName { get; set; }
		public string Symbol { get; set; }
	}
}