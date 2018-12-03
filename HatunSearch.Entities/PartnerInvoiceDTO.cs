// Hatun Search | Layer: Entities || Version: 2018.11.16.810
// (c) 2018 Hatun Search. All rights reserved.

// 'Using' directive
using HatunSearch.Entities.Patterns;
using Stripe;
using System;
using System.Collections.Generic;
using System.Linq;

namespace HatunSearch.Entities
{
	public sealed class PartnerInvoiceDTO : DTO<Guid>
	{
		private Charge charge = null;

		public PartnerDTO Partner { get; set; }
		public DateTime TimeStamp { get; set; }
		public CurrencyDTO Currency { get; set; }
		public string StripeId { get; set; }

		public IEnumerable<PartnerInvoiceDetailDTO> Details { get; set; }

		public Charge StripeCharge
		{
			get => charge;
			set
			{
				charge = value;
				if (value != null)
				{
					Card card = value.Source as Card;
					Card = new PartnerCardDTO() { StripeCard = card };
				}
			}
		}
		public decimal TotalCost => Details.Sum(i => i.Cost);

		public PartnerCardDTO Card { get; private set; }
	}
}