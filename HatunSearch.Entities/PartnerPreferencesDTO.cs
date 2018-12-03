// Hatun Search | Layer: Entities || Version: 2018.11.16.810
// (c) 2018 Hatun Search. All rights reserved.

// 'Using' directive
using System.ComponentModel.DataAnnotations;

namespace HatunSearch.Entities
{
	public sealed class PartnerPreferencesDTO
	{
		[Required(ErrorMessage = "PreferredCurrencyIsRequired")]
		public CurrencyDTO PreferredCurrency { get; set; }
		[Required(ErrorMessage = "PreferredLanguageIsRequired")]
		public LanguageDTO PreferredLanguage { get; set; }
	}
}