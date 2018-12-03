// Hatun Search | Layer: Entities || Version: 2018.11.16.810
// (c) 2018 Hatun Search. All rights reserved.

// 'Using' directive
using HatunSearch.Entities.DataAnnotations;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace HatunSearch.Entities
{
	public sealed class PartnerCompanyInfoDTO
	{
		[DisplayName("CompanyName")]
		[Required(ErrorMessage = "CompanyNameIsRequired")]
		[StringLength(32, MinimumLength = 2, ErrorMessage = "CompanyNameLengthErrorMessage")]
		public string Name { get; set; }
		[Required(ErrorMessage = "AddressIsRequired")]
		[StringLength(32, MinimumLength = 4, ErrorMessage = "AddressLengthErrorMessage")]
		[RegularExpression(@"^[\p{L}\p{N} :,°'#.;]{4,32}$", ErrorMessage = "AddressFormatErrorMessage")]
		public string Address { get; set; }
		[Required(ErrorMessage = "DistrictIsRequired")]
		public DistrictDTO District { get; set; }
		[Required(ErrorMessage = "ProvinceIsRequired")]
		public ProvinceDTO Province
		{
			get => District.Province;
			set => District.Province = value;
		}
		[Required(ErrorMessage = "RegionIsRequired")]
		public RegionDTO Region
		{
			get => Province.Region;
			set => Province.Region = value;
		}
		[Required(ErrorMessage = "CountryIsRequired")]
		public CountryDTO Country { get; set; }
		[DataType(DataType.PhoneNumber)]
		[Required(ErrorMessage = "PhoneNumberIsRequired")]
		[PhoneNumberEx(ErrorMessage = "PhoneNumberFormatErrorMessage")]
		public string PhoneNumber { get; set; }
		[DataType(DataType.Url)]
		[Required(ErrorMessage = "WebsiteIsRequired")]
		[UrlEx(ErrorMessage = "WebsiteFormatErrorMessage")]
		public string Website { get; set; }
	}
}