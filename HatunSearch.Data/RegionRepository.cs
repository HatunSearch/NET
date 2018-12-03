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
	public sealed class RegionRepository : Repository
	{
		private const string getDisplayNameQuery = "SELECT Country, Code, [Language], DisplayName FROM Localization.Region WHERE Country = @Country AND Code = @Code",
			readByCountry = "SELECT Country, Code FROM [Geography].Region WHERE Country = @Country";

		public RegionRepository() { }
		public RegionRepository(Connector connector) : base(connector) { }

		internal IEnumerable<KeyValuePair<string, string>> GetDisplayName(string countryId, string code) =>
			Connector.ExecuteReader(getDisplayNameQuery, new Dictionary<string, object>()
			{
				{ "Country", countryId },
				{ "Code", code }
			}, reader => new KeyValuePair<string, string>(reader["Language"] as string, reader["DisplayName"] as string));
		public IEnumerable<RegionDTO> ReadByCountry(string countryId)
		{
			ProvinceRepository provinceRepository = new ProvinceRepository(Connector);
			return Connector.ExecuteReader(readByCountry, new Dictionary<string, object>() { { "Country", countryId }, }, reader =>
			{
				string code = reader["Code"] as string, country = reader["Country"] as string;
				return new RegionDTO()
				{
					Country = new CountryDTO() { Id = country },
					Code = code,
					DisplayName = new LocalizationDictionary(GetDisplayName(countryId, code)),
					Provinces = provinceRepository.ReadByCountryAndRegion(country, code)
				};
			});
		}
	}
}