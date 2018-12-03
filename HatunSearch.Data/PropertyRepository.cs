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
	public sealed class PropertyRepository : Repository, IDeletableRepository<Guid>, IInsertableRepository<PropertyDTO, Guid?>, ISelectableRepository<PropertyDTO, Guid>, IUpdatableRepository<Guid>
	{
		private const string
			deleteQuery = "UPDATE Business.Property SET IsActive = 0 WHERE Id = @Id",
			insertQuery =
				@"INSERT INTO Business.Property ([Name], [Type], [Address], District, Province, Region, Country, [Partner]) OUTPUT inserted.Id VALUES(@Name, @Type, @Address, @District, @Province,
				@Region, @Country, @Partner)",
			selectByIdQuery =
				@"SELECT Id, [Name], [Type], [Address], District, Province, Region, Country, [Partner], PublishMode, HasBeenPaid, HasBeenReviewed, HasBeenPublished, IsActive FROM Business.Property
				WHERE Id = @Id AND IsActive = 1",
			selectByPartnerQuery =
				@"SELECT Id, [Name], [Type], [Address], District, Province, Region, Country, [Partner], PublishMode, HasBeenPaid, HasBeenReviewed, HasBeenPublished, IsActive FROM Business.Property
				WHERE [Partner] = @Partner AND IsActive = 1";

		public PropertyRepository() { }
		public PropertyRepository(Connector connector) : base(connector) { }

		public bool Delete(Guid id) => Connector.ExecuteNonQuery(deleteQuery, new Dictionary<string, object>() { { "Id", id } }) > 0;
		public void Insert(PropertyDTO property, out Guid? id)
		{
			id = Connector.ExecuteScalar<Guid?>(insertQuery, new Dictionary<string, object>()
			{
				{ "Name", property.Name },
				{ "Type", property.Type },
				{ "Address", property.Address },
				{ "District", property.District.Code },
				{ "Province", property.Province.Code },
				{ "Region", property.Region.Code },
				{ "Country", property.Country },
				{ "Partner", property.Partner }
			});
		}
		private PropertyDTO ReadFromDataReader(IDataReader reader) => ReadFromDataReader(reader, new CountryRepository(Connector), new DistrictRepository(Connector),
			new PropertyFeatureDetailRepository(Connector), new PropertyPictureRepository(Connector), new PropertyTypeRepository(Connector), new ProvinceRepository(Connector),
			new PublishModeRepository(Connector), new RegionRepository(Connector));
		private PropertyDTO ReadFromDataReader(IDataReader reader, CountryRepository countryRepository, DistrictRepository districtRepository, PropertyFeatureDetailRepository featureDetailRepository,
			PropertyPictureRepository pictureRepository, PropertyTypeRepository propertyTypeRepository, ProvinceRepository provinceRepository, PublishModeRepository publishModeRepository,
			RegionRepository regionRepository)
		{
			string countryId = reader["Country"] as string, districtCode = reader["District"] as string, provinceCode = reader["Province"] as string, regionCode = reader["Region"] as string;
			Guid propertyId = (Guid)reader["Id"];
			byte? publishModeId = reader["PublishMode"] as byte?;
			byte typeId = (byte)reader["Type"];
			CountryDTO country = new CountryDTO()
			{
				Id = countryId,
				DisplayName = new LocalizationDictionary(countryRepository.GetDisplayName(countryId)),
				Regions = regionRepository.ReadByCountry(countryId)
			};
			PropertyDTO result = new PropertyDTO()
			{
				Id = propertyId,
				Name = reader["Name"] as string,
				Type = new PropertyTypeDTO()
				{
					Id = typeId,
					DisplayName = new LocalizationDictionary(propertyTypeRepository.GetDisplayName(typeId))
				},
				Address = reader["Address"] as string,
				District = new DistrictDTO()
				{
					Code = districtCode,
					Country = country,
					DisplayName = new LocalizationDictionary(districtRepository.GetDisplayName(countryId, districtCode))
				},
				Province = new ProvinceDTO()
				{
					Code = provinceCode,
					Country = country,
					DisplayName = new LocalizationDictionary(provinceRepository.GetDisplayName(countryId, provinceCode)),
					Districts = districtRepository.ReadByCountryAndRegionAndProvince(countryId, regionCode, provinceCode)
				},
				Region = new RegionDTO()
				{
					Code = regionCode,
					Country = country,
					DisplayName = new LocalizationDictionary(regionRepository.GetDisplayName(countryId, regionCode)),
					Provinces = provinceRepository.ReadByCountryAndRegion(countryId, regionCode)
				},
				Country = country,
				Partner = new PartnerDTO() { Id = (Guid)reader["Partner"] },
				HasBeenPaid = (bool)reader["HasBeenPaid"],
				HasBeenReviewed = (bool)reader["HasBeenReviewed"],
				HasBeenPublished = (bool)reader["HasBeenPublished"],
				IsActive = (bool)reader["IsActive"],
				Features = featureDetailRepository.SelectByProperty(propertyId),
				Pictures = pictureRepository.SelectByProperty(propertyId)
			};
			if (publishModeId != null)
			{
				result.PublishMode = new PublishModeDTO()
				{
					Id = publishModeId.Value,
					DisplayName = new LocalizationDictionary(publishModeRepository.GetDisplayName(publishModeId.Value))
				};
			}
			return result;
		}
		public PropertyDTO SelectById(Guid id) => Connector.ExecuteReader(selectByIdQuery, new Dictionary<string, object>() { { "Id", id } }, ReadFromDataReader).FirstOrDefault();
		public IEnumerable<PropertyDTO> SelectByPartner(Guid partner)
		{
			CountryRepository countryRepository = new CountryRepository(Connector);
			DistrictRepository districtRepository = new DistrictRepository(Connector);
			PropertyFeatureDetailRepository featureDetailRepository = new PropertyFeatureDetailRepository(Connector);
			PropertyPictureRepository pictureRepository = new PropertyPictureRepository(Connector);
			PropertyTypeRepository propertyTypeRepository = new PropertyTypeRepository(Connector);
			ProvinceRepository provinceRepository = new ProvinceRepository(Connector);
			PublishModeRepository publishModeRepository = new PublishModeRepository(Connector);
			RegionRepository regionRepository = new RegionRepository(Connector);
			return Connector.ExecuteReader(selectByPartnerQuery, new Dictionary<string, object>() { { "Partner", partner } }, reader =>
				ReadFromDataReader(reader, countryRepository, districtRepository, featureDetailRepository, pictureRepository, propertyTypeRepository, provinceRepository, publishModeRepository, regionRepository));
		}
		public int Update(Guid id, IDictionary<string, object> fields) => Update("Business.Property", "Id", id, fields);
	}
}