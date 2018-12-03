// Hatun Search | Layer: Data || Version: 2018.11.16.810
// (c) 2018 Hatun Search. All rights reserved.

namespace HatunSearch.Data.Patterns
{
	public interface IDeletableRepository<TId>
	{
		bool Delete(TId id);
	}
	public interface IDeletableRepository<TId1, TId2>
	{
		bool Delete(TId1 id1, TId2 id2);
	}
}