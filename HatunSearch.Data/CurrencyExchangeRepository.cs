// Hatun Search | Layer: Data || Version: 2018.11.16.810
// (c) 2018 Hatun Search. All rights reserved.

// 'Using' directive
using HatunSearch.Data.Databases;
using HatunSearch.Data.Patterns;
using HatunSearch.Entities;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace HatunSearch.Data
{
	public sealed class CurrencyExchangeRepository : Repository, ISelectableRepository<CurrencyExchangeDTO, CurrencyDTO, CurrencyDTO>
	{
		private const string selectByToQuery = "SELECT [From], [To], Rate, UpdatedAt FROM Business.CurrencyExchange WHERE [To] = @To";

		public CurrencyExchangeRepository() { }
		public CurrencyExchangeRepository(Connector connector) : base(connector) { }

		private CurrencyExchangeDTO ReadFromDataReader(IDataReader reader)
		{
			return new CurrencyExchangeDTO()
			{
				From = new CurrencyDTO() { Id = reader["From"] as string },
				To = new CurrencyDTO() { Id = reader["To"] as string },
				Rate = (decimal)reader["Rate"],
				UpdatedAt = (DateTime)reader["UpdatedAt"]
			};
		}
		public CurrencyExchangeDTO SelectById(CurrencyDTO from, CurrencyDTO to) => SelectById(from.Id, to.Id);
		public CurrencyExchangeDTO SelectById(string from, string to) =>
			Connector.ExecuteReader(selectByToQuery, new Dictionary<string, object>()
			{
				{ "From", from },
				{ "To", to }
			}, ReadFromDataReader).FirstOrDefault();
		public IEnumerable<CurrencyExchangeDTO> SelectByTo(string to) => Connector.ExecuteReader(selectByToQuery, new Dictionary<string, object>() { { "To", to } }, ReadFromDataReader);
	}
}