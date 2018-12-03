// Hatun Search | Layer: Entities || Version: 2018.11.16.810
// (c) 2018 Hatun Search. All rights reserved.

// 'Using' directives
using HatunSearch.Entities.DataAnnotations;
using System.ComponentModel.DataAnnotations;

namespace HatunSearch.Entities
{
	public sealed class PartnerPersonalInfoDTO
	{
		private string emailAddress = null;

		[Required(ErrorMessage = "FirstNameIsRequired")]
		[StringLength(16, MinimumLength = 2, ErrorMessage = "FirstNameLengthErrorMessage")]
		[RegularExpression(@"^[\p{L} '-]{2,16}$", ErrorMessage = "FirstNameFormatErrorMessage")]
		public string FirstName { get; set; }
		[StringLength(16, MinimumLength = 2, ErrorMessage = "MiddleNameLengthErrorMessage")]
		[RegularExpression(@"^[\p{L} '-]{2,16}$", ErrorMessage = "MiddleNameFormatErrorMessage")]
		public string MiddleName { get; set; }
		[Required(ErrorMessage = "LastNameIsRequired")]
		[StringLength(32, MinimumLength = 2, ErrorMessage = "LastNameLengthErrorMessage")]
		[RegularExpression(@"^[\p{L} '-]{2,32}$", ErrorMessage = "LastNameFormatErrorMessage")]
		public string LastName { get; set; }
		[Required(ErrorMessage = "GenderIsRequired")]
		public GenderDTO Gender { get; set; }
		[DataType(DataType.EmailAddress)]
		[Required(ErrorMessage = "EmailAddressIsRequired")]
		[StringLength(255, MinimumLength = 6, ErrorMessage = "EmailAddressLengthErrorMessage")]
		[EmailAddressEx(ErrorMessage = "EmailAddressFormatErrorMessage")]
		public string EmailAddress
		{
			get => emailAddress;
			set => emailAddress = value.ToLower();
		}
		[DataType(DataType.PhoneNumber)]
		[Required(ErrorMessage = "MobileNumberIsRequired")]
		[StringLength(15, MinimumLength = 5, ErrorMessage = "MobileNumberLengthErrorMessage")]
		[PhoneNumberEx(acceptMobileNumbersOnly: true, ErrorMessage = "MobileNumberFormatErrorMessage")]
		public string MobileNumber { get; set; }
	}
}