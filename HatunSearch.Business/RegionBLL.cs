﻿// Hatun Search | Layer: Business || Version: 2018.11.16.810
// (c) 2018 Hatun Search. All rights reserved.

// 'Using' directive
using HatunSearch.Business.Patterns;
using HatunSearch.Data;
using HatunSearch.Data.Databases;
using HatunSearch.Entities;
using System.Collections.Generic;

namespace HatunSearch.Business
{
	public sealed class RegionBLL : BLL<RegionRepository>
	{
		public RegionBLL(Connector connector) : base(connector) { }

		public IEnumerable<RegionDTO> ReadByCountry(string countryId) => Repository.ReadByCountry(countryId);
	}
}