// Hatun Search | Layer: Entities || Version: 2018.11.16.810
// (c) 2018 Hatun Search. All rights reserved.

// 'Using' directive
using HatunSearch.Entities.Patterns;

namespace HatunSearch.Entities.Data
{
	public sealed class CompositeKey<TId1, TId2>
	{
		public TId1 FirstKey { get; set; }
		public TId2 SecondKey { get; set; }

		public override bool Equals(object obj)
		{
			if (obj is CompositeKey<TId1, TId2> compositeKey)
			{
				bool areFirstKeysEqual = !typeof(IDTO).IsAssignableFrom(typeof(TId1)) ? compositeKey.FirstKey.Equals(FirstKey) : (compositeKey.FirstKey as IDTO).Id.Equals((compositeKey.FirstKey as IDTO).Id),
					areSecondKeysEqual = !typeof(IDTO).IsAssignableFrom(typeof(TId2)) ? compositeKey.SecondKey.Equals(SecondKey) : (compositeKey.SecondKey as IDTO).Id.Equals((compositeKey.SecondKey as IDTO).Id);
				return areFirstKeysEqual && areSecondKeysEqual;
			}
			else return base.Equals(obj);
		}
		public override int GetHashCode() => base.GetHashCode();
		public override string ToString()
		{
			string firstKey = string.Empty, secondKey = string.Empty;
			firstKey = typeof(IDTO).IsAssignableFrom(typeof(TId1)) ? (FirstKey as IDTO).Id.ToString() : FirstKey.ToString();
			secondKey = typeof(IDTO).IsAssignableFrom(typeof(TId2)) ? (SecondKey as IDTO).Id.ToString() : SecondKey.ToString();
			return $"{firstKey}|{secondKey}";
		}
	}
}