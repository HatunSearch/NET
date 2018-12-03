// Hatun Search | Layer: Business || Version: 2018.11.16.810
// (c) 2018 Hatun Search. All rights reserved.

// 'Using' directive
using HatunSearch.Entities.Patterns;

namespace HatunSearch.Business.Patterns
{
	public interface ICreatableBLL<TDTO, TResult> where TDTO : IDTO where TResult : struct
	{
		TResult Create(TDTO item);
	}
}