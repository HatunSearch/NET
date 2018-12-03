// Hatun Search | Layer: Business || Version: 2018.11.16.810
// (c) 2018 Hatun Search. All rights reserved.

// 'Using' directive
using HatunSearch.Business.Helpers;
using HatunSearch.Business.Patterns;
using HatunSearch.Data;
using HatunSearch.Data.Databases;
using HatunSearch.Entities;
using HatunSearch.Entities.Security;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;

namespace HatunSearch.Business
{
	public sealed class PartnerSessionBLL : BLL<PartnerSessionRepository>, ICreatableBLL<PartnerSessionDTO, PartnerSessionBLL.CreateResult>, IReadableBLL<PartnerSessionDTO, byte[]>,
		IUpdatableBLL<byte[], PartnerSessionBLL.UpdateResult>
	{
		public PartnerSessionBLL(Connector connector) : base(connector) { }

		public enum CreateResult : byte { OK = 1 }
		public enum LoginResult : byte
		{
			OK = 1,
			AccountDoesntExist = 2,
			PasswordDoesntMatch = 3,
			EmailAddressHasNotBeenVerified = 4,
			AccountIsLocked = 5
		}
		public enum LogoutResult : byte
		{
			OK = 1,
			NotFound = 2
		}
		public enum UpdateResult : byte
		{
			OK = 1,
			NotFound = 2
		}

		public CreateResult Create(PartnerSessionDTO session)
		{
			Repository.Insert(session, out byte[] id);
			session.Id = id;
			return CreateResult.OK;
		}
		public LoginResult Login(PartnerCredentialDTO credential, IPAddress ipAddress, bool keepOpened, out PartnerSessionDTO session)
		{
			Connector.IsTransaction = true;
			PartnerBLL partnerBLL = new PartnerBLL(Connector);
			PartnerDTO partner = partnerBLL.ReadByUsername(credential.Username);
			if (partner != null)
			{
				if (!partner.IsLocked)
				{
					byte[] credentialPassword = SHA512Hasher.Hash(credential.Password);
					if (BinaryComparer.AreEqual(credentialPassword, partner.Password))
					{
						if (partner.HasEmailAddressBeenVerified)
						{
							DateTime loggedAt = DateTime.UtcNow;
							session = new PartnerSessionDTO()
							{
								Partner = partner,
								IPAddress = ipAddress,
								LoggedAt = loggedAt
							};
							if (!keepOpened) session.ExpiresOn = loggedAt.AddMinutes(16);
							Create(session);
							Connector.CommitTransaction();
							return LoginResult.OK;
						}
						else
						{
							Connector.RollbackTransaction();
							session = null;
							return LoginResult.EmailAddressHasNotBeenVerified;
						}
					}
					else
					{
						PartnerLoginAttemptBLL loginAttemptBLL = new PartnerLoginAttemptBLL(Connector);
						PartnerLoginAttemptDTO loginAttempt = new PartnerLoginAttemptDTO()
						{
							Partner = partner,
							IPAddress = ipAddress
						};
						loginAttemptBLL.Create(loginAttempt);
						Guid partnerId = partner.Id;
						PartnerSessionDTO lastSession = ReadLastByPartner(partnerId);
						List<PartnerLoginAttemptDTO> loginAttempts = loginAttemptBLL.ReadByPartnerAndTimeStampAsDate(partnerId, lastSession?.LoggedAt ?? DateTime.UtcNow.Date).ToList();
						if (loginAttempts.Count >= 3) partnerBLL.Update(partnerId, new Dictionary<string, object>() { { "IsLocked", true } });
						Connector.CommitTransaction();
						session = null;
						return LoginResult.PasswordDoesntMatch;
					}
				}
				else
				{
					Connector.RollbackTransaction();
					session = null;
					return LoginResult.AccountIsLocked;
				}
			}
			else
			{
				Connector.RollbackTransaction();
				session = null;
				return LoginResult.AccountDoesntExist;
			}
		}
		public LogoutResult Logout(byte[] sessionId)
		{
			UpdateResult result = Update(sessionId, new Dictionary<string, object>()
			{
				{ "ExpiresOn", DateTime.UtcNow },
				{ "IsActive", false }
			});
			return (LogoutResult)(byte)result;
		}
		public PartnerSessionDTO ReadById(byte[] id) => Repository.SelectById(id);
		public PartnerSessionDTO ReadLastByPartner(Guid partnerId) => Repository.SelectLastByPartner(partnerId);
		public UpdateResult Update(byte[] id, IDictionary<string, object> fields) => Repository.Update(id, fields) == 1 ? UpdateResult.OK : UpdateResult.NotFound;
		public void UpdateExpiration(byte[] id, DateTime expiresOn) => Update(id, new Dictionary<string, object>() { { "ExpiresOn", expiresOn } });
	}
}