// Hatun Search | Layer: PartnersWeb || Version: 2018.11.16.810
// (c) 2018 Hatun Search. All rights reserved.

// 'Using' directive
using HatunSearch.Business;
using HatunSearch.Data.Databases;
using HatunSearch.Entities;
using System;
using System.ComponentModel.DataAnnotations;
using System.Web;
using System.Web.Hosting;
using System.Web.Mvc;

namespace HatunSearch.PartnersWeb.Controllers
{
	[RoutePrefix("management/sales")]
	public sealed class ManagementSalesController : ManagementBaseController
	{
		public ManagementSalesController() : base("ManagementSales") { }

		public sealed class AddPropertyPictureDTO
		{
			[Required(ErrorMessage = "PictureIsRequired")]
			public HttpPostedFileBase Picture { get; set; }
			[Required(ErrorMessage = "DescriptionIsRequired")]
			[StringLength(32, MinimumLength = 1, ErrorMessage = "DescriptionLengthErrorMessage")]
			public string Description { get; set; }
		}
		public sealed class AnswerQuestionDTO
		{
			[Required(ErrorMessage = "AnswerIsRequired")]
			[StringLength(16384, MinimumLength = 2, ErrorMessage = "AnswerLengthErrorMessage")]
			public string Answer { get; set; }
		}
		public sealed class PublishPropertyDTO
		{
			[Required(ErrorMessage = "PublishModeIsRequired")]
			public PublishModeDTO PublishMode { get; set; }
			[Required(ErrorMessage = "PaymentMethodIsRequired")]
			public PartnerCardDTO PaymentMethod { get; set; }
		}

		private void AddEditProperty_BaseGet() => AddEditProperty_BaseGet(WebApp.Connector);
		private void AddEditProperty_BaseGet(Connector connector)
		{
			CountryBLL countryBLL = new CountryBLL(connector);
			PropertyTypeBLL propertyTypeBLL = new PropertyTypeBLL(connector);
			ViewBag.Countries = countryBLL.ReadAll();
			ViewBag.Types = propertyTypeBLL.ReadAll();
		}
		private void AddEditProperty_BasePost(Connector connector, PropertyDTO property)
		{
			RegionBLL regionBLL = new RegionBLL(connector);
			ProvinceBLL provinceBLL = new ProvinceBLL(connector);
			DistrictBLL districtBLL = new DistrictBLL(connector);
			CountryDTO country = property.Country;
			RegionDTO region = property.Region;
			ProvinceDTO province = property.Province;
			DistrictDTO district = property.District;
			if (country != null)
			{
				country.Regions = regionBLL.ReadByCountry(country.Id);
				if (region != null) region.Country = country;
				if (province != null) province.Country = country;
				if (district != null) district.Country = country;
			}
			if (region != null) region.Provinces = provinceBLL.ReadByCountryAndRegion(country.Id, region.Code);
			if (province != null) province.Districts = districtBLL.ReadByCountryAndRegionAndProvince(country.Id, region.Code, province.Code);
		}
		private PropertyDTO AddEditPropertyFeature_Base(Guid propertyId)
		{
			PropertyBLL propertyBLL = new PropertyBLL(WebApp.Connector);
			PropertyFeatureBLL propertyFeatureBLL = new PropertyFeatureBLL(WebApp.Connector);
			PropertyDTO property = propertyBLL.ReadById(propertyId);
			ViewBag.Features = propertyFeatureBLL.ReadAll();
			return property;
		}
		[HttpGet]
		[Route("properties/add")]
		public ActionResult AddProperty()
		{
			AddEditProperty_BaseGet();
			return View();
		}
		[HttpPost]
		[Route("properties/add")]
		public ActionResult AddProperty(PropertyDTO property)
		{
			Connector connector = WebApp.Connector;
			AddEditProperty_BasePost(connector, property);
			if (ModelState.IsValid)
			{
				PartnerBLL partnerBLL = new PartnerBLL(connector);
				PartnerBLL.AddPropertyResult result = partnerBLL.AddProperty(Account, property);
				switch (result)
				{
					case PartnerBLL.AddPropertyResult.OK:
						TempData["Result"] = "PropertyHasBeenAdded";
						return RedirectToAction("Properties");
					default: return BadRequest();
				}
			}
			else
			{
				AddEditProperty_BaseGet(connector);
				return BadRequestWithErrors(property);
			}
		}
		[HttpGet]
		[Route("properties/{id}/features/add")]
		public ActionResult AddPropertyFeature(Guid id) => AddEditPropertyFeature_Base(id) != null ? View() as ActionResult : HttpNotFound();
		[HttpPost]
		[Route("properties/{id}/features/add")]
		public ActionResult AddPropertyFeature(Guid id, PropertyFeatureDetailDTO featureDetail)
		{
			if (ModelState.IsValid)
			{
				PartnerBLL partnerBLL = new PartnerBLL(WebApp.Connector);
				PartnerBLL.AddPropertyFeatureDetailResult result = partnerBLL.AddPropertyFeatureDetail(Account, id, featureDetail);
				switch (result)
				{
					case PartnerBLL.AddPropertyFeatureDetailResult.OK:
						TempData["Result"] = "PropertyFeatureDetailHasBeenAdded";
						return RedirectToAction("ViewProperty");
					case PartnerBLL.AddPropertyFeatureDetailResult.NotFound: return HttpNotFound();
					case PartnerBLL.AddPropertyFeatureDetailResult.FeatureAlreadyAdded:
						AddError("Feature", "FeatureAlreadyAdded");
						AddEditPropertyFeature_Base(id);
						return View(featureDetail);
					default: return BadRequest();
				}
			}
			else return AddEditPropertyFeature_Base(id) != null ? BadRequestWithErrors() as ActionResult : HttpNotFound();
		}
		[HttpGet]
		[Route("properties/{id}/pictures/add")]
		public ActionResult AddPropertyPicture(Guid id)
		{
			PropertyDTO property = AddEditPropertyPicture_Base(id);
			return property != null ? View() as ActionResult : HttpNotFound();
		}
		[HttpPost]
		[Route("properties/{id}/pictures/add")]
		public ActionResult AddPropertyPicture(Guid id, AddPropertyPictureDTO request)
		{
			PartnerBLL partnerBLL = new PartnerBLL(WebApp.Connector);
			PropertyDTO property = AddEditPropertyPicture_Base(partnerBLL, id);
			if (ModelState.IsValid)
			{
				PropertyPictureDTO picture = new PropertyPictureDTO()
				{
					Property = property,
					Description = request.Description
				};
				PartnerBLL.AddPropertyPictureResult result = partnerBLL.AddPropertyPicture(Account, id, picture);
				switch (result)
				{
					case PartnerBLL.AddPropertyPictureResult.OK:
						string imgRepository = HostingEnvironment.MapPath("~/img");
						request.Picture.SaveAs($"{imgRepository}/{picture.Id}.bin");
						TempData["Result"] = "PropertyPictureHasBeenAdded";
						return RedirectToAction("ViewProperty");
					case PartnerBLL.AddPropertyPictureResult.NotFound: return HttpNotFound();
					default: return BadRequest();
				}
			}
			else return property != null ? BadRequestWithErrors() as ActionResult : HttpNotFound();
		}
		private PropertyDTO AddEditPropertyPicture_Base(Guid id) => AddEditPropertyPicture_Base(new PartnerBLL(WebApp.Connector), id);
		private PropertyDTO AddEditPropertyPicture_Base(PartnerBLL partnerBLL, Guid id)
		{
			PropertyDTO property = partnerBLL.ReadPropertyById(Account, id);
			ViewBag.PropertyId = id;
			return property;
		}
		[HttpGet]
		[Route("customer/questions/{id}/answer")]
		public ActionResult AnswerQuestion(Guid id)
		{
			bool result = AnswerQuestion_Base(id);
			return result ? View() as ActionResult : HttpNotFound();
		}
		[HttpPost]
		[Route("customer/questions/{id}/answer")]
		public ActionResult AnswerQuestion(Guid id, AnswerQuestionDTO request)
		{
			if (ModelState.IsValid)
			{
				PartnerBLL partnerBLL = new PartnerBLL(WebApp.Connector)
				{
					AnswerQuestionEmailSubject = LocalizationProvider["AnswerQuestionEmailSubject"],
					AnswerQuestionEmailTemplate = LocalizationProvider["AnswerQuestionEmailTemplate"]
				};
				string baseUrl = new UriBuilder(Request.Url.Scheme, Request.Url.Host, Request.Url.Port).ToString();
				PartnerBLL.AnswerQuestionResult result = partnerBLL.AnswerQuestion(Account, id, request.Answer, baseUrl);
				switch (result)
				{
					case PartnerBLL.AnswerQuestionResult.OK:
						TempData["Result"] = "QuestionHasBeenAnswered";
						return RedirectToAction("ViewCustomerQuestion");
					case PartnerBLL.AnswerQuestionResult.NotFound: return HttpNotFound();
					case PartnerBLL.AnswerQuestionResult.QuestionHasBeenAlreadyAnswered:
					default:
						return BadRequest();
				}
			}
			else
			{
				bool result = AnswerQuestion_Base(id);
				return result ? BadRequestWithErrors() as ActionResult : HttpNotFound();
			}
		}
		private bool AnswerQuestion_Base(Guid id)
		{
			PartnerBLL partnerBLL = new PartnerBLL(WebApp.Connector);
			CustomerQuestionDTO question = partnerBLL.ReadCustomerQuestionById(Account, id);
			bool isAnswerable = question != null && question.Answer == null;
			if (isAnswerable) ViewBag.Question = question;
			return isAnswerable;
		}
		[HttpGet]
		[Route("customers/questions")]
		public ActionResult CustomersQuestions() => View();
		[AcceptVerbs(HttpVerbs.Get | HttpVerbs.Post)]
		[Route("properties/{id}/delete")]
		public ActionResult DeleteProperty(Guid id)
		{
			PartnerBLL partnerBLL = new PartnerBLL(WebApp.Connector);
			if (Request.HttpMethod == "GET")
			{
				PropertyDTO property = partnerBLL.ReadPropertyById(Account, id);
				if (property != null && (!property.HasBeenPaid || (property.HasBeenReviewed && !property.HasBeenPublished))) return View(property);
				else return HttpNotFound();
			}
			else
			{
				PartnerBLL.DeletePropertyResult result = partnerBLL.DeleteProperty(Account, id);
				switch (result)
				{
					case PartnerBLL.DeletePropertyResult.OK:
						TempData["Result"] = "PropertyHasBeenDeleted";
						return RedirectToAction("Properties");
					case PartnerBLL.DeletePropertyResult.NotFound: return HttpNotFound();
					default: return BadRequest();
				}
			}
		}
		[AcceptVerbs(HttpVerbs.Get | HttpVerbs.Post)]
		[Route("properties/{propertyId}/features/delete/{featureId}")]
		public ActionResult DeletePropertyFeature(Guid propertyId, byte featureId)
		{
			PartnerBLL partnerBLL = new PartnerBLL(WebApp.Connector);
			if (Request.HttpMethod == "GET")
			{
				PropertyFeatureDetailDTO featureDetail = partnerBLL.ReadPropertyFeatureDetailById(Account, propertyId, featureId);
				if (featureDetail != null)
				{
					ViewBag.PropertyId = propertyId;
					return View(featureDetail);
				}
				else return HttpNotFound();
			}
			else
			{
				PartnerBLL.DeletePropertyFeatureDetailResult result = partnerBLL.DeletePropertyFeatureDetail(Account, propertyId, featureId);
				switch (result)
				{
					case PartnerBLL.DeletePropertyFeatureDetailResult.OK:
						TempData["Result"] = "PropertyFeatureDetailHasBeenDeleted";
						return RedirectToAction("ViewProperty", new { id = propertyId });
					case PartnerBLL.DeletePropertyFeatureDetailResult.NotFound: return HttpNotFound();
					default: return BadRequest();
				}
			}
		}
		[AcceptVerbs(HttpVerbs.Get | HttpVerbs.Post)]
		[Route("properties/{propertyId}/pictures/{pictureId}/delete")]
		public ActionResult DeletePropertyPicture(Guid propertyId, Guid pictureId)
		{
			PartnerBLL partnerBLL = new PartnerBLL(WebApp.Connector);
			if (Request.HttpMethod == "GET")
			{
				PropertyPictureDTO picture = partnerBLL.ReadPropertyPictureById(Account, propertyId, pictureId);
				if (picture != null)
				{
					ViewBag.PropertyId = propertyId;
					return View(picture);
				}
				else return HttpNotFound();
			}
			else
			{
				PartnerBLL.DeletePropertyPictureResult result = partnerBLL.DeletePropertyPicture(Account, propertyId, pictureId);
				switch (result)
				{
					case PartnerBLL.DeletePropertyPictureResult.OK:
						TempData["Result"] = "PropertyPictureHasBeenDeleted";
						HostingEnvironment.MapPath($"~/img/{pictureId}.img");
						return RedirectToAction("ViewProperty", new { id = propertyId });
					case PartnerBLL.DeletePropertyPictureResult.NotFound: return HttpNotFound();
					default: return BadRequest();
				}
			}
		}
		[HttpGet]
		[Route("properties/{id}/edit")]
		public ActionResult EditProperty(Guid id)
		{
			Connector connector = WebApp.Connector;
			PartnerBLL partnerBLL = new PartnerBLL(connector);
			PropertyDTO property = partnerBLL.ReadPropertyById(Account, id);
			if (property != null && (!property.HasBeenPaid || (property.HasBeenReviewed && !property.HasBeenPublished)))
			{
				AddEditProperty_BaseGet(connector);
				return View(property);
			}
			else return HttpNotFound();
		}
		[HttpPost]
		[Route("properties/{id}/edit")]
		public ActionResult EditProperty(Guid id, PropertyDTO property)
		{
			Connector connector = WebApp.Connector;
			AddEditProperty_BasePost(connector, property);
			if (ModelState.IsValid)
			{
				PartnerBLL partnerBLL = new PartnerBLL(connector);
				PartnerBLL.UpdatePropertyResult result = partnerBLL.UpdateProperty(Account, id, property);
				switch (result)
				{
					case PartnerBLL.UpdatePropertyResult.OK:
						TempData["Result"] = "PropertyHasBeenEdited";
						return RedirectToAction("ViewProperty", new { id });
					case PartnerBLL.UpdatePropertyResult.NotFound: return HttpNotFound();
					default: return BadRequest();
				}
			}
			else
			{
				AddEditProperty_BaseGet(connector);
				return BadRequestWithErrors(property);
			}
		}
		[HttpGet]
		[Route("properties/{propertyId}/features/edit/{featureId}")]
		public ActionResult EditPropertyFeature(Guid propertyId, byte featureId)
		{
			PartnerBLL partnerBLL = new PartnerBLL(WebApp.Connector);
			PropertyFeatureDetailDTO featureDetail = partnerBLL.ReadPropertyFeatureDetailById(Account, propertyId, featureId);
			EditPropertyFeature_Base(propertyId);
			return View(featureDetail);
		}
		[HttpPost]
		[Route("properties/{propertyId}/features/edit/{featureId}")]
		public ActionResult EditPropertyFeature(Guid propertyId, byte featureId, PropertyFeatureDetailDTO featureDetail)
		{
			if (ModelState.IsValid)
			{
				PartnerBLL partnerBLL = new PartnerBLL(WebApp.Connector);
				PartnerBLL.UpdatePropertyFeatureDetailResult result = partnerBLL.UpdatePropertyFeature(Account, propertyId, featureId, featureDetail);
				switch (result)
				{
					case PartnerBLL.UpdatePropertyFeatureDetailResult.OK:
						TempData["Result"] = "PropertyFeatureDetailHasBeenEdited";
						return RedirectToAction("ViewProperty", new { id = propertyId });
					case PartnerBLL.UpdatePropertyFeatureDetailResult.NotFound: return HttpNotFound();
					case PartnerBLL.UpdatePropertyFeatureDetailResult.FeatureAlreadyAdded:
						AddError("Feature", "FeatureAlreadyAdded");
						EditPropertyFeature_Base(propertyId);
						return View(featureDetail);
					default: return BadRequest();
				}
			}
			else
			{
				EditPropertyFeature_Base(propertyId);
				return BadRequestWithErrors();
			}
		}
		private void EditPropertyFeature_Base(Guid propertyId)
		{
			AddEditPropertyFeature_Base(propertyId);
			ViewBag.PropertyId = propertyId;
		}
		[HttpGet]
		[Route("properties/{propertyId}/pictures/{pictureId}/edit")]
		public ActionResult EditPropertyPicture(Guid propertyId, Guid pictureId)
		{
			PartnerBLL partnerBLL = new PartnerBLL(WebApp.Connector);
			PropertyDTO property = AddEditPropertyPicture_Base(partnerBLL, propertyId);
			if (property != null)
			{
				PropertyPictureDTO picture = partnerBLL.ReadPropertyPictureById(Account, propertyId, pictureId);
				return picture != null ? View(picture) as ActionResult : HttpNotFound();
			}
			else return HttpNotFound();
		}
		[HttpPost]
		[Route("properties/{propertyId}/pictures/{pictureId}/edit")]
		public ActionResult EditPropertyPicture(Guid propertyId, Guid pictureId, PropertyPictureDTO picture)
		{
			PartnerBLL partnerBLL = new PartnerBLL(WebApp.Connector);
			PropertyDTO property = AddEditPropertyPicture_Base(partnerBLL, propertyId);
			if (ModelState.IsValid)
			{
				PartnerBLL.UpdatePropertyPictureResult result = partnerBLL.UpdatePropertyPicture(Account, propertyId, pictureId, picture);
				switch (result)
				{
					case PartnerBLL.UpdatePropertyPictureResult.OK:
						TempData["Result"] = "PropertyPictureHasBeenEdited";
						return RedirectToAction("ViewProperty", new { id = propertyId });
					case PartnerBLL.UpdatePropertyPictureResult.NotFound: return HttpNotFound();
					default: return BadRequest();
				}
			}
			else return property != null ? BadRequestWithErrors() as ActionResult : HttpNotFound();
		}
		[HttpGet]
		[Route("properties")]
		public ActionResult Properties() => View();
		[HttpGet]
		[Route("properties/{id}/publish")]
		public ActionResult PublishProperty(Guid id)
		{
			bool result = PublishProperty_Base(id);
			return result ? View() as ActionResult : HttpNotFound();
		}
		[HttpPost]
		[Route("properties/{id}/publish")]
		public ActionResult PublishProperty(Guid id, PublishPropertyDTO request)
		{
			Connector connector = WebApp.Connector;
			if (ModelState.IsValid)
			{
				PartnerBLL partnerBLL = new PartnerBLL(connector)
				{
					PublishPropertyEmailSubject = LocalizationProvider["PublishPropertyEmailSubject"],
					PublishPropertyEmailTemplate = LocalizationProvider["PublishPropertyEmailTemplate"]
				};
				string baseUrl = new UriBuilder(Request.Url.Scheme, Request.Url.Host, Request.Url.Port).ToString();
				PartnerBLL.PublishPropertyResult result = partnerBLL.PublishProperty(Account, id, request.PaymentMethod, request.PublishMode, baseUrl);
				switch (result)
				{
					case PartnerBLL.PublishPropertyResult.OK:
						TempData["Result"] = "PropertyHasBeenPaid";
						return RedirectToAction("ViewProperty");
					case PartnerBLL.PublishPropertyResult.NotFound: return HttpNotFound();
					case PartnerBLL.PublishPropertyResult.PaymentHasBeenRejected:
						AddError("PaymentMethod", result.ToString());
						PublishProperty_Base(connector, id);
						return View(request);
					default: return BadRequest();
				}
			}
			else
			{
				PublishProperty_Base(connector, id);
				return BadRequestWithErrors();
			}
		}
		private bool PublishProperty_Base(Guid propertyId) => PublishProperty_Base(WebApp.Connector, propertyId);
		private bool PublishProperty_Base(Connector connector, Guid propertyId)
		{
			PartnerBLL partnerBLL = new PartnerBLL(connector);
			CurrencyExchangeBLL currencyExchangeBLL = new CurrencyExchangeBLL(connector);
			PublishModeBLL publishModeBLL = new PublishModeBLL(connector);
			PropertyDTO property = partnerBLL.ReadPropertyById(Account, propertyId);
			bool result = property != null && !property.HasBeenPaid;
			if (result)
			{
				ViewBag.CurrenciesExchanges = currencyExchangeBLL.ReadByTo(Account.PreferredCurrency.Id);
				ViewBag.PublishModes = publishModeBLL.ReadAll();
				ViewBag.Property = property;
			}
			return result;
		}
		[HttpGet]
		[Route("customer/questions/{id}")]
		public ActionResult ViewCustomerQuestion(Guid id)
		{
			PartnerBLL partnerBLL = new PartnerBLL(WebApp.Connector);
			CustomerQuestionDTO question = partnerBLL.ReadCustomerQuestionById(Account, id);
			if (question != null)
			{
				if (!question.HasQuestionBeenRead) partnerBLL.MarkQuestionAsRead(Account, id);
				return View(question);
			}
			else return HttpNotFound();
		}
		[HttpGet]
		[Route("properties/{id}")]
		public ActionResult ViewProperty(Guid id)
		{
			PartnerBLL partnerBLL = new PartnerBLL(WebApp.Connector);
			PropertyDTO property = partnerBLL.ReadPropertyById(Account, id);
			return property != null ? View(property) as ActionResult : HttpNotFound();
		}
	}
}