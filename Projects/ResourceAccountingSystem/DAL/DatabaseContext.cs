using System;
using System.Linq;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.Migrations;
using System.Data.Common;
using DAL.DataClasses;

namespace DAL
{
	public class DatabaseContext : DbContext, IDisposable
	{
		public DbSet<Device> Devices { get; set; }
		public DbSet<Parameter> Parameters { get; set; }
		public DbSet<Tariff> Tariffs { get; set; }
		public DbSet<Measure> Measures { get; set; }

		public DatabaseContext(DbConnection connection)
			: base(connection, true)
		{
			Database.SetInitializer<DatabaseContext>(new MigrateDatabaseToLatestVersion<DatabaseContext, Configuration>(true));
		}

		public void Test()
		{
			Measures.FirstOrDefault();
		}

		public static DatabaseContext Initialize()
		{
			var connectionFactory = new SqlConnectionFactory();
			var connection = connectionFactory.CreateConnection(@"Data Source=.\sqlexpress;Initial Catalog=RubezhResurs;Integrated Security=True");
			return new DatabaseContext(connection);
		}

		void IDisposable.Dispose()
		{
			Dispose();
		}
	}

	internal sealed class Configuration : DbMigrationsConfiguration<DatabaseContext>
	{
		public Configuration()
		{
			AutomaticMigrationsEnabled = true;
			AutomaticMigrationDataLossAllowed = true;
		}
	}
}
