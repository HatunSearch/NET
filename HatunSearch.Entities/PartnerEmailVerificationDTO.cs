// Hatun Search | Layer: Entities || Version: 2018.11.16.810
// (c) 2018 Hatun Search. All rights reserved.

// 'Using' directive
using HatunSearch.Entities.Patterns;
using System;

namespace HatunSearch.Entities
{
	public sealed class PartnerEmailVerificationDTO : DTO<byte[]>
	{
		public PartnerDTO Partner { get; set; }
		public string EmailAddress { get; set; }
		public DateTime? ExpiresOn { get; set; }
		public bool IsActive { get; set; }
	}
}