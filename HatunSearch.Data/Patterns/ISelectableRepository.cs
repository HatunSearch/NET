// Hatun Search | Layer: Data || Version: 2018.11.16.810
// (c) 2018 Hatun Search. All rights reserved.

// 'Using' directive
using HatunSearch.Entities.Patterns;
using System.Collections.Generic;

namespace HatunSearch.Data.Patterns
{
	public interface ISelectableRepository<TDTO> where TDTO : IDTO
	{
		IEnumerable<TDTO> SelectAll();
	}
	public interface ISelectableRepository<TDTO, TId> where TDTO : IDTO<TId>
	{
		TDTO SelectById(TId id);
	}
	public interface ISelectableRepository<TDTO, TId1, TId2> where TDTO : IDTO<TId1, TId2>
	{
		TDTO SelectById(TId1 id1, TId2 id2);
	}
}