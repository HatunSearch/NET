// Hatun Search | Layer: Business || Version: 2018.11.16.810
// (c) 2018 Hatun Search. All rights reserved.

// 'Using' directive
using HatunSearch.Business.Patterns;
using HatunSearch.Data;
using HatunSearch.Data.Databases;
using HatunSearch.Entities;
using System;
using System.Collections.Generic;

namespace HatunSearch.Business
{
	public sealed class CustomerQuestionBLL : BLL<CustomerQuestionRepository>, IReadableBLL<CustomerQuestionDTO, Guid>
	{
		public CustomerQuestionBLL(Connector connector) : base(connector) { }

		public enum UpdateResult : byte
		{
			OK = 1,
			NotFound = 2
		}

		public CustomerQuestionDTO ReadById(Guid id) => Repository.SelectById(id);
		public IEnumerable<CustomerQuestionDTO> ReadByPartner(Guid partnerId) => Repository.SelectByPartner(partnerId);
		public UpdateResult Update(Guid id, IDictionary<string, object> fields) => Repository.Update(id, fields) == 1 ? UpdateResult.OK : UpdateResult.NotFound;
	}
}