// Hatun Search | Layer: PartnersWeb || Version: 2018.11.16.810
// (c) 2018 Hatun Search. All rights reserved.

// 'Using' directive
using HatunSearch.Business;
using HatunSearch.Business.Helpers;
using HatunSearch.Data.Databases;
using HatunSearch.Entities;
using HatunSearch.Entities.Security;
using System;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace HatunSearch.PartnersWeb.Controllers
{
	[RoutePrefix("management/account")]
	public sealed class ManagementAccountController : ManagementBaseController
	{
		public ManagementAccountController() : base("ManagementAccount") { }

		public sealed class AddCardDTO
		{
			[Required(ErrorMessage = "YouShouldEnterYourCreditCardInformation")]
			public string TokenId { get; set; }
		}
		public sealed class EditCredentialsDTO
		{
			[DataType(DataType.Password)]
			[Required(ErrorMessage = "PasswordIsRequired")]
			[StringLength(64, MinimumLength = 8, ErrorMessage = "PasswordLengthErrorMessage")]
			public string CurrentPassword { get; set; }
			[DataType(DataType.Password)]
			[Required(ErrorMessage = "PasswordIsRequired")]
			[StringLength(64, MinimumLength = 8, ErrorMessage = "PasswordLengthErrorMessage")]
			public string NewPassword { get; set; }
		}

		[HttpGet]
		[Route("mycards/add")]
		public ActionResult AddCard() => View();
		[HttpPost]
		[Route("mycards/add")]
		public ActionResult AddCard(AddCardDTO request)
		{
			if (ModelState.IsValid)
			{
				PartnerBLL partnerBLL = new PartnerBLL(WebApp.Connector);
				PartnerBLL.AddCardResult result = partnerBLL.AddCard(Account, request.TokenId);
				switch (result)
				{
					case PartnerBLL.AddCardResult.OK:
						TempData["Result"] = "CardHasBeenAdded";
						return RedirectToAction("MyCards");
					case PartnerBLL.AddCardResult.CardIsNotCredit:
					case PartnerBLL.AddCardResult.CardHasAlreadyBeenAdded:
					case PartnerBLL.AddCardResult.MaximumAmountOfCardsReached:
						AddError("TokenId", result.ToString());
						return View();
					default: return BadRequest();
				}
			}
			else return BadRequestWithErrors();
		}
		[AcceptVerbs(HttpVerbs.Get | HttpVerbs.Post)]
		[Route("mycards/delete/{id}")]
		public ActionResult DeleteCard(Guid id)
		{
			PartnerBLL partnerBLL = new PartnerBLL(WebApp.Connector);
			if (Request.HttpMethod == "GET")
			{
				PartnerCardDTO card = partnerBLL.ReadCardById(Account, id);
				return card != null ? View(card) as ActionResult : HttpNotFound();
			}
			else
			{
				PartnerBLL.DeleteCardResult result = partnerBLL.DeleteCard(Account, id);
				switch (result)
				{
					case PartnerBLL.DeleteCardResult.OK:
						TempData["Result"] = "CardHasBeenDeleted";
						return RedirectToAction("MyCards");
					case PartnerBLL.DeleteCardResult.NotFound: return HttpNotFound();
					default: return BadRequest();
				}
			}
		}
		[HttpGet]
		[Route("edit/credentials")]
		public ActionResult EditCredentials() => View();
		[HttpPost]
		[Route("edit/credentials")]
		public ActionResult EditCredentials(EditCredentialsDTO request)
		{
			if (ModelState.IsValid)
			{
				if (request.CurrentPassword != request.NewPassword)
				{
					if (BinaryComparer.AreEqual(Account.Password, SHA512Hasher.Hash(request.CurrentPassword)))
					{
						PartnerBLL partnerBLL = new PartnerBLL(WebApp.Connector);
						Uri requestUrl = Request.Url;
						string baseUrl = new UriBuilder(requestUrl.Scheme, requestUrl.Host, requestUrl.Port).ToString();
						partnerBLL.ChangePasswordEmailSubject = LocalizationProvider["ChangePasswordEmailSubject"];
						partnerBLL.ChangePasswordEmailTemplate = LocalizationProvider["ChangePasswordEmailTemplate"];
						partnerBLL.ChangePassword(Account, request.NewPassword, baseUrl);
						TempData["Result"] = "PasswordHasBeenChanged";
						return RedirectToAction("MyProfile");
					}
					else
					{
						AddError("CurrentPassword", "CurrentPasswordDoesntMatch");
						return View();
					}
				}
				else
				{
					AddError("NewPassword", "NewAndCurrentPasswordAreTheSame");
					return View();
				}
			}
			else return BadRequestWithErrors();
		}
		[HttpGet]
		[Route("edit/companyinfo")]
		public ActionResult EditCompanyInfo()
		{
			EditCompanyInfo_Base();
			return View(Account.CompanyInfo);
		}
		[HttpPost]
		[Route("edit/companyinfo")]
		public ActionResult EditCompanyInfo(PartnerCompanyInfoDTO companyInfo)
		{
			if (ModelState.IsValid)
			{
				PartnerBLL partnerBLL = new PartnerBLL(WebApp.Connector);
				partnerBLL.UpdateCompanyInfo(Account.Id, companyInfo);
				TempData["Result"] = "CompanyInfoHasBeenUpdated";
				return RedirectToAction("MyProfile");
			}
			else
			{
				EditCompanyInfo_Base();
				return BadRequestWithErrors(companyInfo);
			}
		}
		private void EditCompanyInfo_Base() => EditCompanyInfo_Base(WebApp.Connector);
		private void EditCompanyInfo_Base(Connector connector)
		{
			CountryBLL countryBLL = new CountryBLL(connector);
			ViewBag.Countries = countryBLL.ReadAll();
		}
		[HttpGet]
		[Route("edit/personalinfo")]
		public ActionResult EditPersonalInfo()
		{
			EditPersonalInfo_Base();
			return View(Account.PersonalInfo);
		}
		[HttpPost]
		[Route("edit/personalinfo")]
		public ActionResult EditPersonalInfo(PartnerPersonalInfoDTO personalInfo)
		{
			Connector connector = WebApp.Connector;
			if (ModelState.IsValid)
			{
				PartnerBLL partnerBLL = new PartnerBLL(connector)
				{
					EmailAddressVerificationSubject = LocalizationProvider["EmailVerificationSubject"],
					EmailAddressVerificationTemplate = LocalizationProvider["EmailVerificationTemplate"]
				};
				string baseUrl = new UriBuilder(Request.Url.Scheme, Request.Url.Host, Request.Url.Port).ToString();
				PartnerBLL.UpdatePersonalInfoResult result = partnerBLL.UpdatePersonalInfo(Account.Id, personalInfo, baseUrl, Url.Action("VerifyEmailAddress", "Accounts"));
				switch (result)
				{
					case PartnerBLL.UpdatePersonalInfoResult.OK:
						TempData["Result"] = "PersonalInfoHasBeenUpdated";
						return RedirectToAction("MyProfile");
					case PartnerBLL.UpdatePersonalInfoResult.EmailAddressAlreadyUsed:
						AddError("EmailAddress", result.ToString());
						EditPersonalInfo_Base(connector);
						return View(personalInfo);
					default: return BadRequest();
				}
			}
			else
			{
				EditPersonalInfo_Base(connector);
				return BadRequestWithErrors(personalInfo);
			}
		}
		private void EditPersonalInfo_Base() => EditPersonalInfo_Base(WebApp.Connector);
		private void EditPersonalInfo_Base(Connector connector)
		{
			GenderBLL genderBLL = new GenderBLL(connector);
			ViewBag.Genders = genderBLL.ReadAll();
		}
		[HttpGet]
		[Route("edit/preferences")]
		public ActionResult EditPreferences()
		{
			EditPreferences_Base();
			return View(Account.Preferences);
		}
		[HttpPost]
		[Route("edit/preferences")]
		public ActionResult EditPreferences(PartnerPreferencesDTO preferences)
		{
			if (ModelState.IsValid)
			{
				PartnerBLL partnerBLL = new PartnerBLL(WebApp.Connector);
				partnerBLL.UpdatePreferences(Account.Id, preferences);
				TempData["Result"] = "PreferencesHaveBeenUpdated";
				return RedirectToAction("MyProfile");
			}
			else
			{
				EditPreferences_Base();
				return BadRequestWithErrors(preferences);
			}
		}
		private void EditPreferences_Base()
		{
			CountryDTO country = Account.Country;
			ViewBag.Currencies = country.SupportedCurrencies;
			ViewBag.Languages = country.SupportedLanguages;
		}
		[HttpGet]
		[Route("mycards")]
		public ActionResult MyCards() => View();
		[HttpGet]
		[Route("myinvoices")]
		public ActionResult MyInvoices() => View();
		[HttpGet]
		[Route("myprofile")]
		public ActionResult MyProfile() => View();
		[HttpGet]
		[Route("myinvoices/{id}")]
		public ActionResult ViewInvoice(Guid id)
		{
			PartnerBLL partnerBLL = new PartnerBLL(WebApp.Connector);
			PartnerInvoiceDTO invoice = partnerBLL.ReadInvoiceById(Account, id);
			return invoice != null ? View(invoice) as ActionResult : HttpNotFound();
		}
	}
}