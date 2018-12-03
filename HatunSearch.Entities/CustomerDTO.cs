// Hatun Search | Layer: Entities || Version: 2018.11.16.810
// (c) 2018 Hatun Search. All rights reserved.

// 'Using' directive
using HatunSearch.Entities.Patterns;
using System;
using System.Text;

namespace HatunSearch.Entities
{
	public sealed class CustomerDTO : DTO<Guid>
	{
		public string Username { get; set; }
		public byte[] Password { get; set; }
		public string FirstName { get; set; }
		public string MiddleName { get; set; }
		public string LastName { get; set; }
		public GenderDTO Gender { get; set; }
		public string EmailAddress { get; set; }
		public string MobileNumber { get; set; }
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
		public CurrencyDTO PreferredCurrency { get; set; }
		public LanguageDTO PreferredLanguage { get; set; }
		public bool HasEmailAddressBeenVerified { get; set; }
		public bool IsLocked { get; set; }
		public bool IsActive { get; set; }

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
	}
}