// Hatun Search | Layer: Business || Version: 2018.11.16.810
// (c) 2018 Hatun Search. All rights reserved.

// 'Using' directive
using HatunSearch.Business.Patterns;
using HatunSearch.Data;
using HatunSearch.Data.Databases;
using HatunSearch.Entities;
using Stripe;
using System;
using System.Collections.Generic;
using System.Linq;

namespace HatunSearch.Business
{
	public sealed class PartnerCardBLL : BLL<PartnerCardRepository>, ICreatableBLL<PartnerCardDTO, PartnerCardBLL.CreateResult>, IDeletableBLL<Guid, PartnerCardBLL.DeleteResult>
	{
		public PartnerCardBLL(Connector connector) : base(connector) { }

		public enum CreateResult : byte
		{
			OK = 1,
			CardIsNotCredit = 2,
			CardHasAlreadyBeenAdded = 3,
			MaximumAmountOfCardsReached = 4
		}
		public enum DeleteResult : byte
		{
			OK = 1,
			NotFound = 2
		}

		public CreateResult Create(PartnerCardDTO card)
		{
			CreateResult result = default;
			CardService cardService = new CardService();
			TokenService tokenService = new TokenService();
			Token stripeToken = tokenService.Get(card.StripeId);
			Card stripeNewCard = stripeToken.Card;
			if (stripeNewCard.Funding == "credit")
			{
				Connector.IsTransaction = true;
				try
				{
					PartnerDTO partner = card.Partner;
					string partnerStripeId = partner.StripeId;
					if (partnerStripeId == null)
					{
						CustomerService customerService = new CustomerService();
						PartnerBLL partnerBLL = new PartnerBLL(Connector);
						Customer customer = customerService.Create(new CustomerCreateOptions() { Email = partner.EmailAddress });
						partner.StripeId = customer.Id;
						partnerStripeId = partner.StripeId;
						partnerBLL.Update(card.Partner.Id, new Dictionary<string, object> { { "StripeId", partner.StripeId } });
					}
					IEnumerable<Card> stripeCards = cardService.List(partnerStripeId);
					if (stripeCards.Count() < 10)
					{
						bool hasAlreadyBeenAdded = false;
						foreach (Card stripeCard in stripeCards)
						{
							if (stripeCard.Fingerprint == stripeNewCard.Fingerprint)
							{
								hasAlreadyBeenAdded = true;
								break;
							}
						}
						if (!hasAlreadyBeenAdded)
						{
							CardCreateOptions cardCreateOptions = new CardCreateOptions() { SourceToken = card.StripeId };
							stripeNewCard = cardService.Create(card.Partner.StripeId, cardCreateOptions);
							card.StripeId = stripeNewCard.Id;
							Repository.Insert(card, out Guid? id);
							card.Id = id.Value;
							result = CreateResult.OK;
						}
						else result = CreateResult.CardHasAlreadyBeenAdded;
						Connector.CommitTransaction();
					}
					else
					{
						Connector.RollbackTransaction();
						result = CreateResult.MaximumAmountOfCardsReached;
					}
				}
				catch (Exception exception)
				{
					Connector.RollbackTransaction();
					throw exception;
				}
			}
			else result = CreateResult.CardIsNotCredit;
			return result;
		}
		public DeleteResult Delete(Guid id)
		{
			try
			{
				Connector.IsTransaction = true;
				PartnerCardDTO card = ReadById(id);
				bool result = false;
				if (card != null)
				{
					CardService cardService = new CardService();
					cardService.Delete(card.Partner.StripeId, card.StripeId);
					result = Repository.Delete(id);
					Connector.CommitTransaction();
				}
				else Connector.RollbackTransaction();
				return result ? DeleteResult.OK : DeleteResult.NotFound;
			}
			catch (Exception exception)
			{
				Connector.RollbackTransaction();
				throw exception;
			}
		}
		public Card GetStripeCard(PartnerCardDTO card) => GetStripeCard(card, new CardService());
		public Card GetStripeCard(PartnerCardDTO card, CardService cardService) => cardService.Get(card.Partner.StripeId, card.StripeId);
		public PartnerCardDTO ReadById(Guid id)
		{
			PartnerCardDTO card = Repository.SelectById(id);
			if (card != null) card.StripeCard = GetStripeCard(card);
			return card;
		}
		public IEnumerable<PartnerCardDTO> ReadByPartner(Guid partnerId)
		{
			CardService cardService = new CardService();
			IEnumerable<PartnerCardDTO> cards = Repository.SelectByPartner(partnerId);
			return cards.Select(i =>
			{
				i.StripeCard = GetStripeCard(i, cardService);
				return i;
			});
		}
	}
}