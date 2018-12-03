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
	public sealed class CountryRepository : Repository, ISelectableRepository<CountryDTO>, ISelectableRepository<CountryDTO, string>
	{
		private const string getDisplayNameQuery = "SELECT Id, [Language], DisplayName FROM Localization.Country WHERE Id = @Id",
			getSupportedCurrenciesQuery = "SELECT Currency AS Id FROM [Geography].CountrySupportedCurrency WHERE Country = @Id",
			getSupportedLanguagesQuery = "SELECT [Language] AS Id FROM [Geography].CountrySupportedLanguage WHERE Country = @Id",
			selectAllQuery = "SELECT Id, PredominantCurrency, PredominantLanguage FROM [Geography].Country",
			selectByIdQuery = "SELECT Id, PredominantCurrency, PredominantLanguage FROM [Geography].Country WHERE Id = @Id";

		public CountryRepository() { }
		public CountryRepository(Connector connector) : base(connector) { }

		internal IEnumerable<KeyValuePair<string, string>> GetDisplayName(string id) =>
			Connector.ExecuteReader(getDisplayNameQuery, new Dictionary<string, object>() { { "Id", id } },
				reader => new KeyValuePair<string, string>(reader["Language"] as string, reader["DisplayName"] as string));
		internal IEnumerable<CurrencyDTO> GetSupportedCurrencies(string id) => GetSupportedCurrencies(id, new CurrencyRepository(Connector));
		internal IEnumerable<CurrencyDTO> GetSupportedCurrencies(string id, CurrencyRepository currencyRepository)
		{
			return Connector.ExecuteReader(getSupportedCurrenciesQuery, new Dictionary<string, object>() { { "Id", id } }, reader =>
			{
				string currencyId = reader["Id"] as string;
				return new CurrencyDTO()
				{
					Id = currencyId,
					DisplayName = new LocalizationDictionary(currencyRepository.GetDisplayName(currencyId))
				};
			});
		}
		internal IEnumerable<LanguageDTO> GetSupportedLanguages(string id) => GetSupportedLanguages(id, new LanguageRepository(Connector));
		internal IEnumerable<LanguageDTO> GetSupportedLanguages(string id, LanguageRepository languageRepository)
		{
			return Connector.ExecuteReader(getSupportedLanguagesQuery, new Dictionary<string, object>() { { "Id", id } }, reader =>
			{
				string languageId = reader["Id"] as string;
				return new LanguageDTO()
				{
					Id = languageId,
					DisplayName = new LocalizationDictionary(languageRepository.GetDisplayName(languageId))
				};
			});
		}
		private CountryDTO ReadFromDataReader(IDataReader reader) => ReadFromDataReader(reader, new CurrencyRepository(Connector), new LanguageRepository(Connector), new RegionRepository(Connector));
		private CountryDTO ReadFromDataReader(IDataReader reader, CurrencyRepository currencyRepository, LanguageRepository languageRepository, RegionRepository regionRepository)
		{
			string id = reader["Id"] as string;
			return new CountryDTO()
			{
				Id = id,
				DisplayName = new LocalizationDictionary(GetDisplayName(id)),
				PredominantCurrency = new CurrencyDTO() { Id = reader["PredominantCurrency"] as string },
				PredominantLanguage = new LanguageDTO() { Id = reader["PredominantLanguage"] as string },
				Regions = regionRepository.ReadByCountry(id),
				SupportedCurrencies = GetSupportedCurrencies(id, currencyRepository),
				SupportedLanguages = GetSupportedLanguages(id, languageRepository)
			};
		}
		public IEnumerable<CountryDTO> SelectAll()
		{
			CurrencyRepository currencyRepository = new CurrencyRepository(Connector);
			LanguageRepository languageRepository = new LanguageRepository(Connector);
			RegionRepository regionRepository = new RegionRepository(Connector);
			return Connector.ExecuteReader(selectAllQuery, reader => ReadFromDataReader(reader, currencyRepository, languageRepository, regionRepository));
		}
		public CountryDTO SelectById(string id) => Connector.ExecuteReader(selectByIdQuery, new Dictionary<string, object>() { { "Id", id } }, ReadFromDataReader).FirstOrDefault();
	}
}