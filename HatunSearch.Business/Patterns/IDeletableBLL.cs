// Hatun Search | Layer: Business || Version: 2018.11.16.810
// (c) 2018 Hatun Search. All rights reserved.

namespace HatunSearch.Business.Patterns
{
	public interface IDeletableBLL<TId, TResult>
	{
		TResult Delete(TId id);
	}
	public interface IDeletableBLL<TId1, TId2, TResult>
	{
		TResult Delete(TId1 id1, TId2 id2);
	}
}