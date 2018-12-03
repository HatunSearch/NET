// Hatun Search | Layer: Data || Version: 2018.11.16.810
// (c) 2018 Hatun Search. All rights reserved.

// 'Using' directive
using HatunSearch.Data.Databases;
using HatunSearch.Data.Patterns;
using HatunSearch.Entities;
using HatunSearch.Entities.Globalization;
using System;
using System.Collections.Generic;

namespace HatunSearch.Data
{
	public sealed class PartnerInvoiceDetailRepository : Repository
	{
		private const string
			insertQuery = "INSERT INTO Payments.PartnerInvoiceDetail (Invoice, Property, PublishMode, Cost) VALUES (@Invoice, @Property, @PublishMode, @Cost)",
			selectByInvoiceQuery =
				@"SELECT InvoiceDetail.Invoice, InvoiceDetail.Property AS PropertyId, Property.[Name] AS PropertyName, InvoiceDetail.PublishMode, InvoiceDetail.Cost
				FROM Payments.PartnerInvoiceDetail AS InvoiceDetail JOIN Business.Property AS Property ON InvoiceDetail.Property = Property.Id WHERE InvoiceDetail.Invoice = @Invoice";

		public PartnerInvoiceDetailRepository() { }
		public PartnerInvoiceDetailRepository(Connector connector) : base(connector) { }

		public void Insert(PartnerInvoiceDetailDTO invoiceDetail)
		{
			Connector.ExecuteNonQuery(insertQuery, new Dictionary<string, object>()
			{
				{ "Invoice", invoiceDetail.Invoice },
				{ "Property", invoiceDetail.Property },
				{ "PublishMode", invoiceDetail.PublishMode },
				{ "Cost", invoiceDetail.Cost }
			});
		}
		public IEnumerable<PartnerInvoiceDetailDTO> SelectByInvoice(Guid invoiceId)
		{
			PublishModeRepository publishModeRepository = new PublishModeRepository(Connector);
			return Connector.ExecuteReader(selectByInvoiceQuery, new Dictionary<string, object>() { { "Invoice", invoiceId } }, reader =>
			{
				byte publishModeId = (byte)reader["PublishMode"];
				return new PartnerInvoiceDetailDTO()
				{
					Invoice = new PartnerInvoiceDTO() { Id = (Guid)reader["Invoice"] },
					Property = new PropertyDTO()
					{
						Id = (Guid)reader["PropertyId"],
						Name = reader["PropertyName"] as string
					},
					PublishMode = new PublishModeDTO()
					{
						Id = publishModeId,
						DisplayName = new LocalizationDictionary(publishModeRepository.GetDisplayName(publishModeId))
					},
					Cost = (decimal)reader["Cost"]
				};
			});
		}
	}
}