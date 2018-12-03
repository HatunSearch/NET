// Hatun Search | Layer: Data || Version: 2018.11.16.810
// (c) 2018 Hatun Search. All rights reserved.

// 'Using' directive
using HatunSearch.Data.Databases;
using HatunSearch.Data.Patterns;
using HatunSearch.Entities;
using System;
using System.Collections.Generic;
using System.Net;

namespace HatunSearch.Data
{
	public sealed class PartnerLoginAttemptRepository : Repository, IInsertableRepository<PartnerLoginAttemptDTO, Guid>
	{
		private const string insertQuery = "INSERT INTO [Security].PartnerLoginAttempt ([Partner], IPAddress) OUTPUT INSERTED.Id VALUES(@Partner, @IPAddress)",
			selectByPartnerAndTimeStampAsDateQuery = "SELECT Id, [Partner], IPAddress, TimeStamp FROM [Security].PartnerLoginAttempt WHERE Partner = @Partner AND TimeStamp > @TimeStamp";

		public PartnerLoginAttemptRepository() { }
		public PartnerLoginAttemptRepository(Connector connector) : base(connector) { }

		public void Insert(PartnerLoginAttemptDTO loginAttempt, out Guid id)
		{
			id = Connector.ExecuteScalar<Guid>(insertQuery, new Dictionary<string, object>()
			{
				{ "Partner", loginAttempt.Partner },
				{ "IPAddress", loginAttempt.IPAddress }
			});
		}
		public IEnumerable<PartnerLoginAttemptDTO> SelectByPartnerAndTimeStampAsDate(Guid partnerId, DateTime timeStamp)
		{
			return Connector.ExecuteReader(selectByPartnerAndTimeStampAsDateQuery, new Dictionary<string, object>()
			{
				{ "Partner", partnerId },
				{ "TimeStamp", timeStamp }
			}, reader => new PartnerLoginAttemptDTO()
			{
				Id = (Guid)reader["Id"],
				Partner = new PartnerDTO() { Id = (Guid)reader["Partner"] },
				IPAddress = IPAddress.Parse(reader["IPAddress"] as string),
				TimeStamp = (DateTime)reader["TimeStamp"]
			});
		}
	}
}