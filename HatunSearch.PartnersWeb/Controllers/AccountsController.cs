// Hatun Search | Layer: PartnersWeb || Version: 2018.11.16.810
// (c) 2018 Hatun Search. All rights reserved.

// 'Using' directive
using HatunSearch.Business;
using HatunSearch.Data.Databases;
using HatunSearch.Data.Helpers;
using HatunSearch.Entities;
using System;
using System.Collections.Generic;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace HatunSearch.PartnersWeb.Controllers
{
	[RoutePrefix("accounts")]
	public sealed class AccountsController : HybridBaseController
	{
		public AccountsController() : base("Accounts") { }

		protected override void OnActionExecuting(ActionExecutingContext filterContext)
		{
			base.OnActionExecuting(filterContext);
			string actionName = filterContext.ActionDescriptor.ActionName;
			HttpCookie cookie = Request.Cookies["Session"];
			if (cookie != null)
			{
				string cookieValue = cookie.Value;
				PartnerSessionDTO session = null;
				try
				{
					byte[] sessionId = FormatHelper.FromHexStringToArray(cookieValue);
					PartnerSessionBLL sessionBLL = new PartnerSessionBLL(WebApp.Connector);
					session = sessionBLL.ReadById(sessionId);
				}
				catch { }
				DateTime? expiresOn = session?.ExpiresOn;
				if (session != null && ((expiresOn != null && expiresOn > DateTime.UtcNow) || expiresOn == null) && session.IsActive && session.Partner.HasEmailAddressBeenVerified)
				{
					if (actionName != "Logout") filterContext.Result = RedirectToAction("Home", "Management");
				}
				else if (actionName == "Logout") filterContext.Result = RedirectToAction("Login");
			}
			TempData.Clear();
		}

		[HttpGet]
		[Route("login")]
		public ActionResult Login() => View();
		[HttpPost]
		[Route("login")]
		public ActionResult Login(PartnerCredentialDTO credential)
		{
			if (ModelState.IsValid)
			{
				PartnerSessionBLL sessionBLL = new PartnerSessionBLL(WebApp.Connector);
				PartnerSessionBLL.LoginResult result = sessionBLL.Login(credential, IPAddress.Parse(Request.UserHostAddress), false, out PartnerSessionDTO session);
				switch (result)
				{
					case PartnerSessionBLL.LoginResult.OK:
						Session.Abandon();
						Response.Cookies.Add(new HttpCookie("Session", FormatHelper.FromArrayToHexString(session.Id)));
						return RedirectToAction("Home", "Management");
					case PartnerSessionBLL.LoginResult.AccountDoesntExist:
					case PartnerSessionBLL.LoginResult.EmailAddressHasNotBeenVerified:
					case PartnerSessionBLL.LoginResult.AccountIsLocked:
						AddError("Username", result.ToString());
						return View(credential);
					case PartnerSessionBLL.LoginResult.PasswordDoesntMatch:
						AddError("Password", result.ToString());
						return View(credential);
					default: return BadRequest();
				}
			}
			else return BadRequestWithErrors(credential);
		}
		[HttpGet]
		[Route("logout")]
		public ActionResult Logout()
		{
			try
			{
				HttpCookie cookie = Request.Cookies["Session"];
				if (cookie != null)
				{
					string cookieValue = cookie.Value;
					byte[] sessionId = FormatHelper.FromHexStringToArray(cookieValue);
					PartnerSessionBLL sessionBLL = new PartnerSessionBLL(WebApp.Connector);
					cookie.Expires = DateTime.UtcNow;
					Response.SetCookie(cookie);
					return sessionBLL.Logout(sessionId) == PartnerSessionBLL.LogoutResult.OK ? View() as ActionResult : RedirectToAction("Login");
				}
				else return RedirectToAction("Login");
			}
			catch { return RedirectToAction("Login"); }
		}
		[HttpGet]
		[Route("signup")]
		public ActionResult Signup() => RedirectToAction("SignupStep1");
		[HttpGet]
		[Route("signup/step1")]
		public ActionResult SignupStep1()
		{
			PartnerCredentialDTO credential = Session["Signup$credential"] as PartnerCredentialDTO;
			return View(credential);
		}
		[HttpPost]
		[Route("signup/step1")]
		public ActionResult SignupStep1(PartnerCredentialDTO credential)
		{
			if (ModelState.IsValid)
			{
				Session["Signup$Credential"] = credential;
				return RedirectToAction("SignupStep2");
			}
			else return BadRequestWithErrors(credential);
		}
		[HttpGet]
		[Route("signup/step2")]
		public ActionResult SignupStep2()
		{
			if (Session["Signup$Credential"] is PartnerCredentialDTO)
			{
				PartnerPersonalInfoDTO companyInfo = Session["Signup$PersonalInfo"] as PartnerPersonalInfoDTO;
				SignupStep2_Base();
				return View(companyInfo);
			}
			else return RedirectToAction("SignupStep1");
		}
		[HttpPost]
		[Route("signup/step2")]
		public ActionResult SignupStep2(PartnerPersonalInfoDTO personalInfo)
		{
			if (ModelState.IsValid)
			{
				Session["Signup$PersonalInfo"] = personalInfo;
				return RedirectToAction("SignupStep3");
			}
			else
			{
				SignupStep2_Base();
				return BadRequestWithErrors( personalInfo);
			}
		}
		private void SignupStep2_Base()
		{
			GenderBLL genderBLL = new GenderBLL(WebApp.Connector);
			ViewBag.Genders = genderBLL.ReadAll();
		}
		[HttpGet]
		[Route("signup/step3")]
		public ActionResult SignupStep3()
		{
			if (Session["Signup$PersonalInfo"] is PartnerPersonalInfoDTO)
			{
				PartnerCompanyInfoDTO companyInfo = Session["Signup$CompanyInfo"] as PartnerCompanyInfoDTO;
				CountryBLL countryBLL = new CountryBLL(WebApp.Connector);
				ViewBag.Countries = countryBLL.ReadAll();
				return View(companyInfo);
			}
			else return RedirectToAction("SignupStep2");
		}
		[HttpPost]
		[Route("signup/step3")]
		public ActionResult SignupStep3(PartnerCompanyInfoDTO companyInfo)
		{
			Connector connector = WebApp.Connector;
			RegionBLL regionBLL = new RegionBLL(connector);
			ProvinceBLL provinceBLL = new ProvinceBLL(connector);
			DistrictBLL districtBLL = new DistrictBLL(connector);
			CountryDTO country = companyInfo.Country;
			RegionDTO region = companyInfo.Region;
			ProvinceDTO province = companyInfo.Province;
			DistrictDTO district = companyInfo.District;
			if (country != null)
			{
				country.Regions = regionBLL.ReadByCountry(country.Id);
				if (region != null) region.Country = country;
				if (province != null) province.Country = country;
				if (district != null) district.Country = country;
			}
			if (region != null) region.Provinces = provinceBLL.ReadByCountryAndRegion(country.Id, region.Code);
			if (province != null) province.Districts = districtBLL.ReadByCountryAndRegionAndProvince(country.Id, region.Code, province.Code);
			if (ModelState.IsValid)
			{
				Session["Signup$CompanyInfo"] = companyInfo;
				return RedirectToAction("SignupStep4");
			}
			else
			{
				CountryBLL countryBLL = new CountryBLL(WebApp.Connector);
				ViewBag.Countries = countryBLL.ReadAll();
				return BadRequestWithErrors(companyInfo);
			}
		}
		[HttpGet]
		[Route("signup/step4")]
		public ActionResult SignupStep4()
		{
			if (Session["Signup$CompanyInfo"] is PartnerCompanyInfoDTO companyInfo)
			{
				CountryBLL countryBLL = new CountryBLL(WebApp.Connector);
				PartnerPreferencesDTO preferences = Session["Signup$Preferences"] as PartnerPreferencesDTO;
				CountryDTO country = countryBLL.ReadById(companyInfo.Country.Id);
				ViewBag.Currencies = country.SupportedCurrencies;
				ViewBag.Languages = country.SupportedLanguages;
				return View(preferences);
			}
			else return RedirectToAction("SignupStep3");
		}
		[HttpPost]
		[Route("signup/step4")]
		public ActionResult SignupStep4(PartnerPreferencesDTO preferences)
		{
			if (ModelState.IsValid)
			{
				PartnerBLL partnerBLL = new PartnerBLL(WebApp.Connector)
				{
					EmailAddressVerificationSubject = LocalizationProvider["VerifyYourEmailAddress"],
					EmailAddressVerificationTemplate = LocalizationProvider["EmailVerificationTemplate"]
				};
				PartnerDTO partner = new PartnerDTO();
				PartnerCredentialDTO credential = Session["Signup$Credential"] as PartnerCredentialDTO;
				PartnerPersonalInfoDTO personalInfo = Session["Signup$PersonalInfo"] as PartnerPersonalInfoDTO;
				PartnerCompanyInfoDTO companyInfo = Session["Signup$CompanyInfo"] as PartnerCompanyInfoDTO;
				partner.Join(credential);
				partner.Join(personalInfo);
				partner.Join(companyInfo);
				partner.Join(preferences);
				Uri requestUrl = Request.Url;
				string baseUrl = new UriBuilder(requestUrl.Scheme, requestUrl.Host, requestUrl.Port).ToString();
				PartnerBLL.SignupResult result = partnerBLL.Signup(partner, baseUrl, Url.Action("VerifyEmailAddress"));
				switch (result)
				{
					case PartnerBLL.SignupResult.OK:
						Session["Signup$Preferences"] = preferences;
						return RedirectToAction("VerifyEmailAddress");
					case PartnerBLL.SignupResult.UsernameAlreadyUsed:
						TempData["Errors"] = new Dictionary<string, string>() { { "Username", result.ToString() } };
						return RedirectToAction("SignupStep1");
					case PartnerBLL.SignupResult.EmailAddressAlreadyUsed:
						TempData["Errors"] = new Dictionary<string, string>() { { "EmailAddress", result.ToString() } };
						return RedirectToAction("SignupStep2");
					default: return BadRequest();
				}
			}
			else return BadRequestWithErrors(preferences);
		}
		[HttpGet]
		[Route("signup/verification")]
		public ActionResult VerifyEmailAddress()
		{
			if (Session["Signup$Preferences"] is PartnerPreferencesDTO)
			{
				PartnerPersonalInfoDTO personalInfo = Session["Signup$PersonalInfo"] as PartnerPersonalInfoDTO;
				Session.Abandon();
				ViewBag.EmailAddress = personalInfo.EmailAddress;
				return View();
			}
			else return RedirectToAction("SignupStep1");
		}
		[HttpGet]
		[Route("signup/verification/{id}")]
		public ActionResult VerifyEmailAddress(string id)
		{
			PartnerEmailVerificationBLL emailVerificationBLL = new PartnerEmailVerificationBLL(WebApp.Connector);
			switch (emailVerificationBLL.VerifyEmailAddress(id, out PartnerEmailVerificationDTO emailVerification))
			{
				case PartnerEmailVerificationBLL.VerifyEmailAddressResult.OK: return View(emailVerification);
				case PartnerEmailVerificationBLL.VerifyEmailAddressResult.NotFound:
				default: return HttpNotFound();
			}
		}
	}
}