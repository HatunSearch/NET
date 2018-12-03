// Hatun Search | Layer: Data || Version: 2018.11.16.810
// (c) 2018 Hatun Search. All rights reserved.

// 'Using' directive
using HatunSearch.Data.Helpers;
using HatunSearch.Entities;
using HatunSearch.Entities.Security;
using System;
using System.Security.Cryptography;

namespace HatunSearch.Data.Security
{
	public static class SecureIdGenerator
	{
		public static byte[] Generate(PartnerEmailVerificationDTO emailVerification) => Generate($"Security.PartnerEmailVerification|Partner={emailVerification.Partner.Id};EmailAddress={emailVerification.EmailAddress}");
		public static byte[] Generate(PartnerSessionDTO session) => Generate($"Security.PartnerSession|Partner={session.Partner.Id};IPAddress={session.IPAddress}");
		private static byte[] Generate(string value)
		{
			using (RandomNumberGenerator randomNumberGenerator = new RNGCryptoServiceProvider())
			{
				byte[] data = new byte[256];
				randomNumberGenerator.GetBytes(data);
				return SHA512Hasher.Hash($"{value}|TimeStamp={DateTime.UtcNow.Ticks}|Random={FormatHelper.FromArrayToHexString(data)}");
			}
		}
	}
}