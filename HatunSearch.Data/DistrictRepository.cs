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
	public sealed class DistrictRepository : Repository
	{
		private const string getDisplayNameQuery = "SELECT Country, Code, [Language], DisplayName FROM Localization.District WHERE Country = @Country AND Code = @Code",
			readByCountryAndRegionAndProvinceQuery =
			@"SELECT District.Country, District.Code, Province.Code AS Province, Province.Region AS Region FROM [Geography].District AS District 
			JOIN [Geography].Province AS Province ON District.Country = Province.Country AND District.Province = Province.Code WHERE District.Country = @Country AND District.Province = @Province";

		public DistrictRepository() { }
		public DistrictRepository(Connector connector) : base(connector) { }

		internal IEnumerable<KeyValuePair<string, string>> GetDisplayName(string countryId, string code) =>
			Connector.ExecuteReader(getDisplayNameQuery, new Dictionary<string, object>()
			{
				{ "Country", countryId },
				{ "Code", code }
			}, reader => new KeyValuePair<string, string>(reader["Language"] as string, reader["DisplayName"] as string));
		public IEnumerable<DistrictDTO> ReadByCountryAndRegionAndProvince(string countryId, string regionCode, string provinceCode)
		{
			return Connector.ExecuteReader(readByCountryAndRegionAndProvinceQuery, new Dictionary<string, object>()
			{
				{ "Country", countryId },
				{ "Province", provinceCode }
			}, reader =>
			{
				CountryDTO country = new CountryDTO() { Id = reader["Country"] as string };
				string code = reader["Code"] as string;
				return new DistrictDTO()
				{
					Country = country,
					Code = code,
					Province = new ProvinceDTO()
					{
						Code = reader["Province"] as string,
						Country = country,
						Region = new RegionDTO()
						{
							Code = reader["Region"] as string,
							Country = country
						}
					},
					DisplayName = new LocalizationDictionary(GetDisplayName(countryId, code))
				};
			});
		}
	}
}