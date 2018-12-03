// Hatun Search | Layer: Business || Version: 2018.11.16.810
// (c) 2018 Hatun Search. All rights reserved.

// 'Using' directive
using HatunSearch.Business.Patterns;
using HatunSearch.Data;
using HatunSearch.Data.Databases;
using HatunSearch.Data.Helpers;
using HatunSearch.Entities;
using System;
using System.Collections.Generic;

namespace HatunSearch.Business
{
	public sealed class PartnerEmailVerificationBLL : BLL<PartnerEmailVerificationRepository>, ICreatableBLL<PartnerEmailVerificationDTO, PartnerEmailVerificationBLL.CreateResult>,
		IDeletableBLL<byte[], PartnerEmailVerificationBLL.DeleteResult>, IReadableBLL<PartnerEmailVerificationDTO, byte[]>
	{
		public PartnerEmailVerificationBLL(Connector connector) : base(connector) { }

		public enum CreateResult : byte { OK = 1 }
		public enum DeleteResult : byte
		{
			NotFound = 0,
			OK = 1
		}
		public enum VerifyEmailAddressResult : byte
		{
			NotFound = 0,
			OK = 1
		}

		public CreateResult Create(PartnerEmailVerificationDTO emailVerification)
		{
			Repository.Insert(emailVerification, out byte[] id);
			emailVerification.Id = id;
			return CreateResult.OK;
		}
		public DeleteResult Delete(byte[] id) => Repository.Delete(id) ? DeleteResult.OK : DeleteResult.NotFound;
		public PartnerEmailVerificationDTO ReadById(byte[] id) => Repository.SelectById(id);
		public VerifyEmailAddressResult VerifyEmailAddress(string id, out PartnerEmailVerificationDTO emailVerification) => VerifyEmailAddress(FormatHelper.FromHexStringToArray(id), out emailVerification);
		public VerifyEmailAddressResult VerifyEmailAddress(byte[] id, out PartnerEmailVerificationDTO emailVerification)
		{
			try
			{
				Connector.IsTransaction = true;
				VerifyEmailAddressResult result = default;
				PartnerEmailVerificationDTO emailVerificationResult = ReadById(id);
				if (emailVerificationResult != null && emailVerificationResult.IsActive)
				{
					PartnerBLL partnerBLL = new PartnerBLL(Connector);
					partnerBLL.Update(emailVerificationResult.Partner.Id, new Dictionary<string, object>() { { "HasEmailAddressBeenVerified", true } });
					Delete(id);
					result = VerifyEmailAddressResult.OK;
				}
				else result = VerifyEmailAddressResult.NotFound;
				Connector.CommitTransaction();
				emailVerification = emailVerificationResult;
				return result;
			}
			catch (Exception exception)
			{
				Connector.RollbackTransaction();
				throw exception;
			}
		}
	}
}