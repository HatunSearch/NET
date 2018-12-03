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
	public sealed class ProvinceRepository : Repository
	{
		private const string getDisplayNameQuery = "SELECT Country, Code, [Language], DisplayName FROM Localization.Province WHERE Country = @Country AND Code = @Code",
			readByCountryAndRegionQuery = "SELECT Country, Code, Region FROM [Geography].Province WHERE Country = @Country AND Region = @Region";

		public ProvinceRepository() { }
		public ProvinceRepository(Connector connector) : base(connector) { }

		internal IEnumerable<KeyValuePair<string, string>> GetDisplayName(string countryId, string code) =>
			Connector.ExecuteReader(getDisplayNameQuery, new Dictionary<string, object>()
			{
				{ "Country", countryId },
				{ "Code", code }
			}, reader => new KeyValuePair<string, string>(reader["Language"] as string, reader["DisplayName"] as string));
		public IEnumerable<ProvinceDTO> ReadByCountryAndRegion(string countryId, string regionCode)
		{
			DistrictRepository districtRepository = new DistrictRepository(Connector);
			return Connector.ExecuteReader(readByCountryAndRegionQuery, new Dictionary<string, object>()
			{
				{ "Country", countryId },
				{ "Region", regionCode }
			}, reader =>
			{
				CountryDTO country = new CountryDTO() { Id = reader["Country"] as string };
				string code = reader["Code"] as string, region = reader["Region"] as string;
				return new ProvinceDTO()
				{
					Country = country,
					Code = code,
					Region = new RegionDTO()
					{
						Code = region,
						Country = country
					},
					DisplayName = new LocalizationDictionary(GetDisplayName(country.Id, code)),
					Districts = districtRepository.ReadByCountryAndRegionAndProvince(country.Id, region, code)
				};
			});
		}
	}
}