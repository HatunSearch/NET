// Hatun Search | Layer: Business || Version: 2018.11.16.810
// (c) 2018 Hatun Search. All rights reserved.

// 'Using' directive
using HatunSearch.Business.Patterns;
using HatunSearch.Data;
using HatunSearch.Data.Databases;
using HatunSearch.Entities;
using System;
using System.Collections.Generic;

namespace HatunSearch.Business
{
	public sealed class PropertyBLL : BLL<PropertyRepository>, ICreatableBLL<PropertyDTO, PropertyBLL.CreateResult>, IDeletableBLL<Guid, PropertyBLL.DeleteResult>, IReadableBLL<PropertyDTO, Guid>,
		IUpdatableBLL<Guid, PropertyBLL.UpdateResult>
	{
		public PropertyBLL(Connector connector) : base(connector) { }

		public enum CreateResult : byte { OK = 1 }
		public enum DeleteResult : byte
		{
			OK = 1,
			NotFound = 2
		}
		public enum UpdateResult : byte
		{
			OK = 1,
			NotFound = 2
		}

		public CreateResult Create(PropertyDTO property)
		{
			Repository.Insert(property, out Guid? id);
			if (id != null) property.Id = id.Value;
			return CreateResult.OK;
		}
		public DeleteResult Delete(Guid id) => Repository.Delete(id) ? DeleteResult.OK : DeleteResult.NotFound;
		public PropertyDTO ReadById(Guid id) => Repository.SelectById(id);
		public IEnumerable<PropertyDTO> ReadByPartner(Guid partnerId) => Repository.SelectByPartner(partnerId);
		public UpdateResult Update(Guid id, IDictionary<string, object> fields) => Repository.Update(id, fields) == 1 ? UpdateResult.OK : UpdateResult.NotFound;
	}
}