// Hatun Search | Layer: Testing || Version: 2018.11.16.810
// (c) 2018 Hatun Search. All rights reserved.

// 'Using' directive
using HatunSearch.Data.Databases;
using System.Configuration;

namespace HatunSearch.Testing
{
	public static class TestApp
	{
		private readonly static string connectionString = ConfigurationManager.ConnectionStrings["SQLServer"].ConnectionString;
		
		public static Connector Connector => new SQLServerConnector(connectionString);
	}
}