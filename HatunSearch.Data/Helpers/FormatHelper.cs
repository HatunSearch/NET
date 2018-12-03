// Hatun Search | Layer: Data || Version: 2018.11.16.810
// (c) 2018 Hatun Search. All rights reserved.

// 'Using' directive
using System.Globalization;
using System.Text;

namespace HatunSearch.Data.Helpers
{
	public static class FormatHelper
	{
		public static string FromArrayToHexString(byte[] value)
		{
			StringBuilder stringBuilder = new StringBuilder(value.Length * 2);
			foreach (byte item in value) stringBuilder.Append(item.ToString("x2"));
			return stringBuilder.ToString();
		}
		public static byte[] FromHexStringToArray(string value)
		{
			byte[] result = new byte[value.Length / 2];
			for (int i = 0; i < value.Length; i += 2) result[i / 2] = byte.Parse(value.Substring(i, 2), NumberStyles.HexNumber);
			return result;
		}
	}
}