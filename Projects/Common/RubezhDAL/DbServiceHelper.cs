using System;
using RubezhAPI;
using System.Data.Common;
using System.Data.Entity.Infrastructure;
using Infrastructure.Common.Windows;
using Common;
using Npgsql;
using System.Data.SqlClient;

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
			IDbConnectionFactory connectionFactory = null;
			var connectionString = "";
			switch (GlobalSettingsHelper.GlobalSettings.DbSettings.DbType)
			{
				case DbType.Postgres:
					connectionString = CreatePostgresConnectionString(GlobalSettingsHelper.GlobalSettings.DbSettings);
					connectionFactory = new Npgsql.NpgsqlConnectionFactory();
					break;
				case DbType.MsSql:
					connectionString = CreateMsSQLConnectionString(GlobalSettingsHelper.GlobalSettings.DbSettings);
					connectionFactory = new SqlConnectionFactory();
					break;
			}
			return connectionFactory.CreateConnection(connectionString);
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

		static string CreateMsSQLConnectionString(DbSettings dbSettings)
		{
			if (dbSettings.IsFullConnectionString)
				return dbSettings.ConnectionString;
			var builder = new SqlConnectionStringBuilder();
			builder.DataSource = dbSettings.DataSource;
			builder.InitialCatalog = dbSettings.DbName;
			if (dbSettings.IsSQLAuthentication)
			{
				builder.UserID = dbSettings.UserName;
				builder.Password = dbSettings.Password;
				builder.IntegratedSecurity = false;
			}
			else
			{
				builder.IntegratedSecurity = true;
			}
			return builder.ConnectionString;
		}

		static string CreatePostgresConnectionString(DbSettings dbSettings)
		{
			if (dbSettings.IsFullConnectionString)
				return dbSettings.ConnectionString;
			var builder = new NpgsqlConnectionStringBuilder();
			builder.Database = dbSettings.DbName;
			builder.Host = dbSettings.Server;
			builder.Port = dbSettings.Port;
			if (dbSettings.IsSQLAuthentication)
			{
				builder.IntegratedSecurity = false;
				builder.UserName = dbSettings.UserName;
				builder.Password = dbSettings.Password;
			}
			return builder.ConnectionString;
		}
	}
}