// Hatun Search | Layer: Business || Version: 2018.11.16.810
// (c) 2018 Hatun Search. All rights reserved.

// 'Using' directive
using HatunSearch.Business.Patterns;
using HatunSearch.Data;
using HatunSearch.Data.Databases;
using HatunSearch.Entities;

namespace HatunSearch.Business
{
	public sealed class PartnerInvoiceDetailBLL : BLL<PartnerInvoiceDetailRepository>, ICreatableBLL<PartnerInvoiceDetailDTO, PartnerInvoiceDetailBLL.CreateResult>
	{
		public PartnerInvoiceDetailBLL(Connector connector) : base(connector) { }

		public enum CreateResult : byte { OK = 1 }

		public CreateResult Create(PartnerInvoiceDetailDTO invoiceDetail)
		{
			Repository.Insert(invoiceDetail);
			return CreateResult.OK;
		}
	}
}