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
	public sealed class PropertyFeatureBLL : BLL<PropertyFeatureRepository>, IReadableBLL<PropertyFeatureDTO>
	{
		public PropertyFeatureBLL(Connector connector) : base(connector) { }

		public IEnumerable<PropertyFeatureDTO> ReadAll() => Repository.SelectAll();
	}
}