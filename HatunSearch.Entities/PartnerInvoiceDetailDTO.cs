// Hatun Search | Layer: Entities || Version: 2018.11.16.810
// (c) 2018 Hatun Search. All rights reserved.

// 'Using' directive
using HatunSearch.Entities.Patterns;

namespace HatunSearch.Entities
{
	public sealed class PartnerInvoiceDetailDTO : DTO<PartnerInvoiceDTO, PropertyDTO>
	{
		public PartnerInvoiceDTO Invoice
		{
			get => Id.FirstKey;
			set => Id.FirstKey = value;
		}
		public PropertyDTO Property
		{
			get => Id.SecondKey;
			set => Id.SecondKey = value;
		}
		public PublishModeDTO PublishMode { get; set; }
		public decimal Cost { get; set; }
	}
}