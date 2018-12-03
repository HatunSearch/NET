// Hatun Search | Layer: Data || Version: 2018.11.16.810
// (c) 2018 Hatun Search. All rights reserved.

// 'Using' directive
using HatunSearch.Data.Databases;
using HatunSearch.Data.Patterns;
using HatunSearch.Data.Security;
using HatunSearch.Entities;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net;

namespace HatunSearch.Data
{
	public sealed class PartnerSessionRepository : Repository, IInsertableRepository<PartnerSessionDTO, byte[]>, ISelectableRepository<PartnerSessionDTO, byte[]>, IUpdatableRepository<byte[]>
	{
		private const string
			insertQuery = "INSERT INTO [Security].PartnerSession (Id, [Partner], IPAddress, LoggedAt, ExpiresOn) VALUES(@Id, @Partner, @IPAddress, @LoggedAt, @ExpiresOn)",
			selectByIdQuery =
				@"SELECT PartnerSession.Id, PartnerSession.[Partner] AS PartnerId, [Partner].HasEmailAddressBeenVerified AS PartnerHasEmailAddressBeenVerified, PartnerSession.IPAddress,
				PartnerSession.LoggedAt, PartnerSession.ExpiresOn, PartnerSession.IsActive
				FROM [Security].PartnerSession AS PartnerSession
				JOIN Business.[Partner] AS [Partner] ON PartnerSession.[Partner] = [Partner].Id
				WHERE PartnerSession.Id = @Id",
			selectLastByPartnerQuery =
				@"SELECT TOP 1 PartnerSession.Id, PartnerSession.[Partner] AS PartnerId, [Partner].HasEmailAddressBeenVerified AS PartnerHasEmailAddressBeenVerified, PartnerSession.IPAddress,
				PartnerSession.LoggedAt, PartnerSession.ExpiresOn, PartnerSession.IsActive
				FROM [Security].PartnerSession AS PartnerSession
				JOIN Business.[Partner] AS [Partner] ON PartnerSession.[Partner] = [Partner].Id
				WHERE PartnerSession.[Partner] = @Partner ORDER BY PartnerSession.LoggedAt DESC";

		public PartnerSessionRepository() { }
		public PartnerSessionRepository(Connector connector) : base(connector) { }

		public void Insert(PartnerSessionDTO session, out byte[] id)
		{
			byte[] idResult = SecureIdGenerator.Generate(session);
			Connector.ExecuteNonQuery(insertQuery, new Dictionary<string, object>()
			{
				{ "Id", idResult },
				{ "Partner", session.Partner },
				{ "IPAddress", session.IPAddress },
				{ "LoggedAt", session.LoggedAt },
				{ "ExpiresOn", session.ExpiresOn }
			});
			id = idResult;
		}
		private PartnerSessionDTO ReadFromDataReader(IDataReader reader)
		{
			return new PartnerSessionDTO()
			{
				Id = (byte[])reader["Id"],
				Partner = new PartnerDTO()
				{
					Id = (Guid)reader["PartnerId"],
					HasEmailAddressBeenVerified = (bool)reader["PartnerHasEmailAddressBeenVerified"]
				},
				IPAddress = IPAddress.Parse(reader["IPAddress"] as string),
				LoggedAt = (DateTime)reader["LoggedAt"],
				ExpiresOn = reader["ExpiresOn"] as DateTime?,
				IsActive = (bool)reader["IsActive"]
			};
		}
		public PartnerSessionDTO SelectById(byte[] id) => Connector.ExecuteReader(selectByIdQuery, new Dictionary<string, object>() { { "Id", id } }, ReadFromDataReader).FirstOrDefault();
		public PartnerSessionDTO SelectLastByPartner(Guid partnerId) =>
			Connector.ExecuteReader(selectLastByPartnerQuery, new Dictionary<string, object>() { { "Partner", partnerId } }, ReadFromDataReader).FirstOrDefault();
		public int Update(byte[] id, IDictionary<string, object> fields) => Update("[Security].PartnerSession", "Id", id, fields);
	}
}