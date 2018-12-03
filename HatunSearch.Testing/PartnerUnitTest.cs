// Hatun Search | Layer: Testing || Version: 2018.11.16.810
// (c) 2018 Hatun Search. All rights reserved.

// 'Using' directive
using System.Net.Mail;
using HatunSearch.Business;
using HatunSearch.Business.Helpers;
using HatunSearch.Data.Helpers;
using HatunSearch.Entities;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace HatunSearch.Testing
{
	[TestClass]
	public class PartnerUnitTest
	{
		[TestMethod]
		public void ChangePasswordTest()
		{
			PartnerBLL partnerBLL = new PartnerBLL(TestApp.Connector)
			{
				ChangePasswordEmailSubject = "Cambio de contraseña en su cuenta",
				ChangePasswordEmailTemplate = "<!DOCTYPE html><html><head><meta charset=\"utf-8\"></head><body style=\"font-family: Roboto, 'Segoe UI', sans-serif; margin: 24px;\"><img src=\"{0}\" width=\"192\" /><h3>Cambio de contraseña en su cuenta</h3><p>¡Hola, {1}!</p><p>Recientemente se ha cambiado su contraseña de su cuenta de Hatun Search. Por motivos de seguridad, le enviamos esta notificación para que, en caso usted no haya realizado dicho cambio, pueda comunicarse con nosotros para poder tomar medidas con el fin de proteger su cuenta.</p><p>Si usted no realizó el cambio antes mencionado, por favor, escríbanos un mensaje a <a href=\"mailto:support@hatunsearch.me\">support@hatunsearch.me</a> indicándonos dicho inconveniente. Le brindaremos ayuda para recuperar el control de su cuenta.</p><p>Saludos,<br>Departamento de Atención al Cliente</p><p>Por favor, no responda directamente a este mensaje, ya que ha sido enviado desde un correo electrónico que no se supervisa.</p></body></html>"
			};
			EmailSender.DefaultAddress = new MailAddress("noreply@hatunsearch.me", "Hatun Search");
			PartnerDTO previousPartner = partnerBLL.ReadByUsername("aldoastupillo");
			partnerBLL.ChangePassword(previousPartner, "1234567890", "https://partners.hatunsearch.me");
			PartnerDTO currentPartner = partnerBLL.ReadByUsername("aldoastupillo");
			string currentPassword = FormatHelper.FromArrayToHexString(currentPartner.Password), previousPassword = FormatHelper.FromArrayToHexString(previousPartner.Password);
			Assert.AreNotEqual(previousPassword, currentPassword);
			partnerBLL.ChangePassword(previousPartner, "12345678", "https://partners.hatunsearch.me");
		}
		[TestMethod]
		public void CreateAPartnerAccountWithAnAlreadyUsedEmailAddress()
		{
			PartnerBLL partnerBLL = new PartnerBLL(TestApp.Connector);
			CountryDTO country = new CountryDTO() { Id = "PE" };
			DistrictDTO district = new DistrictDTO()
			{
				Country = country,
				Code = "150106",
				Province = new ProvinceDTO()
				{
					Country = country,
					Code = "150100",
					Region = new RegionDTO()
					{
						Country = country,
						Code = "150000"
					}
				}
			};
			PartnerDTO partner = new PartnerDTO()
			{
				Username = "aldoastupilloc",
				Password = new byte[64],
				FirstName = "Aldo",
				MiddleName = "Alejandro",
				LastName = "Astupillo Cáceres",
				Gender = new GenderDTO() { Id = "M" },
				EmailAddress = "aastupillo@hatunsearch.me",
				MobileNumber = "+51989637468",
				CompanyName = "Hatun Search",
				Address = "Av. Larco 322",
				Country = country,
				District = district,
				PhoneNumber = "+5115474849",
				Website = "https://hatunsearch.me",
				PreferredCurrency = new CurrencyDTO() { Id = "PEN" },
				PreferredLanguage = new LanguageDTO() { Id = "ES" }
			};
			PartnerBLL.SignupResult result = partnerBLL.Signup(partner, "https://partners.hatunsearch.me", "https://partners.hatunsearch.me/es-pe/accounts/signup/verification");
			Assert.AreEqual(PartnerBLL.SignupResult.EmailAddressAlreadyUsed, result);
		}
		[TestMethod]
		public void CreateAPartnerAccountWithAnAlreadyUsedUsername()
		{
			PartnerBLL partnerBLL = new PartnerBLL(TestApp.Connector);
			CountryDTO country = new CountryDTO() { Id = "PE" };
			DistrictDTO district = new DistrictDTO()
			{
				Country = country,
				Code = "150106",
				Province = new ProvinceDTO()
				{
					Country = country,
					Code = "150100",
					Region = new RegionDTO()
					{
						Country = country,
						Code = "150000"
					}
				}
			};
			PartnerDTO partner = new PartnerDTO()
			{
				Username = "aldoastupillo",
				Password = new byte[64],
				FirstName = "Aldo",
				MiddleName = "Alejandro",
				LastName = "Astupillo Cáceres",
				Gender = new GenderDTO() { Id = "M" },
				EmailAddress = "aastupillo@hatunsearch.com",
				MobileNumber = "+51989637468",
				CompanyName = "Hatun Search",
				Address = "Av. Larco 322",
				Country = country,
				District = district,
				PhoneNumber = "+5115474849",
				Website = "https://hatunsearch.me",
				PreferredCurrency = new CurrencyDTO() { Id = "PEN" },
				PreferredLanguage = new LanguageDTO() { Id = "ES" }
			};
			PartnerBLL.SignupResult result = partnerBLL.Signup(partner, "https://partners.hatunsearch.me", "https://partners.hatunsearch.me/es-pe/accounts/signup/verification");
			Assert.AreEqual(PartnerBLL.SignupResult.UsernameAlreadyUsed, result);
		}
	}
}