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
	public sealed class PropertyFeatureDetailBLL : BLL<PropertyFeatureDetailRepository>, ICreatableBLL<PropertyFeatureDetailDTO, PropertyFeatureDetailBLL.CreateResult>,
		IDeletableBLL<PropertyDTO, PropertyFeatureDTO, PropertyFeatureDetailBLL.DeleteResult>, IReadableBLL<PropertyFeatureDetailDTO, PropertyDTO, PropertyFeatureDTO>,
		IUpdatableBLL<PropertyDTO, PropertyFeatureDTO, PropertyFeatureDetailBLL.UpdateResult>
	{
		public PropertyFeatureDetailBLL(Connector connector) : base(connector) { }

		public enum CreateResult : byte
		{
			OK = 1,
			FeatureAlreadyAdded = 2
		}
		public enum DeleteResult : byte
		{
			OK = 1,
			NotFound = 2
		}
		public enum UpdateResult : byte
		{
			OK = 1,
			NotFound = 2,
			FeatureAlreadyAdded = 3
		}

		public CreateResult Create(PropertyFeatureDetailDTO featureDetail)
		{
			try
			{
				Repository.Insert(featureDetail);
				return CreateResult.OK;
			}
			catch (Exception exception)
			{
				if (exception.Message.Contains("PK_Business_PropertyFeatureDetail")) return CreateResult.FeatureAlreadyAdded;
				else throw exception;
			}
		}
		public DeleteResult Delete(PropertyDTO property, PropertyFeatureDTO feature) => Delete(property.Id, feature.Id);
		public DeleteResult Delete(Guid propertyId, byte featureId) => Repository.Delete(propertyId, featureId) ? DeleteResult.OK : DeleteResult.NotFound;
		public PropertyFeatureDetailDTO ReadById(PropertyDTO property, PropertyFeatureDTO feature) => Repository.SelectById(property, feature);
		public PropertyFeatureDetailDTO ReadById(Guid propertyId, byte featureId) => Repository.SelectById(propertyId, featureId);
		public IEnumerable<PropertyFeatureDetailDTO> ReadByProperty(Guid propertyId) => Repository.SelectByProperty(propertyId);
		public UpdateResult Update(PropertyDTO property, PropertyFeatureDTO feature, IDictionary<string, object> fields) => Update(property.Id, feature.Id, fields);
		public UpdateResult Update(Guid propertyId, byte featureId, IDictionary<string, object> fields)
		{
			try
			{
				Repository.Update(propertyId, featureId, fields);
				return UpdateResult.OK;
			}
			catch (Exception exception)
			{
				if (exception.Message.Contains("PK_Business_PropertyFeatureDetail")) return UpdateResult.FeatureAlreadyAdded;
				else throw exception;
			}
		}
	}
}