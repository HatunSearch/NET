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
	public sealed class PartnerLoginAttemptBLL : BLL<PartnerLoginAttemptRepository>, ICreatableBLL<PartnerLoginAttemptDTO, PartnerLoginAttemptBLL.CreateResult>
	{
		public PartnerLoginAttemptBLL(Connector connector) : base(connector) { }

		public enum CreateResult : byte { OK = 1 }

		public CreateResult Create(PartnerLoginAttemptDTO loginAttempt)
		{
			Repository.Insert(loginAttempt, out Guid id);
			loginAttempt.Id = id;
			return CreateResult.OK;
		}
		public IEnumerable<PartnerLoginAttemptDTO> ReadByPartnerAndTimeStampAsDate(Guid partnerId, DateTime timeStamp) => Repository.SelectByPartnerAndTimeStampAsDate(partnerId, timeStamp);
	}
}