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
	public sealed class PublishModeBLL : BLL<PublishModeRepository>, IReadableBLL<PublishModeDTO>, IReadableBLL<PublishModeDTO, byte>
	{
		public PublishModeBLL(Connector connector) : base(connector) { }

		public IEnumerable<PublishModeDTO> ReadAll() => Repository.SelectAll();
		public PublishModeDTO ReadById(byte id) => Repository.SelectById(id);
	}
}