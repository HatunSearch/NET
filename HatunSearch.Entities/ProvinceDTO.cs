// Hatun Search | Layer: Entities || Version: 2018.11.16.810
// (c) 2018 Hatun Search. All rights reserved.

// 'Using' directive
using HatunSearch.Entities.Globalization;
using HatunSearch.Entities.Patterns;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace HatunSearch.Entities
{
	public sealed class ProvinceDTO : DTO<CountryDTO, string>
	{
		public CountryDTO Country
		{
			get => Id.FirstKey;
			set => Id.FirstKey = value;
		}
		public string Code
		{
			get => Id.SecondKey;
			set => Id.SecondKey = value;
		}
		public LocalizationDictionary DisplayName { get; set; }
		[JsonIgnore]
		public RegionDTO Region { get; set; }

		[JsonIgnore]
		public IEnumerable<DistrictDTO> Districts { get; set; }
	}
}