// Hatun Search | Layer: Business || Version: 2018.11.16.810
// (c) 2018 Hatun Search. All rights reserved.

// 'Using' directive
using HatunSearch.Business.Patterns;
using HatunSearch.Data;
using HatunSearch.Data.Databases;
using HatunSearch.Entities;
using System.Collections.Generic;

namespace HatunSearch.Business
{
	public sealed class DistrictBLL : BLL<DistrictRepository>
	{
		public DistrictBLL(Connector connector) : base(connector) { }

		public IEnumerable<DistrictDTO> ReadByCountryAndRegionAndProvince(string countryId, string regionCode, string provinceCode) => Repository.ReadByCountryAndRegionAndProvince(countryId, regionCode, provinceCode);
	}
}