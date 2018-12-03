// Hatun Search | Layer: Entities || Version: 2018.11.16.810
// (c) 2018 Hatun Search. All rights reserved.

// 'Using' directive
using System.Security.Cryptography;
using System.Text;

namespace HatunSearch.Entities.Security
{
	public static class SHA512Hasher
	{
		public static byte[] Hash(string value)
		{
			using (SHA512Managed sha512 = new SHA512Managed())
				return sha512.ComputeHash(Encoding.UTF8.GetBytes(value));
		}
	}
}