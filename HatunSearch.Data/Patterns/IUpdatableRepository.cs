// Hatun Search | Layer: Data || Version: 2018.11.16.810
// (c) 2018 Hatun Search. All rights reserved.

// 'Using' directive
using System.Collections.Generic;

namespace HatunSearch.Data.Patterns
{
	public interface IUpdatableRepository<TId>
	{
		int Update(TId id, IDictionary<string, object> fields);
	}
	public interface IUpdatableRepository<TId1, TId2>
	{
		int Update(TId1 id1, TId2 id2, IDictionary<string, object> fields);
	}
}