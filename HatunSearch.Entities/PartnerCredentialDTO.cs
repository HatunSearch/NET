// Hatun Search | Layer: Entities || Version: 2018.11.16.810
// (c) 2018 Hatun Search. All rights reserved.

// 'Using' directive
using System.ComponentModel.DataAnnotations;

namespace HatunSearch.Entities
{
	public sealed class PartnerCredentialDTO
	{
		private string username = null;

		[Required(ErrorMessage = "UsernameIsRequired")]
		[StringLength(32, MinimumLength = 4, ErrorMessage = "UsernameLengthErrorMessage")]
		[RegularExpression(@"^[\p{L}\p{N}][\p{L}\p{N}-_.]{3,31}$", ErrorMessage = "UsernameFormatErrorMessage")]
		public string Username
		{
			get => username;
			set => username = value.ToLower();
		}
		[DataType(DataType.Password)]
		[Required(ErrorMessage = "PasswordIsRequired")]
		[StringLength(64, MinimumLength = 8, ErrorMessage = "PasswordLengthErrorMessage")]
		public string Password { get; set; }
	}
}