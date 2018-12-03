// Hatun Search | Layer: Entities || Version: 2018.11.16.810
// (c) 2018 Hatun Search. All rights reserved.

// 'Using' directive
using System.ComponentModel.DataAnnotations;
using HatunSearch.Entities.Patterns;

namespace HatunSearch.Entities
{
	public sealed class PropertyFeatureDetailDTO : DTO<PropertyDTO, PropertyFeatureDTO>
	{
		public PropertyDTO Property
		{
			get => Id.FirstKey;
			set => Id.FirstKey = value;
		}
		[Required(ErrorMessage = "FeatureIsRequired")]
		public PropertyFeatureDTO Feature
		{
			get => Id.SecondKey;
			set => Id.SecondKey = value;
		}
		[Required(ErrorMessage = "ValueIsRequired")]
		[StringLength(32, MinimumLength = 1, ErrorMessage = "ValueLengthErrorMessage")]
		public string Value { get; set; }
	}
}