// Hatun Search | Layer: Entities || Version: 2018.11.16.810
// (c) 2018 Hatun Search. All rights reserved.

// 'Using' directive
using HatunSearch.Entities.Patterns;
using System;
using System.Net;

namespace HatunSearch.Entities
{
	public sealed class PartnerLoginAttemptDTO : DTO<Guid>
	{
		public PartnerDTO Partner { get; set; }
		public IPAddress IPAddress { get; set; }
		public DateTime TimeStamp { get; set; }
	}
}