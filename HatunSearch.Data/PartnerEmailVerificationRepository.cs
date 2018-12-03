// Hatun Search | Layer: Data || Version: 2018.11.16.810
// (c) 2018 Hatun Search. All rights reserved.

// 'Using' directive
using HatunSearch.Data.Databases;
using HatunSearch.Data.Patterns;
using HatunSearch.Data.Security;
using HatunSearch.Entities;
using System;
using System.Collections.Generic;
using System.Linq;

namespace HatunSearch.Data
{
	public sealed class PartnerEmailVerificationRepository : Repository, IDeletableRepository<byte[]>, IInsertableRepository<PartnerEmailVerificationDTO, byte[]>,
		ISelectableRepository<PartnerEmailVerificationDTO, byte[]>
	{
		private const string deleteQuery = "UPDATE [Security].PartnerEmailVerification SET IsActive = 0 WHERE Id = @Id",
			insertQuery = "INSERT INTO [Security].PartnerEmailVerification (Id, [Partner], EmailAddress, ExpiresOn) VALUES(@Id, @Partner, @EmailAddress, @ExpiresOn)",
			selectByIdQuery = "SELECT Id, [Partner], EmailAddress, ExpiresOn, IsActive FROM [Security].PartnerEmailVerification WHERE Id = @Id AND IsActive = 1";

		public PartnerEmailVerificationRepository() { }
		public PartnerEmailVerificationRepository(Connector connector) : base(connector) { }

		public bool Delete(byte[] id) => Connector.ExecuteNonQuery(deleteQuery, new Dictionary<string, object>() { { "Id", id } }) == 1;
		public void Insert(PartnerEmailVerificationDTO emailVerification, out byte[] id)
		{
			byte[] generatedId = SecureIdGenerator.Generate(emailVerification);
			Connector.ExecuteNonQuery(insertQuery, new Dictionary<string, object>()
			{
				{ "Id", generatedId },
				{ "Partner", emailVerification.Partner },
				{ "EmailAddress", emailVerification.EmailAddress },
				{ "ExpiresOn", emailVerification.ExpiresOn }
			});
			id = generatedId;
		}
		public PartnerEmailVerificationDTO SelectById(byte[] id)
		{
			return Connector.ExecuteReader(selectByIdQuery, new Dictionary<string, object>() { { "Id", id } }, reader => new PartnerEmailVerificationDTO()
			{
				Id = reader["Id"] as byte[],
				Partner = new PartnerDTO() { Id = (Guid)reader["Partner"] },
				EmailAddress = reader["EmailAddress"] as string,
				ExpiresOn = reader["ExpiresOn"] as DateTime?,
				IsActive = (bool)reader["IsActive"]
			}).FirstOrDefault();
		}
	}
}