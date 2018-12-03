// Hatun Search | Layer: Business || Version: 2018.11.16.810
// (c) 2018 Hatun Search. All rights reserved.

// 'Using' directive
using HatunSearch.Business.Helpers;
using HatunSearch.Business.Patterns;
using HatunSearch.Data;
using HatunSearch.Data.Databases;
using HatunSearch.Data.Helpers;
using HatunSearch.Entities;
using HatunSearch.Entities.Security;
using Stripe;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Net.Mail;

namespace HatunSearch.Business
{
	public sealed class PartnerBLL : BLL<PartnerRepository>, IReadableBLL<PartnerDTO, Guid>, IUpdatableBLL<Guid, PartnerBLL.UpdateResult>
	{
		public PartnerBLL(Connector connector) : base(connector) { }

		public enum AddCardResult : byte
		{
			OK = 1,
			CardIsNotCredit = 2,
			CardHasAlreadyBeenAdded = 3,
			MaximumAmountOfCardsReached = 4
		}
		public enum AddPropertyFeatureDetailResult : byte
		{
			OK = 1,
			NotFound = 2,
			FeatureAlreadyAdded = 3
		}
		public enum AddPropertyPictureResult : byte
		{
			OK = 1,
			NotFound = 2
		}
		public enum AddPropertyResult : byte { OK = 1 }
		public enum AnswerQuestionResult : byte
		{
			OK = 1,
			NotFound = 2,
			QuestionHasBeenAlreadyAnswered = 3
		}
		public enum ChangePasswordResult : byte { OK = 1 }
		public enum CreateResult : byte
		{
			OK = 1,
			UsernameAlreadyUsed = 2,
			EmailAddressAlreadyUsed = 3
		}
		public enum DeleteCardResult : byte
		{
			OK = 1,
			NotFound = 2
		}
		public enum DeletePropertyFeatureDetailResult : byte
		{
			OK = 1,
			NotFound = 2
		}
		public enum DeletePropertyPictureResult : byte
		{
			OK = 1,
			NotFound = 2
		}
		public enum DeletePropertyResult : byte
		{
			OK = 1,
			NotFound = 2
		}
		public enum MarkQuestionAsReadResult : byte
		{
			OK = 1,
			NotFound = 2
		}
		public enum PublishPropertyResult : byte
		{
			OK = 1,
			NotFound = 2,
			PaymentHasBeenRejected = 3
		}
		public enum SignupResult : byte
		{
			OK = 1,
			UsernameAlreadyUsed = 2,
			EmailAddressAlreadyUsed = 3
		}
		public enum UpdateCompanyInfoResult : byte { OK = 1 }
		public enum UpdatePersonalInfoResult : byte
		{
			OK = 1,
			EmailAddressAlreadyUsed = 2
		}
		public enum UpdatePreferencesResult : byte { OK = 1 }
		public enum UpdatePropertyFeatureDetailResult : byte
		{
			OK = 1,
			NotFound = 2,
			FeatureAlreadyAdded = 3
		}
		public enum UpdatePropertyPictureResult : byte
		{
			OK = 1,
			NotFound = 2
		}
		public enum UpdatePropertyResult : byte
		{
			OK = 1,
			NotFound = 2
		}
		public enum UpdateResult : byte { OK = 1 }

		public AddCardResult AddCard(PartnerDTO partner, string tokenId)
		{
			PartnerCardBLL cardBLL = new PartnerCardBLL(Connector);
			PartnerCardDTO card = new PartnerCardDTO()
			{
				Partner = partner,
				StripeId = tokenId
			};
			PartnerCardBLL.CreateResult result = cardBLL.Create(card);
			return (AddCardResult)(byte)result;
		}
		public AddPropertyResult AddProperty(PartnerDTO partner, PropertyDTO property)
		{
			PropertyBLL propertyBLL = new PropertyBLL(Connector);
			property.Partner = partner;
			PropertyBLL.CreateResult result = propertyBLL.Create(property);
			return (AddPropertyResult)(byte)result;
		}
		public AddPropertyFeatureDetailResult AddPropertyFeatureDetail(PartnerDTO partner, Guid propertyId, PropertyFeatureDetailDTO featureDetail)
		{
			Connector.IsTransaction = true;
			try
			{
				PropertyBLL propertyBLL = new PropertyBLL(Connector);
				PropertyDTO property = propertyBLL.ReadById(propertyId);
				if (property.Partner.Id == partner.Id)
				{
					PropertyFeatureDetailBLL propertyFeatureDetailBLL = new PropertyFeatureDetailBLL(Connector);
					featureDetail.Property = property;
					PropertyFeatureDetailBLL.CreateResult result = propertyFeatureDetailBLL.Create(featureDetail);
					if (result == PropertyFeatureDetailBLL.CreateResult.OK)
					{
						Connector.CommitTransaction();
						return AddPropertyFeatureDetailResult.OK;
					}
					else
					{
						Connector.RollbackTransaction();
						if (result == PropertyFeatureDetailBLL.CreateResult.FeatureAlreadyAdded) return AddPropertyFeatureDetailResult.FeatureAlreadyAdded;
						else return AddPropertyFeatureDetailResult.NotFound;
					}
				}
				else return AddPropertyFeatureDetailResult.NotFound;
			}
			catch (Exception exception)
			{
				Connector.RollbackTransaction();
				throw exception;
			}
		}
		public AddPropertyPictureResult AddPropertyPicture(PartnerDTO partner, Guid propertyId, PropertyPictureDTO picture)
		{
			Connector.IsTransaction = true;
			try
			{
				PropertyBLL propertyBLL = new PropertyBLL(Connector);
				PropertyDTO property = propertyBLL.ReadById(propertyId);
				if (property.Partner.Id == partner.Id)
				{
					PropertyPictureBLL pictureBLL = new PropertyPictureBLL(Connector);
					picture.Property = property;
					PropertyPictureBLL.CreateResult result = pictureBLL.Create(picture);
					if (result == PropertyPictureBLL.CreateResult.OK) Connector.CommitTransaction();
					else Connector.RollbackTransaction();
					return (AddPropertyPictureResult)(byte)result;
				}
				else return AddPropertyPictureResult.NotFound;
			}
			catch (Exception exception)
			{
				Connector.RollbackTransaction();
				throw exception;
			}
		}
		public AnswerQuestionResult AnswerQuestion(PartnerDTO partner, Guid questionId, string answer, string baseUrl)
		{
			Connector.IsTransaction = true;
			try
			{
				CustomerQuestionBLL questionBLL = new CustomerQuestionBLL(Connector);
				CustomerQuestionDTO question = questionBLL.ReadById(questionId);
				AnswerQuestionResult result = default;
				if (question.Answer == null)
				{
					CustomerQuestionBLL.UpdateResult baseResult = questionBLL.Update(questionId, new Dictionary<string, object>()
					{
						{ "Answer", answer },
						{ "AnswerTimeStamp", DateTime.UtcNow }
					});
					result = (AnswerQuestionResult)(byte)baseResult;
					if (result == AnswerQuestionResult.OK)
					{
						string fullName = question.Customer.FullName;
						MailAddress to = new MailAddress(question.Customer.EmailAddress, fullName);
						if (baseUrl.LastIndexOf('/') == baseUrl.Length - 1) baseUrl = baseUrl.Substring(0, baseUrl.Length - 1);
						string logoUrl = new UriBuilder(baseUrl) { Path = "/png/Logo_361x86.png" }.ToString(), propertyName = question.Property.Name;
						string subject = string.Format(AnswerQuestionEmailSubject, propertyName);
						string body = string.Format(AnswerQuestionEmailTemplate, logoUrl, propertyName, fullName, question.Question, answer);
						EmailSender.Send(to, subject, body);
					}
				}
				else result = AnswerQuestionResult.QuestionHasBeenAlreadyAnswered;
				Connector.CommitTransaction();
				return result;
			}
			catch (Exception exception)
			{
				Connector.RollbackTransaction();
				throw exception;
			}
		}
		public ChangePasswordResult ChangePassword(PartnerDTO partner, string password, string baseUrl)
		{
			Repository.Update(partner.Id, new Dictionary<string, object>() { { "Password", SHA512Hasher.Hash(password) } });
			string fullName = partner.FullName;
			MailAddress to = new MailAddress(partner.EmailAddress, fullName);
			if (baseUrl.LastIndexOf('/') == baseUrl.Length - 1) baseUrl = baseUrl.Substring(0, baseUrl.Length - 1);
			string logoUrl = new UriBuilder(baseUrl) { Path = "/png/Logo_361x86.png" }.ToString();
			string body = string.Format(ChangePasswordEmailTemplate, logoUrl, fullName);
			EmailSender.Send(to, ChangePasswordEmailSubject, body);
			return ChangePasswordResult.OK;
		}
		public CreateResult Create(PartnerDTO partner)
		{
			try
			{
				Repository.Insert(partner, out Guid? id);
				partner.Id = id.Value;
				return CreateResult.OK;
			}
			catch (Exception exception)
			{
				string message = exception.Message;
				if (message.Contains("UQ_Business_Partner_Username")) return CreateResult.UsernameAlreadyUsed;
				else if (message.Contains("UQ_Business_Partner_EmailAddress")) return CreateResult.EmailAddressAlreadyUsed;
				else return default;
			}
		}
		private void CreateEmailVerificationMessage(PartnerDTO partner, bool hasExpiration, string baseUrl, string emailVerificationUrl)
		{
			PartnerEmailVerificationBLL emailVerificationBLL = new PartnerEmailVerificationBLL(Connector);
			PartnerEmailVerificationDTO emailVerification = new PartnerEmailVerificationDTO()
			{
				Partner = partner,
				EmailAddress = partner.EmailAddress,
			};
			if (hasExpiration) emailVerification.ExpiresOn = DateTime.UtcNow.AddHours(24);
			emailVerificationBLL.Create(emailVerification);
			string fullName = partner.FullName;
			MailAddress to = new MailAddress(partner.EmailAddress, fullName);
			if (baseUrl.LastIndexOf('/') == baseUrl.Length - 1) baseUrl = baseUrl.Substring(0, baseUrl.Length - 1);
			string logoUrl = new UriBuilder(baseUrl) { Path = "/png/Logo_361x86.png" }.ToString(),
				verificationUrl = new UriBuilder(baseUrl) { Path = $"{emailVerificationUrl}/{FormatHelper.FromArrayToHexString(emailVerification.Id)}" }.ToString();
			string body = string.Format(EmailAddressVerificationTemplate, logoUrl, fullName, verificationUrl);
			EmailSender.Send(to, EmailAddressVerificationSubject, body);
		}
		public DeleteCardResult DeleteCard(PartnerDTO partner, Guid cardId)
		{
			PartnerCardBLL cardBLL = new PartnerCardBLL(Connector);
			PartnerCardDTO card = ReadCardById(partner, cardId);
			PartnerCardBLL.DeleteResult result = default;
			if (card != null) result = cardBLL.Delete(cardId);
			else result = PartnerCardBLL.DeleteResult.NotFound;
			return (DeleteCardResult)(byte)result;
		}
		public DeletePropertyResult DeleteProperty(PartnerDTO partner, Guid propertyId)
		{
			PropertyBLL propertyBLL = new PropertyBLL(Connector);
			PropertyDTO property = ReadPropertyById(partner, propertyId);
			PropertyBLL.DeleteResult result = default;
			if (property != null) result = propertyBLL.Delete(propertyId);
			else result = PropertyBLL.DeleteResult.NotFound;
			return (DeletePropertyResult)(byte)result;
		}
		public DeletePropertyFeatureDetailResult DeletePropertyFeatureDetail(PartnerDTO partner, Guid propertyId, byte featureId)
		{
			Connector.IsTransaction = true;
			try
			{
				PropertyBLL propertyBLL = new PropertyBLL(Connector);
				PropertyDTO property = propertyBLL.ReadById(propertyId);
				if (property.Partner.Id == partner.Id)
				{
					PropertyFeatureDetailBLL propertyFeatureDetailBLL = new PropertyFeatureDetailBLL(Connector);
					PropertyFeatureDetailBLL.DeleteResult result = propertyFeatureDetailBLL.Delete(propertyId, featureId);
					if (result == PropertyFeatureDetailBLL.DeleteResult.OK) Connector.CommitTransaction();
					else Connector.RollbackTransaction();
					return (DeletePropertyFeatureDetailResult)(byte)result;
				}
				else return DeletePropertyFeatureDetailResult.NotFound;
			}
			catch (Exception exception)
			{
				Connector.RollbackTransaction();
				throw exception;
			}
		}
		public DeletePropertyPictureResult DeletePropertyPicture(PartnerDTO partner, Guid propertyId, Guid pictureId)
		{
			Connector.IsTransaction = true;
			try
			{
				PropertyBLL propertyBLL = new PropertyBLL(Connector);
				PropertyDTO property = propertyBLL.ReadById(propertyId);
				if (property.Partner.Id == partner.Id)
				{
					PropertyPictureBLL pictureBLL = new PropertyPictureBLL(Connector);
					PropertyPictureBLL.DeleteResult result = pictureBLL.Delete(pictureId);
					if (result == PropertyPictureBLL.DeleteResult.OK) Connector.CommitTransaction();
					else Connector.RollbackTransaction();
					return (DeletePropertyPictureResult)(byte)result;
				}
				else return DeletePropertyPictureResult.NotFound;
			}
			catch (Exception exception)
			{
				Connector.RollbackTransaction();
				throw exception;
			}
		}
		public MarkQuestionAsReadResult MarkQuestionAsRead(PartnerDTO partner, Guid questionId)
		{
			CustomerQuestionBLL questionBLL = new CustomerQuestionBLL(Connector);
			CustomerQuestionBLL.UpdateResult result = questionBLL.Update(questionId, new Dictionary<string, object>() { { "HasQuestionBeenRead", true } });
			return (MarkQuestionAsReadResult)(byte)result;
		}
		public PublishPropertyResult PublishProperty(PartnerDTO partner, Guid propertyId, PartnerCardDTO card, PublishModeDTO publishMode, string baseUrl)
		{
			Connector.IsTransaction = true;
			try
			{
				PropertyBLL propertyBLL = new PropertyBLL(Connector);
				PropertyDTO property = ReadPropertyById(partner, propertyId);
				PropertyBLL.UpdateResult result = default;
				if (property != null && !property.HasBeenPaid)
				{
					PartnerCardBLL cardBLL = new PartnerCardBLL(Connector);
					ChargeService chargeService = new ChargeService();
					PublishModeBLL publishModeBLL = new PublishModeBLL(Connector);
					card = cardBLL.ReadById(card.Id);
					publishMode = publishModeBLL.ReadById(publishMode.Id);
					decimal cost = publishMode.Cost;
					CurrencyDTO preferredCurrency = partner.PreferredCurrency;
					if (preferredCurrency.Id != publishMode.Currency.Id)
					{
						CurrencyExchangeBLL currencyExchangeBLL = new CurrencyExchangeBLL(Connector);
						CurrencyExchangeDTO currencyExchange = currencyExchangeBLL.ReadById(publishMode.Currency, partner.PreferredCurrency);
						cost *= currencyExchange.Rate;
					}
					publishMode = publishModeBLL.ReadById(publishMode.Id);
					ChargeCreateOptions createOptions = new ChargeCreateOptions()
					{
						Description = $"{publishMode.DisplayName} @ Hatun Search",
						Currency = preferredCurrency.Id,
						Amount = (long)(cost * 100),
						CustomerId = partner.StripeId,
						SourceId = card.StripeId
					};
					Charge charge = chargeService.Create(createOptions);
					if (charge != null)
					{
						PartnerInvoiceBLL invoiceBLL = new PartnerInvoiceBLL(Connector);
						PartnerInvoiceDTO invoice = new PartnerInvoiceDTO
						{
							Partner = partner,
							Currency = preferredCurrency,
							StripeId = charge.Id,
							Details = new List<PartnerInvoiceDetailDTO>()
							{
								new PartnerInvoiceDetailDTO()
								{
									Property = property,
									PublishMode = publishMode,
									Cost = cost
								}
							}
						};
						invoiceBLL.Create(invoice);
						result = propertyBLL.Update(propertyId, new Dictionary<string, object>()
						{
							{ "PublishMode", publishMode.Id },
							{ "HasBeenPaid", true }
						});
						MailAddress to = new MailAddress(partner.EmailAddress, partner.FullName);
						if (baseUrl.LastIndexOf('/') == baseUrl.Length - 1) baseUrl = baseUrl.Substring(0, baseUrl.Length - 1);
						string logoUrl = new UriBuilder(baseUrl) { Path = "/png/Logo_361x86.png" }.ToString();
						string body = string.Format(PublishPropertyEmailTemplate, logoUrl, partner.FullName, invoice.Id, card.Brand, $"•••• •••• •••• {card.Last4}", property.Name, publishMode.DisplayName, preferredCurrency.Symbol, cost.ToString("0.00"));
						EmailSender.Send(to, PublishPropertyEmailSubject, body);
						Connector.CommitTransaction();
					}
					else
					{
						Connector.RollbackTransaction();
						return PublishPropertyResult.PaymentHasBeenRejected;
					}
				}
				else
				{
					result = PropertyBLL.UpdateResult.NotFound;
					Connector.RollbackTransaction();
				}
				return (PublishPropertyResult)(byte)result;
			}
			catch (Exception exception)
			{
				Connector.RollbackTransaction();
				if (exception is StripeException) return PublishPropertyResult.PaymentHasBeenRejected;
				else throw exception;
			}
		}
		private PartnerDTO ReadFromRepository(PartnerDTO partner)
		{
			if (partner != null)
			{
				PartnerCardBLL cardBLL = new PartnerCardBLL(Connector);
				CardService cardService = new CardService();
				ChargeService chargeService = new ChargeService();
				PartnerInvoiceBLL invoiceBLL = new PartnerInvoiceBLL(Connector);
				partner.Cards = partner.Cards.Select(i =>
				{
					i.StripeCard = cardBLL.GetStripeCard(i, cardService);
					return i;
				});
				partner.Invoices = partner.Invoices.Select(i =>
				{
					i.StripeCharge = invoiceBLL.GetStripeCharge(i, chargeService);
					return i;
				});
				return partner;
			}
			else return null;
		}
		public PartnerDTO ReadById(Guid id) => ReadFromRepository(Repository.SelectById(id));
		public PartnerDTO ReadByUsername(string username) => ReadFromRepository(Repository.SelectByUsername(username));
		public PartnerCardDTO ReadCardById(PartnerDTO partner, Guid cardId)
		{
			PartnerCardBLL cardBLL = new PartnerCardBLL(Connector);
			PartnerCardDTO card = cardBLL.ReadById(cardId);
			return card.Partner.Id == partner.Id ? card : null;
		}
		public CustomerQuestionDTO ReadCustomerQuestionById(PartnerDTO partner, Guid questionId)
		{
			CustomerQuestionBLL questionBLL = new CustomerQuestionBLL(Connector);
			CustomerQuestionDTO question = questionBLL.ReadById(questionId);
			return question.Partner.Id == partner.Id ? question : null;
		}
		public PartnerInvoiceDTO ReadInvoiceById(PartnerDTO partner, Guid invoiceId)
		{
			PartnerInvoiceBLL invoiceBLL = new PartnerInvoiceBLL(Connector);
			PartnerInvoiceDTO invoice = invoiceBLL.ReadById(invoiceId);
			return invoice.Partner.Id == partner.Id ? invoice : null;
		}
		public PropertyDTO ReadPropertyById(PartnerDTO partner, Guid propertyId)
		{
			PropertyBLL propertyBLL = new PropertyBLL(Connector);
			PropertyDTO property = propertyBLL.ReadById(propertyId);
			return property.Partner.Id == partner.Id ? property : null;
		}
		public PropertyFeatureDetailDTO ReadPropertyFeatureDetailById(PartnerDTO partner, Guid propertyId, byte featureId)
		{
			PropertyBLL propertyBLL = new PropertyBLL(Connector);
			PropertyDTO property = ReadPropertyById(partner, propertyId);
			if (property != null)
			{
				PropertyFeatureDetailBLL featureDetailBLL = new PropertyFeatureDetailBLL(Connector);
				PropertyFeatureDetailDTO featureDetail = featureDetailBLL.ReadById(propertyId, featureId);
				return featureDetail;
			}
			else return null;
		}
		public PropertyPictureDTO ReadPropertyPictureById(PartnerDTO partner, Guid propertyId, Guid pictureId)
		{
			PropertyBLL propertyBLL = new PropertyBLL(Connector);
			PropertyDTO property = ReadPropertyById(partner, propertyId);
			if (property != null)
			{
				PropertyPictureBLL pictureBLL = new PropertyPictureBLL(Connector);
				PropertyPictureDTO picture = pictureBLL.ReadById(pictureId);
				return picture;
			}
			else return null;
		}
		public SignupResult Signup(PartnerDTO partner, string baseUrl, string emailVerificationUrl)
		{
			try
			{
				Connector.IsTransaction = true;
				CreateResult result = Create(partner);
				if (result == CreateResult.OK)
				{
					CreateEmailVerificationMessage(partner, hasExpiration: true, baseUrl, emailVerificationUrl);
					Connector.CommitTransaction();
					return SignupResult.OK;
				}
				else
				{
					Connector.RollbackTransaction();
					return (SignupResult)((byte)result);
				}
			}
			catch (DbException exception)
			{
				Connector.RollbackTransaction();
				throw exception;
			}
			catch (Exception exception)
			{
				Connector.RollbackTransaction();
				throw exception;
			}
		}
		public UpdateResult Update(Guid id, IDictionary<string, object> fields)
		{
			Repository.Update(id, fields);
			return UpdateResult.OK;
		}
		public UpdateCompanyInfoResult UpdateCompanyInfo(Guid id, PartnerCompanyInfoDTO companyInfo)
		{
			Update(id, new Dictionary<string, object>()
			{
				{ "CompanyName", companyInfo.Name },
				{ "Address", companyInfo.Address },
				{ "District", companyInfo.District.Code },
				{ "Province", companyInfo.Province.Code },
				{ "Region", companyInfo.Region.Code },
				{ "Country", companyInfo.Country },
				{ "PhoneNumber", companyInfo.PhoneNumber },
				{ "Website", companyInfo.Website }
			});
			return UpdateCompanyInfoResult.OK;
		}
		public UpdatePersonalInfoResult UpdatePersonalInfo(Guid id, PartnerPersonalInfoDTO personalInfo, string baseUrl, string emailVerificationUrl)
		{
			Connector.IsTransaction = true;
			try
			{
				PartnerDTO partner = ReadById(id);
				bool hasEmailAddressChanged = partner.EmailAddress != personalInfo.EmailAddress;
				Update(id, new Dictionary<string, object>()
				{
					{ "FirstName", personalInfo.FirstName },
					{ "MiddleName", personalInfo.MiddleName },
					{ "LastName", personalInfo.LastName },
					{ "Gender", personalInfo.Gender },
					{ "EmailAddress", personalInfo.EmailAddress },
					{ "MobileNumber", personalInfo.MobileNumber },
					{ "HasEmailAddressBeenVerified", !hasEmailAddressChanged }
				});
				if (hasEmailAddressChanged)
				{
					partner.EmailAddress = personalInfo.EmailAddress;
					CreateEmailVerificationMessage(partner, hasExpiration: false, baseUrl, emailVerificationUrl);
				}
				Connector.CommitTransaction();
				return UpdatePersonalInfoResult.OK;
			}
			catch (Exception exception)
			{
				Connector.RollbackTransaction();
				if (exception.Message.Contains("UQ_Business_Partner_EmailAddress")) return UpdatePersonalInfoResult.EmailAddressAlreadyUsed;
				else throw exception;
			}
		}
		public UpdatePreferencesResult UpdatePreferences(Guid id, PartnerPreferencesDTO preferences)
		{
			Update(id, new Dictionary<string, object>()
			{
				{ "PreferredCurrency", preferences.PreferredCurrency },
				{ "PreferredLanguage", preferences.PreferredLanguage }
			});
			return UpdatePreferencesResult.OK;
		}
		public UpdatePropertyResult UpdateProperty(PartnerDTO partner, Guid propertyId, PropertyDTO property)
		{
			PropertyBLL propertyBLL = new PropertyBLL(Connector);
			PropertyDTO previouslyAddedProperty = ReadPropertyById(partner, propertyId);
			if (previouslyAddedProperty.Partner.Id == partner.Id)
			{
				PropertyBLL.UpdateResult result = default;
				result = propertyBLL.Update(propertyId, new Dictionary<string, object>()
				{
					{ "Name", property.Name },
					{ "Type", property.Type },
					{ "Address", property.Address },
					{ "District", property.District.Code },
					{ "Province", property.Province.Code },
					{ "Region", property.Region.Code },
					{ "Country", property.Country }
				});
				return (UpdatePropertyResult)(byte)result;
			}
			else return UpdatePropertyResult.NotFound;
		}
		public UpdatePropertyFeatureDetailResult UpdatePropertyFeature(PartnerDTO partner, Guid propertyId, byte featureId, PropertyFeatureDetailDTO featureDetail)
		{
			Connector.IsTransaction = true;
			try
			{
				PropertyBLL propertyBLL = new PropertyBLL(Connector);
				PropertyDTO property = propertyBLL.ReadById(propertyId);
				if (property.Partner.Id == partner.Id)
				{
					PropertyFeatureDetailBLL propertyFeatureDetailBLL = new PropertyFeatureDetailBLL(Connector);
					PropertyFeatureDetailBLL.UpdateResult result = propertyFeatureDetailBLL.Update(propertyId, featureId, new Dictionary<string, object>() { { "Value", featureDetail.Value } });
					if (result == PropertyFeatureDetailBLL.UpdateResult.OK) Connector.CommitTransaction();
					else Connector.RollbackTransaction();
					return (UpdatePropertyFeatureDetailResult)(byte)result;
				}
				else return UpdatePropertyFeatureDetailResult.NotFound;
			}
			catch (Exception exception)
			{
				Connector.RollbackTransaction();
				throw exception;
			}
		}
		public UpdatePropertyPictureResult UpdatePropertyPicture(PartnerDTO partner, Guid propertyId, Guid pictureId, PropertyPictureDTO picture)
		{
			Connector.IsTransaction = true;
			try
			{
				PropertyBLL propertyBLL = new PropertyBLL(Connector);
				PropertyDTO property = propertyBLL.ReadById(propertyId);
				if (property.Partner.Id == partner.Id)
				{
					PropertyPictureBLL pictureBLL = new PropertyPictureBLL(Connector);
					PropertyPictureBLL.UpdateResult result = pictureBLL.Update(pictureId, new Dictionary<string, object>() { { "Description", picture.Description } });
					if (result == PropertyPictureBLL.UpdateResult.OK) Connector.CommitTransaction();
					else Connector.RollbackTransaction();
					return (UpdatePropertyPictureResult)(byte)result;
				}
				else return UpdatePropertyPictureResult.NotFound;
			}
			catch (Exception exception)
			{
				Connector.RollbackTransaction();
				throw exception;
			}
		}

		public string AnswerQuestionEmailTemplate { get; set; }
		public string AnswerQuestionEmailSubject { get; set; }
		public string ChangePasswordEmailTemplate { get; set; }
		public string ChangePasswordEmailSubject { get; set; }
		public string EmailAddressVerificationSubject { get; set; }
		public string EmailAddressVerificationTemplate { get; set; }
		public string PublishPropertyEmailSubject { get; set; }
		public string PublishPropertyEmailTemplate { get; set; }
	}
}