// Hatun Search | Layer: Entities || Version: 2018.11.16.810
// (c) 2018 Hatun Search. All rights reserved.

// 'Using' directive
using HatunSearch.Entities.Globalization;
using HatunSearch.Entities.Patterns;

namespace HatunSearch.Entities
{
	public sealed class PropertyFeatureDTO : DTO<byte>
	{
		public LocalizationDictionary DisplayName { get; set; }
	}
}