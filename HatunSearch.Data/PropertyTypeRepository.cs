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
	public sealed class PropertyTypeRepository : Repository, ISelectableRepository<PropertyTypeDTO>
	{
		private const string getDisplayNameQuery = "SELECT Id, [Language], DisplayName FROM Localization.PropertyType WHERE Id = @Id", selectAllQuery = "SELECT Id FROM Business.PropertyType";

		public PropertyTypeRepository() { }
		public PropertyTypeRepository(Connector connector) : base(connector) { }

		internal IEnumerable<KeyValuePair<string, string>> GetDisplayName(byte id) =>
			Connector.ExecuteReader(getDisplayNameQuery, new Dictionary<string, object>() { { "Id", id } },
				reader => new KeyValuePair<string, string>(reader["Language"] as string, reader["DisplayName"] as string));
		public IEnumerable<PropertyTypeDTO> SelectAll() =>
			Connector.ExecuteReader(selectAllQuery, reader =>
			{
				byte id = (byte)reader["Id"];
				return new PropertyTypeDTO()
				{
					Id = id,
					DisplayName = new LocalizationDictionary(GetDisplayName(id))
				};
			});
	}
}