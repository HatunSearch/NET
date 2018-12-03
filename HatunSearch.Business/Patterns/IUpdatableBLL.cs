// Hatun Search | Layer: Business || Version: 2018.11.16.810
// (c) 2018 Hatun Search. All rights reserved.

// 'Using' directive
using System.Collections.Generic;

namespace HatunSearch.Business.Patterns
{
	public interface IUpdatableBLL<TId, TResult> where TResult : struct
	{
		TResult Update(TId id, IDictionary<string, object> fields);
	}
	public interface IUpdatableBLL<TId1, TId2, TResult> where TResult : struct
	{
		TResult Update(TId1 id1, TId2 id2, IDictionary<string, object> fields);
	}
}