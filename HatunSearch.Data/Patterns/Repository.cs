// Hatun Search | Layer: Data || Version: 2018.11.16.810
// (c) 2018 Hatun Search. All rights reserved.

// 'Using' directive
using HatunSearch.Data.Databases;
using System.Collections.Generic;
using System.Text;

namespace HatunSearch.Data.Patterns
{
	public abstract class Repository
	{
		public Repository() { }
		public Repository(Connector connector) => Connector = connector;

		protected int Update(string name, string idName, object idValue, IDictionary<string, object> fields)
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append($"UPDATE {name} SET ");
			bool hasAFieldBeenAddedBefore = false;
			foreach (KeyValuePair<string, object> field in fields)
			{
				if (hasAFieldBeenAddedBefore) stringBuilder.Append(", ");
				stringBuilder.Append($"{field.Key} = @{field.Key}");
				hasAFieldBeenAddedBefore = true;
			}
			stringBuilder.Append($" WHERE {idName} = @{idName}");
			string query = stringBuilder.ToString();
			fields.Add(idName, idValue);
			return Connector.ExecuteNonQuery(query, fields);
		}
		protected int Update(string name, string idName1, object idValue1, string idName2, object idValue2, IDictionary<string, object> fields)
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append($"UPDATE {name} SET ");
			bool hasAFieldBeenAddedBefore = false;
			foreach (KeyValuePair<string, object> field in fields)
			{
				if (hasAFieldBeenAddedBefore) stringBuilder.Append(", ");
				stringBuilder.Append($"{field.Key} = @{field.Key}");
				hasAFieldBeenAddedBefore = true;
			}
			stringBuilder.Append($" WHERE {idName1} = @{idName1} AND {idName2} = @{idName2}");
			string query = stringBuilder.ToString();
			fields.Add(idName1, idValue1);
			fields.Add(idName2, idValue2);
			return Connector.ExecuteNonQuery(query, fields);
		}

		public Connector Connector { get; set; }
	}
}