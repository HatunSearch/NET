// Hatun Search | Layer: Data || Version: 2018.11.16.810
// (c) 2018 Hatun Search. All rights reserved.

// 'Using' directive
using HatunSearch.Entities.Patterns;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Net;

namespace HatunSearch.Data.Databases
{
	public sealed class SQLServerConnector : Connector
	{
		public SQLServerConnector(string connectionString) : base(connectionString) { }

		public override IDataParameter CreateParameter(string key, IDTO value) => new SqlParameter($"@{key}", value.Id);
		public override IDataParameter CreateParameter(string key, IPAddress value) => new SqlParameter($"@{key}", value.ToString());
		public override IDataParameter CreateParameter(string key, object value)
		{
			if (value is IDTO dto) return CreateParameter(key, dto);
			else if (value is IPAddress ipAddress) return CreateParameter(key, ipAddress);
			else return new SqlParameter($"@{key}", value ?? DBNull.Value);
		}
		public override int ExecuteNonQuery(string query, IDictionary<string, object> parameters = null)
		{
			IDbConnection connection = OpenConnection();
			int result = -1;
			using (IDbCommand command = connection.CreateCommand())
			{
				if (IsTransaction) command.Transaction = Transaction;
				command.CommandText = query;
				if (parameters != null)
				{
					IDataParameterCollection commandParameters = command.Parameters;
					if (parameters != null)
					{
						foreach (KeyValuePair<string, object> parameter in parameters)
							commandParameters.Add(CreateParameter(parameter.Key, parameter.Value));
					}
				}
				result = command.ExecuteNonQuery();
			}
			if (!IsTransaction) connection.Close();
			return result;
		}
		public override T ExecuteScalar<T>(string query, IDictionary<string, object> parameters = null)
		{
			IDbConnection connection = OpenConnection();
			object result = null;
			using (IDbCommand command = connection.CreateCommand())
			{
				if (IsTransaction) command.Transaction = Transaction;
				command.CommandText = query;
				if (parameters != null)
				{
					IDataParameterCollection commandParameters = command.Parameters;
					if (parameters != null)
					{
						foreach (KeyValuePair<string, object> parameter in parameters)
							commandParameters.Add(CreateParameter(parameter.Key, parameter.Value));
					}
				}
				result = command.ExecuteScalar();
			}
			if (!IsTransaction) connection.Close();
			return result != null ? (T)result : default;
		}
		public override IEnumerable<T> ExecuteReader<T>(string query, Func<IDataReader, T> callback) => ExecuteReader(query, null, callback);
		public override IEnumerable<T> ExecuteReader<T>(string query, IDictionary<string, object> parameters, Func<IDataReader, T> callback) =>
			IsTransaction ? ExecuteReaderWithoutUsing(query, parameters, callback) : ExecuteReaderWithUsing(query, parameters, callback);
		private IEnumerable<T> ExecuteReaderWithoutUsing<T>(string query, IDictionary<string, object> parameters, Func<IDataReader, T> callback)
		{
			IDbConnection connection = OpenConnection();
			using (IDbCommand command = connection.CreateCommand())
			{
				if (IsTransaction) command.Transaction = Transaction;
				command.CommandText = query;
				if (parameters != null)
				{
					IDataParameterCollection commandParameters = command.Parameters;
					foreach (KeyValuePair<string, object> parameter in parameters)
						commandParameters.Add(CreateParameter(parameter.Key, parameter.Value));
				}
				using (IDataReader reader = command.ExecuteReader())
				{
					while (reader.Read())
						yield return callback(reader);
				}
			}
		}
		private IEnumerable<T> ExecuteReaderWithUsing<T>(string query, IDictionary<string, object> parameters, Func<IDataReader, T> callback)
		{
			using (IDbConnection connection = OpenConnection())
			{
				using (IDbCommand command = connection.CreateCommand())
				{
					command.CommandText = query;
					if (parameters != null)
					{
						IDataParameterCollection commandParameters = command.Parameters;
						foreach (KeyValuePair<string, object> parameter in parameters)
							commandParameters.Add(CreateParameter(parameter.Key, parameter.Value));
					}
					using (IDataReader reader = command.ExecuteReader())
					{
						while (reader.Read())
							yield return callback(reader);
					}
				}
			}
		}
		public override IDbConnection OpenConnection()
		{
			if (!IsTransaction)
			{
				IDbConnection connection = new SqlConnection(ConnectionString);
				connection.Open();
				return connection;
			}
			else return Connection;
		}
	}
}