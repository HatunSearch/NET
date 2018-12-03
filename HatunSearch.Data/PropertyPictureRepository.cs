// Hatun Search | Layer: Data || Version: 2018.11.16.810
// (c) 2018 Hatun Search. All rights reserved.

// 'Using' directive
using HatunSearch.Data.Databases;
using HatunSearch.Data.Patterns;
using HatunSearch.Entities;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace HatunSearch.Data
{
	public sealed class PropertyPictureRepository : Repository, IDeletableRepository<Guid>, IInsertableRepository<PropertyPictureDTO, Guid?>, ISelectableRepository<PropertyPictureDTO, Guid>,
		IUpdatableRepository<Guid>
	{
		private const string deleteQuery = "DELETE FROM Business.PropertyPicture WHERE Id = @Id",
			insertQuery = "INSERT INTO Business.PropertyPicture (Property, [Description]) OUTPUT inserted.Id VALUES(@Property, @Description)",
			selectByIdQuery = "SELECT Id, Property, [Description] FROM Business.PropertyPicture WHERE Id = @Id",
			selectByPropertyQuery = "SELECT Id, Property, [Description] FROM Business.PropertyPicture WHERE Property = @Property";

		public PropertyPictureRepository() { }
		public PropertyPictureRepository(Connector connector) : base(connector) { }

		public bool Delete(Guid id) => Connector.ExecuteNonQuery(deleteQuery, new Dictionary<string, object>() { { "Id", id } }) == 1;
		public void Insert(PropertyPictureDTO picture, out Guid? id)
		{
			id = Connector.ExecuteScalar<Guid?>(insertQuery, new Dictionary<string, object>()
			{
				{ "Property", picture.Property },
				{ "Description", picture.Description }
			});
		}
		private PropertyPictureDTO ReadFromDataReader(IDataReader reader)
		{
			return new PropertyPictureDTO()
			{
				Id = (Guid)reader["Id"],
				Property = new PropertyDTO() { Id = (Guid)reader["Property"] },
				Description = reader["Description"] as string
			};
		}
		public PropertyPictureDTO SelectById(Guid id) => Connector.ExecuteReader(selectByIdQuery, new Dictionary<string, object>() { { "Id", id } }, ReadFromDataReader).FirstOrDefault();
		public IEnumerable<PropertyPictureDTO> SelectByProperty(Guid propertyId) =>
			Connector.ExecuteReader(selectByPropertyQuery, new Dictionary<string, object>() { { "Property", propertyId } }, ReadFromDataReader);
		public int Update(Guid id, IDictionary<string, object> fields) => Update("Business.PropertyPicture", "Id", id, fields);
	}
}