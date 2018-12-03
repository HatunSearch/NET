// Hatun Search | Layer: Entities || Version: 2018.11.16.810
// (c) 2018 Hatun Search. All rights reserved.

// 'Using' directive
using HatunSearch.Entities.Patterns;
using Stripe;
using System;

namespace HatunSearch.Entities
{
	public sealed class PartnerCardDTO : DTO<Guid>
	{
		public PartnerDTO Partner { get; set; }
		public string StripeId { get; set; }

		public Card StripeCard { get; set; }

		public string Brand => StripeCard.Brand;
		public string Expiration => $"{StripeCard.ExpMonth}/{StripeCard.ExpYear}";
		public string Last4 => StripeCard.Last4;
	}
}