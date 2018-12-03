// Hatun Search | Layer: Business || Version: 2018.11.16.810
// (c) 2018 Hatun Search. All rights reserved.

// 'Using' directive
using HatunSearch.Entities.Patterns;
using System.Collections.Generic;

namespace HatunSearch.Business.Patterns
{
	public interface IReadableBLL<TDTO> where TDTO : IDTO
	{
		IEnumerable<TDTO> ReadAll();
	}
	public interface IReadableBLL<TDTO, TId> where TDTO : IDTO<TId>
	{
		TDTO ReadById(TId id);
	}
	public interface IReadableBLL<TDTO, TId1, TId2> where TDTO : IDTO<TId1, TId2>
	{
		TDTO ReadById(TId1 id1, TId2 id2);
	}
}