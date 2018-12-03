// Hatun Search | Layer: Data || Version: 2018.11.16.810
// (c) 2018 Hatun Search. All rights reserved.

// 'Using' directive
using HatunSearch.Entities.Patterns;
using System;
using System.Collections.Generic;
using System.Data;
using System.Net;

namespace HatunSearch.Data.Databases
{
	public abstract class Connector
	{
		private bool isTransaction = false;

		public Connector(string connectionString) => ConnectionString = connectionString;

		public void CommitTransaction()
		{
			if (isTransaction)
			{
				Transaction.Commit();
				IsTransaction = false;
			}
		}
		public abstract IDataParameter CreateParameter(string key, IDTO value);
		public abstract IDataParameter CreateParameter(string key, IPAddress value);
		public abstract IDataParameter CreateParameter(string key, object value);
		public abstract int ExecuteNonQuery(string query, IDictionary<string, object> parameters = null);
		public abstract T ExecuteScalar<T>(string query, IDictionary<string, object> parameters = null);
		public abstract IEnumerable<T> ExecuteReader<T>(string query, Func<IDataReader, T> callback);
		public abstract IEnumerable<T> ExecuteReader<T>(string query, IDictionary<string, object> parameters, Func<IDataReader, T> callback);
		public abstract IDbConnection OpenConnection();
		public void RollbackTransaction()
		{
			if (isTransaction)
			{
				Transaction.Rollback();
				IsTransaction = false;
			}
		}

		protected IDbConnection Connection { get; set; }
		protected string ConnectionString { get; private set; }
		public bool IsTransaction
		{
			get => isTransaction;
			set
			{
				if (!isTransaction && value)
				{
					IDbConnection connection = OpenConnection();
					Connection = connection;
					Transaction = connection.BeginTransaction();
				}
				else if (isTransaction && !value)
				{
					Transaction.Dispose();
					Transaction = null;
					Connection.Close();
					Connection = null;
				}
				isTransaction = value;
			}
		}
		protected IDbTransaction Transaction { get; private set; }
	}
}