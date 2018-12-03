// Hatun Search | Layer: Data || Version: 2018.11.16.810
// (c) 2018 Hatun Search. All rights reserved.

// 'Using' directive
using HatunSearch.Data.Databases;
using HatunSearch.Data.Patterns;
using HatunSearch.Entities;
using HatunSearch.Entities.Globalization;
using System.Collections.Generic;

namespace HatunSearch.Data
{
	public sealed class LanguageRepository : Repository, ISelectableRepository<LanguageDTO>
	{
		private const string getDisplayNameQuery = "SELECT Id, [Language], DisplayName FROM Localization.[Language] WHERE Id = @Id", selectAllQuery = "SELECT Id FROM [Geography].[Language]";

		public LanguageRepository() { }
		public LanguageRepository(Connector connector) : base(connector) { }

		internal IEnumerable<KeyValuePair<string, string>> GetDisplayName(string id) =>
			Connector.ExecuteReader(getDisplayNameQuery, new Dictionary<string, object>() { { "Id", id } },
				reader => new KeyValuePair<string, string>(reader["Language"] as string, reader["DisplayName"] as string));
		public IEnumerable<LanguageDTO> SelectAll() =>
			Connector.ExecuteReader(selectAllQuery, reader =>
			{
				string id = reader["Id"] as string;
				return new LanguageDTO()
				{
					Id = id,
					DisplayName = new LocalizationDictionary(GetDisplayName(id))
				};
			});
	}
}