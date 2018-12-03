// Hatun Search | Layer: Data || Version: 2018.11.16.810
// (c) 2018 Hatun Search. All rights reserved.

// 'Using' directive
using HatunSearch.Data.Databases;
using HatunSearch.Data.Patterns;
using HatunSearch.Entities;
using HatunSearch.Entities.Globalization;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace HatunSearch.Data
{
	public sealed class PublishModeRepository : Repository, ISelectableRepository<PublishModeDTO>, ISelectableRepository<PublishModeDTO, byte>
	{
		private const string getDisplayNameQuery = "SELECT Id, [Language], DisplayName FROM Localization.PublishMode WHERE Id = @Id",
			selectAllQuery = "SELECT Id, Currency, Cost FROM Business.PublishMode",
			selectByIdQuery = "SELECT Id, Currency, Cost FROM Business.PublishMode WHERE Id = @Id";

		public PublishModeRepository() { }
		public PublishModeRepository(Connector connector) : base(connector) { }

		internal IEnumerable<KeyValuePair<string, string>> GetDisplayName(byte id) =>
			Connector.ExecuteReader(getDisplayNameQuery, new Dictionary<string, object>() { { "Id", id } },
				reader => new KeyValuePair<string, string>(reader["Language"] as string, reader["DisplayName"] as string));
		private PublishModeDTO ReadFromDataReader(IDataReader reader)
		{
			byte id = (byte)reader["Id"];
			return new PublishModeDTO()
			{
				Id = id,
				DisplayName = new LocalizationDictionary(GetDisplayName(id)),
				Currency = new CurrencyDTO() { Id = reader["Currency"] as string },
				Cost = (decimal)reader["Cost"]
			};
		}
		public IEnumerable<PublishModeDTO> SelectAll() => Connector.ExecuteReader(selectAllQuery, ReadFromDataReader);
		public PublishModeDTO SelectById(byte id) => Connector.ExecuteReader(selectByIdQuery, new Dictionary<string, object>() { { "Id", id } }, ReadFromDataReader).FirstOrDefault();
	}
}