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
	public sealed class PropertyPictureBLL : BLL<PropertyPictureRepository>, ICreatableBLL<PropertyPictureDTO, PropertyPictureBLL.CreateResult>, IDeletableBLL<Guid, PropertyPictureBLL.DeleteResult>,
		IReadableBLL<PropertyPictureDTO, Guid>, IUpdatableBLL<Guid, PropertyPictureBLL.UpdateResult>
	{
		public PropertyPictureBLL(Connector connector) : base(connector) { }

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

		public CreateResult Create(PropertyPictureDTO picture)
		{
			Repository.Insert(picture, out Guid? id);
			if (id != null) picture.Id = id.Value;
			return CreateResult.OK;
		}
		public DeleteResult Delete(Guid id) => Repository.Delete(id) ? DeleteResult.OK : DeleteResult.NotFound;
		public PropertyPictureDTO ReadById(Guid id) => Repository.SelectById(id);
		public UpdateResult Update(Guid id, IDictionary<string, object> fields) => Repository.Update(id, fields) == 1 ? UpdateResult.OK : UpdateResult.NotFound;
	}
}