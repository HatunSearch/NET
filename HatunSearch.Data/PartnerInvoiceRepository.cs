// Hatun Search | Layer: Data || Version: 2018.11.16.810
// (c) 2018 Hatun Search. All rights reserved.

// 'Using' directive
using HatunSearch.Data.Databases;
using HatunSearch.Data.Patterns;
using HatunSearch.Entities;
using HatunSearch.Entities.Globalization;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace HatunSearch.Data
{
	public sealed class PartnerInvoiceRepository : Repository, IInsertableRepository<PartnerInvoiceDTO, Guid?>, ISelectableRepository<PartnerInvoiceDTO, Guid>
	{
		private const string
			insertQuery = "INSERT INTO Payments.PartnerInvoice ([Partner], Currency, StripeId) OUTPUT inserted.Id VALUES(@Partner, @Currency, @StripeId)",
			selectByIdQuery =
				@"SELECT PartnerInvoice.Id, PartnerInvoice.[Partner], PartnerInvoice.[TimeStamp], PartnerInvoice.Currency AS CurrencyId, Currency.Symbol AS CurrencySymbol, PartnerInvoice.StripeId
				FROM Payments.PartnerInvoice AS PartnerInvoice
				JOIN [Geography].Currency AS Currency ON PartnerInvoice.Currency = Currency.Id
				WHERE PartnerInvoice.Id = @Id",
			selectByPartnerQuery =
				@"SELECT PartnerInvoice.Id, PartnerInvoice.[Partner], PartnerInvoice.[TimeStamp], PartnerInvoice.Currency AS CurrencyId, Currency.Symbol AS CurrencySymbol, PartnerInvoice.StripeId
				FROM Payments.PartnerInvoice AS PartnerInvoice
				JOIN [Geography].Currency AS Currency ON PartnerInvoice.Currency = Currency.Id
				WHERE PartnerInvoice.[Partner] = @Partner";

		public PartnerInvoiceRepository() { }
		public PartnerInvoiceRepository(Connector connector) : base(connector) { }

		public void Insert(PartnerInvoiceDTO invoice, out Guid? id)
		{
			id = Connector.ExecuteScalar<Guid?>(insertQuery, new Dictionary<string, object>()
			{
				{ "Partner", invoice.Partner },
				{ "Currency", invoice.Currency },
				{ "StripeId", invoice.StripeId }
			});
		}
		private PartnerInvoiceDTO ReadFromDataReader(IDataReader reader) => ReadFromDataReader(reader, new CurrencyRepository(Connector), new PartnerInvoiceDetailRepository(Connector));
		private PartnerInvoiceDTO ReadFromDataReader(IDataReader reader, CurrencyRepository currencyRepository, PartnerInvoiceDetailRepository invoiceDetailRepository)
		{
			string currencyId = reader["CurrencyId"] as string;
			Guid invoiceId = (Guid)reader["Id"];
			return new PartnerInvoiceDTO()
			{
				Id = invoiceId,
				Partner = new PartnerDTO() { Id = (Guid)reader["Partner"] },
				TimeStamp = (DateTime)reader["TimeStamp"],
				Currency = new CurrencyDTO()
				{
					Id = currencyId,
					DisplayName = new LocalizationDictionary(currencyRepository.GetDisplayName(currencyId)),
					Symbol = reader["CurrencySymbol"] as string
				},
				StripeId = reader["StripeId"] as string,
				Details = invoiceDetailRepository.SelectByInvoice(invoiceId)
			};
		}
		public PartnerInvoiceDTO SelectById(Guid id) => Connector.ExecuteReader(selectByIdQuery, new Dictionary<string, object>() { { "Id", id } }, ReadFromDataReader).FirstOrDefault();
		public IEnumerable<PartnerInvoiceDTO> SelectByPartner(Guid partnerId)
		{
			CurrencyRepository currencyRepository = new CurrencyRepository(Connector);
			PartnerInvoiceDetailRepository invoiceDetailRepository = new PartnerInvoiceDetailRepository(Connector);
			return Connector.ExecuteReader(selectByPartnerQuery, new Dictionary<string, object>() { { "Partner", partnerId } }, reader => ReadFromDataReader(reader, currencyRepository, invoiceDetailRepository)); ;
		}
	}
}