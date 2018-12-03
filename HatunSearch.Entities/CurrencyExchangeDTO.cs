// Hatun Search | Layer: Entities || Version: 2018.11.16.810
// (c) 2018 Hatun Search. All rights reserved.

// 'Using' directive
using HatunSearch.Entities.Patterns;
using System;

namespace HatunSearch.Entities
{
	public sealed class CurrencyExchangeDTO : DTO<CurrencyDTO, CurrencyDTO>
	{
		public CurrencyDTO From
		{
			get => Id.FirstKey;
			set => Id.FirstKey = value;
		}
		public CurrencyDTO To
		{
			get => Id.SecondKey;
			set => Id.SecondKey = value;
		}
		public decimal Rate { get; set; }
		public DateTime UpdatedAt { get; set; }
	}
}