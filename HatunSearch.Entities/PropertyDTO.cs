// Hatun Search | Layer: Entities || Version: 2018.11.16.810
// (c) 2018 Hatun Search. All rights reserved.

// 'Using' directive
using HatunSearch.Entities.Patterns;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace HatunSearch.Entities
{
	public sealed class PropertyDTO : DTO<Guid>
	{
		public enum PropertyStatus
		{
			WaitingForPayment = 0,
			InReview = 1,
			NotPublished = 2,
			Published = 3,
			Hidden = 4
		}

		[DisplayName("Name")]
		[Required(ErrorMessage = "NameIsRequired")]
		[StringLength(32, MinimumLength = 2, ErrorMessage = "NameLengthErrorMessage")]
		public string Name { get; set; }
		[Required(ErrorMessage = "TypeIsRequired")]
		public PropertyTypeDTO Type { get; set; }
		[Required(ErrorMessage = "AddressIsRequired")]
		[StringLength(32, MinimumLength = 4, ErrorMessage = "AddressLengthErrorMessage")]
		[RegularExpression(@"^[\p{L}\p{N} :,°'#.;]{4,32}$", ErrorMessage = "AddressFormatErrorMessage")]
		public string Address { get; set; }
		[Required(ErrorMessage = "DistrictIsRequired")]
		public DistrictDTO District { get; set; }
		[Required(ErrorMessage = "ProvinceIsRequired")]
		public ProvinceDTO Province
		{
			get => District.Province;
			set => District.Province = value;
		}
		[Required(ErrorMessage = "RegionIsRequired")]
		public RegionDTO Region
		{
			get => Province.Region;
			set => Province.Region = value;
		}
		[Required(ErrorMessage = "CountryIsRequired")]
		public CountryDTO Country { get; set; }
		public PartnerDTO Partner { get; set; }
		public PublishModeDTO PublishMode { get; set; }
		public bool HasBeenPaid { get; set; }
		public bool HasBeenReviewed { get; set; }
		public bool HasBeenPublished { get; set; }
		public bool IsActive { get; set; }

		public IEnumerable<PropertyFeatureDetailDTO> Features { get; set; }
		public IEnumerable<PropertyPictureDTO> Pictures { get; set; }

		public PropertyStatus Status
		{
			get
			{
				if (HasBeenPaid)
				{
					if (HasBeenReviewed) return HasBeenPublished ? PropertyStatus.Published : PropertyStatus.NotPublished;
					else return PropertyStatus.InReview;
				}
				else return PropertyStatus.WaitingForPayment;
			}
		}
	}
}