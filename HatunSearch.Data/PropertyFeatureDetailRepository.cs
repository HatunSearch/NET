// Hatun Search | Layer: Data || Version: 2018.11.16.810
// (c) 2018 Hatun Search. All rights reserved.

// 'Using' directive
using HatunSearch.Data.Databases;
using HatunSearch.Data.Patterns;
using HatunSearch.Entities;
using HatunSearch.Entities.Globalization;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace HatunSearch.Data
{
	public sealed class PropertyFeatureDetailRepository : Repository, IDeletableRepository<PropertyDTO, PropertyFeatureDTO>, IInsertableRepository<PropertyFeatureDetailDTO>,
		ISelectableRepository<PropertyFeatureDetailDTO, PropertyDTO, PropertyFeatureDTO>, IUpdatableRepository<PropertyDTO, PropertyFeatureDTO>
	{
		private const string deleteQuery = "DELETE FROM Business.PropertyFeatureDetail WHERE Property = @Property AND Feature = @Feature",
			insertQuery = "INSERT INTO Business.PropertyFeatureDetail (Property, Feature, [Value]) VALUES(@Property, @Feature, @Value)",
			selectByIdQuery = "SELECT Property, Feature, [Value] FROM Business.PropertyFeatureDetail WHERE Property = @Property AND Feature = @Feature",
			selectByPropertyQuery = "SELECT Property, Feature, [Value] FROM Business.PropertyFeatureDetail WHERE Property = @Property";

		public PropertyFeatureDetailRepository() { }
		public PropertyFeatureDetailRepository(Connector connector) : base(connector) { }

		public bool Delete(PropertyDTO property, PropertyFeatureDTO feature) => Delete(property.Id, feature.Id);
		public bool Delete(Guid propertyId, byte featureId) => Connector.ExecuteNonQuery(deleteQuery, new Dictionary<string, object>()
		{
			{ "Property", propertyId},
			{ "Feature", featureId }
		}) == 1;
		public void Insert(PropertyFeatureDetailDTO featureDetail)
		{
			Connector.ExecuteNonQuery(insertQuery, new Dictionary<string, object>()
			{
				{ "Property", featureDetail.Property },
				{ "Feature", featureDetail.Feature },
				{ "Value", featureDetail.Value }
			});
		}
		private PropertyFeatureDetailDTO ReadFromDataReader(IDataReader reader) => ReadFromDataReader(reader, new PropertyFeatureRepository(Connector));
		private PropertyFeatureDetailDTO ReadFromDataReader(IDataReader reader, PropertyFeatureRepository featureRepository)
		{
			byte featureId = (byte)reader["Feature"];
			return new PropertyFeatureDetailDTO()
			{
				Property = new PropertyDTO() { Id = (Guid)reader["Property"] },
				Feature = new PropertyFeatureDTO()
				{
					Id = featureId,
					DisplayName = new LocalizationDictionary(featureRepository.GetDisplayName(featureId))
				},
				Value = reader["Value"] as string
			};
		}
		public PropertyFeatureDetailDTO SelectById(PropertyDTO property, PropertyFeatureDTO feature) => SelectById(property.Id, feature.Id);
		public PropertyFeatureDetailDTO SelectById(Guid propertyId, byte featureId) =>
			Connector.ExecuteReader(selectByIdQuery, new Dictionary<string, object>()
			{
				{ "Property", propertyId },
				{ "Feature", featureId }
			}, ReadFromDataReader).FirstOrDefault();
		public IEnumerable<PropertyFeatureDetailDTO> SelectByProperty(Guid propertyId)
		{
			PropertyFeatureRepository featureRepository = new PropertyFeatureRepository(Connector);
			return Connector.ExecuteReader(selectByPropertyQuery, new Dictionary<string, object>() { { "Property", propertyId } }, reader => ReadFromDataReader(reader, featureRepository));
		}
		public int Update(PropertyDTO property, PropertyFeatureDTO feature, IDictionary<string, object> fields) => Update(property.Id, feature.Id, fields);
		public int Update(Guid propertyId, byte featureId, IDictionary<string, object> fields) => Update("Business.PropertyFeatureDetail", "Property", propertyId, "Feature", featureId, fields);
	}
}