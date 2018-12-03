// Hatun Search | Layer: Business || Version: 2018.11.16.810
// (c) 2018 Hatun Search. All rights reserved.

// 'Using' directive
using HatunSearch.Business.Patterns;
using HatunSearch.Data;
using HatunSearch.Data.Databases;
using HatunSearch.Entities;
using Stripe;
using System;
using System.Collections.Generic;
using System.Linq;

namespace HatunSearch.Business
{
	public sealed class PartnerInvoiceBLL : BLL<PartnerInvoiceRepository>, ICreatableBLL<PartnerInvoiceDTO, PartnerInvoiceBLL.CreateResult>, IReadableBLL<PartnerInvoiceDTO, Guid>
	{
		public PartnerInvoiceBLL(Connector connector) : base(connector) { }

		public enum CreateResult : byte { OK = 1 }

		public CreateResult Create(PartnerInvoiceDTO invoice)
		{
			bool wasATransaction = Connector.IsTransaction;
			if (!wasATransaction) Connector.IsTransaction = true;
			Repository.Insert(invoice, out Guid? id);
			if (id != null) invoice.Id = id.Value;
			PartnerInvoiceDetailBLL invoiceDetailBLL = new PartnerInvoiceDetailBLL(Connector);
			foreach (PartnerInvoiceDetailDTO detail in invoice.Details)
			{
				detail.Invoice = invoice;
				invoiceDetailBLL.Create(detail);
			}
			if (!wasATransaction) Connector.CommitTransaction();
			return CreateResult.OK;
		}
		private Charge GetStripeCharge(PartnerInvoiceDTO invoice) => GetStripeCharge(invoice, new ChargeService());
		internal Charge GetStripeCharge(PartnerInvoiceDTO invoice, ChargeService chargeService) => chargeService.Get(invoice.StripeId);
		public PartnerInvoiceDTO ReadById(Guid id)
		{
			PartnerInvoiceDTO invoice = Repository.SelectById(id);
			if (invoice != null) invoice.StripeCharge = GetStripeCharge(invoice);
			return invoice;
		}
		public IEnumerable<PartnerInvoiceDTO> ReadByPartner(Guid partnerId)
		{
			ChargeService chargeService = new ChargeService();
			IEnumerable<PartnerInvoiceDTO> invoices = Repository.SelectByPartner(partnerId);
			return invoices.Select(i =>
			{
				i.StripeCharge = GetStripeCharge(i, chargeService);
				return i;
			});
		}
	}
}