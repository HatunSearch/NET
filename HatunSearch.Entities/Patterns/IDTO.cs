// Hatun Search | Layer: Entities || Version: 2018.11.16.810
// (c) 2018 Hatun Search. All rights reserved.

// 'Using' directive
using HatunSearch.Entities.Data;

namespace HatunSearch.Entities.Patterns
{
	public interface IDTO
	{
		object Id { get; set; }
	}
	public interface IDTO<TId> : IDTO
	{
		new TId Id { get; set; }
	}
	public interface IDTO<TId1, TId2> : IDTO, IDTO<CompositeKey<TId1, TId2>>
	{
		new CompositeKey<TId1, TId2> Id { get; }
	}
}