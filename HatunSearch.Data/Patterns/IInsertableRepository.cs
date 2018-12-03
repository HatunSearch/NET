// Hatun Search | Layer: Data || Version: 2018.11.16.810
// (c) 2018 Hatun Search. All rights reserved.

// 'Using' directive
using HatunSearch.Entities.Patterns;

namespace HatunSearch.Data.Patterns
{
	public interface IInsertableRepository<TDTO> where TDTO : IDTO
	{
		void Insert(TDTO item);
	}
	public interface IInsertableRepository<TDTO, TResult> where TDTO : IDTO
	{
		void Insert(TDTO item, out TResult id);
	}
}