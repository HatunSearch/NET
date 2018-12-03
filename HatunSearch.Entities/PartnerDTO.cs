// Hatun Search | Layer: Entities || Version: 2018.11.16.810
// (c) 2018 Hatun Search. All rights reserved.

// 'Using' directive
using HatunSearch.Entities.Patterns;
using HatunSearch.Entities.Security;
using System;
using System.Collections.Generic;
using System.Text;

namespace HatunSearch.Entities
{
	public sealed class PartnerDTO : DTO<Guid>
	{
		public void Join(PartnerCompanyInfoDTO companyInfo)
		{
			CompanyName = companyInfo.Name;
			Address = companyInfo.Address;
			District = companyInfo.District;
			Province = companyInfo.Province;
			Region = companyInfo.Region;
			Country = companyInfo.Country;
			PhoneNumber = companyInfo.PhoneNumber;
			Website = companyInfo.Website;
		}
		public void Join(PartnerCredentialDTO credential)
		{
			Username = credential.Username;
			Password = SHA512Hasher.Hash(credential.Password);
		}
		public void Join(PartnerPersonalInfoDTO personalInfo)
		{
			FirstName = personalInfo.FirstName;
			MiddleName = personalInfo.MiddleName;
			LastName = personalInfo.LastName;
			Gender = personalInfo.Gender;
			EmailAddress = personalInfo.EmailAddress;
			MobileNumber = personalInfo.MobileNumber;
		}
		public void Join(PartnerPreferencesDTO preferences)
		{
			PreferredCurrency = preferences.PreferredCurrency;
			PreferredLanguage = preferences.PreferredLanguage;
		}

		public string Username { get; set; }
		public byte[] Password { get; set; }
		public string FirstName { get; set; }
		public string MiddleName { get; set; }
		public string LastName { get; set; }
		public GenderDTO Gender { get; set; }
		public string EmailAddress { get; set; }
		public string MobileNumber { get; set; }
		public string CompanyName { get; set; }
		public string Address { get; set; }
		public DistrictDTO District { get; set; }
		public ProvinceDTO Province
		{
			get => District.Province;
			set => District.Province = value;
		}
		public RegionDTO Region
		{
			get => Province.Region;
			set => Province.Region = value;
		}
		public CountryDTO Country { get; set; }
		public string PhoneNumber { get; set; }
		public string Website { get; set; }
		public CurrencyDTO PreferredCurrency { get; set; }
		public LanguageDTO PreferredLanguage { get; set; }
		public string StripeId { get; set; }
		public bool HasEmailAddressBeenVerified { get; set; }
		public bool IsLocked { get; set; }
		public bool IsActive { get; set; }

		public IEnumerable<PartnerCardDTO> Cards { get; set; }
		public IEnumerable<CustomerQuestionDTO> CustomerQuestions { get; set; }
		public IEnumerable<PartnerInvoiceDTO> Invoices { get; set; }
		public IEnumerable<PartnerLoginAttemptDTO> LoginAttempts { get; set; }
		public IEnumerable<PropertyDTO> Properties { get; set; }
		public IEnumerable<PartnerSessionDTO> Sessions { get; set; }

		public PartnerCompanyInfoDTO CompanyInfo => new PartnerCompanyInfoDTO()
		{
			Name = CompanyName,
			Address = Address,
			District = District,
			Province = Province,
			Region = Region,
			Country = Country,
			PhoneNumber = PhoneNumber,
			Website = Website
		};
		public string FullName
		{
			get
			{
				StringBuilder stringBuilder = new StringBuilder();
				stringBuilder.Append(FirstName);
				if (MiddleName != null) stringBuilder.Append($" {MiddleName}");
				stringBuilder.Append($" {LastName}");
				return stringBuilder.ToString();
			}
		}
		public PartnerPersonalInfoDTO PersonalInfo => new PartnerPersonalInfoDTO()
		{
			FirstName = FirstName,
			MiddleName = MiddleName,
			LastName = LastName,
			Gender = Gender,
			EmailAddress = EmailAddress,
			MobileNumber = MobileNumber
		};
		public PartnerPreferencesDTO Preferences => new PartnerPreferencesDTO()
		{
			PreferredCurrency = PreferredCurrency,
			PreferredLanguage = PreferredLanguage
		};
	}
}