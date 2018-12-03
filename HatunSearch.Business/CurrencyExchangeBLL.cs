// Hatun Search | Layer: Business || Version: 2018.11.16.810
// (c) 2018 Hatun Search. All rights reserved.

// 'Using' directive
using HatunSearch.Business.Patterns;
using HatunSearch.Data;
using HatunSearch.Data.Databases;
using HatunSearch.Entities;
using System.Collections.Generic;

namespace HatunSearch.Business
{
	public sealed class CurrencyExchangeBLL : BLL<CurrencyExchangeRepository>, IReadableBLL<CurrencyExchangeDTO, CurrencyDTO, CurrencyDTO>
	{
		public CurrencyExchangeBLL(Connector connector) : base(connector) { }

		public CurrencyExchangeDTO ReadById(CurrencyDTO from, CurrencyDTO to) => Repository.SelectById(from, to);
		public CurrencyExchangeDTO ReadById(string from, string to) => Repository.SelectById(from, to);
		public IEnumerable<CurrencyExchangeDTO> ReadByTo(string to) => Repository.SelectByTo(to);
	}
}