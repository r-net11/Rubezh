using System;
using FiresecAPI;
using System.Data.Common;
using System.Data.Entity.Infrastructure;

namespace SKDDriver.DataClasses
{
	public static class DbServiceHelper
	{
		public static OperationResult ConcatOperationResults(params OperationResult[] results)
		{
			var result = new OperationResult();
			foreach (var item in results)
			{
				if (item.HasError)
				{
					result.HasError = true;
					result.Error = string.Format("{0} {1}", result.Error, item.Error);
				}
			}
			return result;
		}

		public static DbConnection CreateConnection(string connectionString, DbType contextType)
		{
			IDbConnectionFactory connectionFactory;
			switch (contextType)
			{
				case DbType.Postgres:
					connectionFactory = new Npgsql.NpgsqlConnectionFactory();
					return connectionFactory.CreateConnection(connectionString);
				case DbType.MsSql:
					connectionFactory = new SqlConnectionFactory();
					return connectionFactory.CreateConnection(connectionString);
				default:
					connectionFactory = new SqlConnectionFactory();
					return connectionFactory.CreateConnection(connectionString);
			}
		}
	}

	

}
