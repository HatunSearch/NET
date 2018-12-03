// Hatun Search | Layer: Business || Version: 2018.11.16.810
// (c) 2018 Hatun Search. All rights reserved.

// 'Using' directive
using HatunSearch.Data.Databases;
using HatunSearch.Data.Patterns;

namespace HatunSearch.Business.Patterns
{
	public abstract class BLL<TRepository> where TRepository : Repository, new()
	{
		public BLL(Connector connector) => Repository = new TRepository() { Connector = connector };

		protected Connector Connector => Repository?.Connector;
		protected TRepository Repository { get; private set; }
	}
}