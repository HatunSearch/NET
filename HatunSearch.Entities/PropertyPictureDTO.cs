// Hatun Search | Layer: Entities || Version: 2018.11.16.810
// (c) 2018 Hatun Search. All rights reserved.

// 'Using' directive
using HatunSearch.Entities.Patterns;
using System;
using System.ComponentModel.DataAnnotations;

namespace HatunSearch.Entities
{
	public sealed class PropertyPictureDTO : DTO<Guid>
	{
		public PropertyDTO Property { get; set; }
		[Required(ErrorMessage = "DescriptionIsRequired")]
		[StringLength(32, MinimumLength = 1, ErrorMessage = "DescriptionLengthErrorMessage")]
		public string Description { get; set; }
	}
}