using System;
using RubezhAPI;
using System.Data.Common;
using System.Data.Entity.Infrastructure;
using Infrastructure.Common;
using Common;

namespace RubezhDAL.DataClasses
{
	public static class DbServiceHelper
	{
		public static OperationResult<T> ConcatOperationResults<T>(params OperationResult<T>[] results)
		{
			var result = new OperationResult<T>();
			foreach (var item in results)
			{
				if (item.HasError)
				{
					result.Errors.Add(item.Error);
				}
			}
			return result;
		}

		public static DbConnection CreateConnection()
		{
			IDbConnectionFactory connectionFactory;
			var connectionString = GlobalSettingsHelper.GlobalSettings.DbConnectionString;
			switch (GlobalSettingsHelper.GlobalSettings.DbType)
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

		public static OperationResult<T> InTryCatch<T>(Func<T> function)
		{
			try
			{
				return new OperationResult<T>(function());
			}
			catch (System.Exception e)
			{
				Logger.Error(e);
				return OperationResult<T>.FromError(e.Message);
			}
		}
	}
}