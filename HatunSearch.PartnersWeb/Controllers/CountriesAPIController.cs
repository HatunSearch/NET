// Hatun Search | Layer: PartnersWeb || Version: 2018.11.16.810
// (c) 2018 Hatun Search. All rights reserved.

// 'Using' directive
using HatunSearch.Business;
using HatunSearch.Entities;
using System.Collections.Generic;
using System.Web.Mvc;

namespace HatunSearch.PartnersWeb.Controllers
{
	[RoutePrefix("api/countries")]
	public sealed class CountriesAPIController : JsonBasedController
	{
		[HttpGet]
		[Route("")]
		public ActionResult Get()
		{
			CountryBLL countryBLL = new CountryBLL(WebApp.Connector);
			IEnumerable<CountryDTO> countries = countryBLL.ReadAll();
			return Json(countries);
		}
		[HttpGet]
		[Route("{countryId}/regions/{regionId}/provinces/{provinceId}/districts")]
		public ActionResult GetDistricts(string countryId, string regionId, string provinceId)
		{
			DistrictBLL districtBLL = new DistrictBLL(WebApp.Connector);
			IEnumerable<DistrictDTO> districts = districtBLL.ReadByCountryAndRegionAndProvince(countryId, regionId, provinceId);
			return Json(districts);
		}
		[HttpGet]
		[Route("{countryId}/regions/{regionId}/provinces")]
		public ActionResult GetProvinces(string countryId, string regionId)
		{
			ProvinceBLL provinceBLL = new ProvinceBLL(WebApp.Connector);
			IEnumerable<ProvinceDTO> provinces = provinceBLL.ReadByCountryAndRegion(countryId, regionId);
			return Json(provinces);
		}
		[HttpGet]
		[Route("{id}/regions")]
		public ActionResult GetRegions(string id)
		{
			RegionBLL regionBLL = new RegionBLL(WebApp.Connector);
			IEnumerable<RegionDTO> regions = regionBLL.ReadByCountry(id);
			return Json(regions);
		}
	}
}