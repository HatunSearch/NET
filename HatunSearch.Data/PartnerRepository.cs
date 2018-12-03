// Hatun Search | Layer: Data || Version: 2018.11.16.810
// (c) 2018 Hatun Search. All rights reserved.

// 'Using' directive
using HatunSearch.Data.Databases;
using HatunSearch.Data.Patterns;
using HatunSearch.Entities;
using HatunSearch.Entities.Globalization;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace HatunSearch.Data
{
	public sealed class PartnerRepository : Repository, IInsertableRepository<PartnerDTO, Guid?>, ISelectableRepository<PartnerDTO, Guid>, IUpdatableRepository<Guid>
	{
		private const string
			insertQuery =
				@"INSERT INTO Business.[Partner] (Username, [Password], FirstName, MiddleName, LastName, Gender, EmailAddress, MobileNumber, CompanyName, [Address], District, Province, Region, Country,
				PhoneNumber, Website, PreferredCurrency, PreferredLanguage) OUTPUT INSERTED.Id VALUES(@Username, @Password, @FirstName, @MiddleName, @LastName, @Gender, @EmailAddress, @MobileNumber,
				@CompanyName, @Address, @District, @Province, @Region, @Country, @PhoneNumber, @Website, @PreferredCurrency, @PreferredLanguage)",
			selectByIdQuery =
				@"SELECT [Partner].Id, [Partner].Username, [Partner].[Password], [Partner].FirstName, [Partner].MiddleName, [Partner].LastName, [Partner].Gender, [Partner].EmailAddress,
				[Partner].MobileNumber, [Partner].CompanyName, [Partner].[Address], [Partner].District, [Partner].Province, [Partner].Region, [Partner].Country, [Partner].PhoneNumber,
				[Partner].Website, [Partner].PreferredCurrency AS PreferredCurrencyId, Currency.Symbol AS PreferredCurrencySymbol, [Partner].PreferredLanguage, [Partner].StripeId,
				[Partner].HasEmailAddressBeenVerified, [Partner].IsLocked, [Partner].IsActive FROM Business.[Partner] AS [Partner]
				JOIN [Geography].Currency AS Currency ON [Partner].PreferredCurrency = Currency.Id WHERE [Partner].Id = @Id AND [Partner].IsActive = 1",
			selectByUsernameQuery =
				@"SELECT [Partner].Id, [Partner].Username, [Partner].[Password], [Partner].FirstName, [Partner].MiddleName, [Partner].LastName, [Partner].Gender, [Partner].EmailAddress,
				[Partner].MobileNumber, [Partner].CompanyName, [Partner].[Address], [Partner].District, [Partner].Province, [Partner].Region, [Partner].Country, [Partner].PhoneNumber,
				[Partner].Website, [Partner].PreferredCurrency AS PreferredCurrencyId, Currency.Symbol AS PreferredCurrencySymbol, [Partner].PreferredLanguage, [Partner].StripeId,
				[Partner].HasEmailAddressBeenVerified, [Partner].IsLocked, [Partner].IsActive FROM Business.[Partner] AS [Partner]
				JOIN [Geography].Currency AS Currency ON [Partner].PreferredCurrency = Currency.Id WHERE [Partner].Username = @Username AND [Partner].IsActive = 1";

		public PartnerRepository() { }
		public PartnerRepository(Connector connector) : base(connector) { }

		public void Insert(PartnerDTO partner, out Guid? id)
		{
			id = Connector.ExecuteScalar<Guid?>(insertQuery, new Dictionary<string, object>()
			{
				{ "Username", partner.Username },
				{ "Password", partner.Password },
				{ "FirstName", partner.FirstName },
				{ "MiddleName", partner.MiddleName },
				{ "LastName", partner.LastName },
				{ "Gender", partner.Gender },
				{ "EmailAddress", partner.EmailAddress },
				{ "MobileNumber", partner.MobileNumber },
				{ "CompanyName", partner.CompanyName },
				{ "Address", partner.Address },
				{ "District", partner.District.Code },
				{ "Province", partner.Province.Code },
				{ "Region", partner.Region.Code },
				{ "Country", partner.Country },
				{ "PhoneNumber", partner.PhoneNumber },
				{ "Website", partner.Website },
				{ "PreferredCurrency", partner.PreferredCurrency },
				{ "PreferredLanguage", partner.PreferredLanguage }
			});
		}
		private PartnerDTO ReadFromDataReader(IDataReader reader) =>
			ReadFromDataReader(reader, new PartnerCardRepository(Connector), new CountryRepository(Connector), new CurrencyRepository(Connector), new CustomerQuestionRepository(Connector),
				new DistrictRepository(Connector), new GenderRepository(Connector), new PartnerInvoiceRepository(Connector), new LanguageRepository(Connector), new PropertyRepository(Connector),
				new ProvinceRepository(Connector), new RegionRepository(Connector));
		private PartnerDTO ReadFromDataReader(IDataReader reader, PartnerCardRepository cardRepository, CountryRepository countryRepository, CurrencyRepository currencyRepository,
			CustomerQuestionRepository customerQuestionRepository, DistrictRepository districtRepository, GenderRepository genderRepository, PartnerInvoiceRepository invoiceRepository,
			LanguageRepository languageRepository, PropertyRepository propertyRepository, ProvinceRepository provinceRepository, RegionRepository regionRepository)
		{
			string countryId = reader["Country"] as string, districtId = reader["District"] as string, genderId = reader["Gender"] as string,
				preferredCurrencyId = reader["PreferredCurrencyId"] as string, preferredLanguageId = reader["PreferredLanguage"] as string, provinceId = reader["Province"] as string,
				regionId = reader["Region"] as string;
			CountryDTO country = new CountryDTO()
			{
				Id = countryId,
				DisplayName = new LocalizationDictionary(countryRepository.GetDisplayName(countryId)),
				Regions = regionRepository.ReadByCountry(countryId),
				SupportedCurrencies = countryRepository.GetSupportedCurrencies(countryId),
				SupportedLanguages = countryRepository.GetSupportedLanguages(countryId)
			};
			Guid id = (Guid)reader["Id"];
			return new PartnerDTO()
			{
				Id = id,
				Username = reader["Username"] as string,
				Password = reader["Password"] as byte[],
				FirstName = reader["FirstName"] as string,
				MiddleName = reader["MiddleName"] as string,
				LastName = reader["LastName"] as string,
				Gender = new GenderDTO()
				{
					Id = genderId,
					DisplayName = new LocalizationDictionary(genderRepository.GetDisplayName(genderId))
				},
				EmailAddress = reader["EmailAddress"] as string,
				MobileNumber = reader["MobileNumber"] as string,
				CompanyName = reader["CompanyName"] as string,
				Address = reader["Address"] as string,
				District = new DistrictDTO()
				{
					Country = country,
					Code = districtId,
					DisplayName = new LocalizationDictionary(districtRepository.GetDisplayName(country.Id, districtId))
				},
				Province = new ProvinceDTO()
				{
					Country = country,
					Code = provinceId,
					DisplayName = new LocalizationDictionary(provinceRepository.GetDisplayName(country.Id, provinceId)),
					Districts = districtRepository.ReadByCountryAndRegionAndProvince(countryId, regionId, provinceId)
				},
				Region = new RegionDTO()
				{
					Country = country,
					Code = regionId,
					DisplayName = new LocalizationDictionary(regionRepository.GetDisplayName(country.Id, regionId)),
					Provinces = provinceRepository.ReadByCountryAndRegion(countryId, regionId)
				},
				Country = country,
				PhoneNumber = reader["PhoneNumber"] as string,
				Website = reader["Website"] as string,
				PreferredCurrency = new CurrencyDTO()
				{
					Id = preferredCurrencyId,
					DisplayName = new LocalizationDictionary(currencyRepository.GetDisplayName(preferredCurrencyId)),
					Symbol = reader["PreferredCurrencySymbol"] as string
				},
				PreferredLanguage = new LanguageDTO()
				{
					Id = preferredLanguageId,
					DisplayName = new LocalizationDictionary(languageRepository.GetDisplayName(preferredLanguageId))
				},
				StripeId = reader["StripeId"] as string,
				HasEmailAddressBeenVerified = (bool)reader["HasEmailAddressBeenVerified"],
				IsLocked = (bool)reader["IsLocked"],
				IsActive = (bool)reader["IsActive"],
				Cards = cardRepository.SelectByPartner(id),
				CustomerQuestions = customerQuestionRepository.SelectByPartner(id),
				Invoices = invoiceRepository.SelectByPartner(id),
				Properties = propertyRepository.SelectByPartner(id)
			};
		}
		public PartnerDTO SelectById(Guid id) => Connector.ExecuteReader(selectByIdQuery, new Dictionary<string, object>() { { "Id", id } }, ReadFromDataReader).FirstOrDefault();
		public PartnerDTO SelectByUsername(string username) =>
			Connector.ExecuteReader(selectByUsernameQuery, new Dictionary<string, object>() { { "Username", username } }, ReadFromDataReader).FirstOrDefault();
		public int Update(Guid id, IDictionary<string, object> fields) => Update("Business.[Partner]", "Id", id, fields);
	}
}