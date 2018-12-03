// Hatun Search | Layer: Data || Version: 2018.11.16.810
// (c) 2018 Hatun Search. All rights reserved.

// 'Using' directive
using HatunSearch.Data.Databases;
using HatunSearch.Data.Patterns;
using HatunSearch.Entities;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace HatunSearch.Data
{
	public sealed class PartnerCardRepository : Repository, IDeletableRepository<Guid>, IInsertableRepository<PartnerCardDTO, Guid?>
	{
		private const string
			deleteQuery = "DELETE FROM Payments.PartnerCard WHERE Id = @Id",
			insertQuery = "INSERT INTO Payments.PartnerCard ([Partner], StripeId) OUTPUT INSERTED.Id VALUES(@Partner, @StripeId)",
			selectByIdQuery =
				@"SELECT PartnerCard.Id, [Partner].Id AS PartnerId, [Partner].StripeId AS PartnerStripeId, PartnerCard.StripeId FROM Payments.PartnerCard AS PartnerCard
				JOIN Business.[Partner] AS [Partner] ON PartnerCard.Partner = [Partner].Id WHERE PartnerCard.Id = @Id",
			selectByPartnerQuery =
				@"SELECT PartnerCard.Id, [Partner].Id AS PartnerId, [Partner].StripeId AS PartnerStripeId, PartnerCard.StripeId FROM Payments.PartnerCard AS PartnerCard
				JOIN Business.[Partner] AS [Partner] ON PartnerCard.Partner = [Partner].Id WHERE PartnerCard.[Partner] = @Partner";

		public PartnerCardRepository() { }
		public PartnerCardRepository(Connector connector) : base(connector) { }

		public bool Delete(Guid id) => Connector.ExecuteNonQuery(deleteQuery, new Dictionary<string, object>() { { "Id", id } }) == 1;
		public void Insert(PartnerCardDTO card, out Guid? id)
		{
			id = Connector.ExecuteScalar<Guid?>(insertQuery, new Dictionary<string, object>()
			{
				{ "Partner", card.Partner.Id },
				{ "StripeId", card.StripeId }
			});
		}
		private PartnerCardDTO ReadFromDataReader(IDataReader reader)
		{
			return new PartnerCardDTO()
			{
				Id = (Guid)reader["Id"],
				Partner = new PartnerDTO()
				{
					Id = (Guid)reader["PartnerId"],
					StripeId = reader["PartnerStripeId"] as string
				},
				StripeId = reader["StripeId"] as string
			};
		}
		public PartnerCardDTO SelectById(Guid id) => Connector.ExecuteReader(selectByIdQuery, new Dictionary<string, object>() { { "Id", id } }, ReadFromDataReader).FirstOrDefault();
		public IEnumerable<PartnerCardDTO> SelectByPartner(Guid partnerId) =>
			Connector.ExecuteReader(selectByPartnerQuery, new Dictionary<string, object>() { { "Partner", partnerId } }, ReadFromDataReader);
	}
}