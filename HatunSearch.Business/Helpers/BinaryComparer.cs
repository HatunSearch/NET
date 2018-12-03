// Hatun Search | Layer: Business || Version: 2018.11.16.810
// (c) 2018 Hatun Search. All rights reserved.

namespace HatunSearch.Business.Helpers
{
	public static class BinaryComparer
	{
		public static bool AreEqual(byte[] firstValue, byte[] secondValue)
		{
			int firstValueLength = firstValue.Length;
			if (firstValueLength == secondValue.Length)
			{
				for (int i = 0; i < firstValueLength; i++)
					if (firstValue[i] != secondValue[i]) return false;
				return true;
			}
			else return false;
		}
	}
}