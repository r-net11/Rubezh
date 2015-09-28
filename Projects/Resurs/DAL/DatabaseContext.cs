using System;
using System.Linq;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.Migrations;
using System.Data.Common;
using ResursAPI;

namespace ResursDAL
{
	public class DatabaseContext : DbContext, IDisposable
	{
		public DbSet<Apartment> Apartments { get; set; }
		public DbSet<Bill> Bills { get; set; }
		public DbSet<Device> Devices { get; set; }
		public DbSet<Measure> Measures { get; set; }
		public DbSet<Parameter> Parameters { get; set; }
		public DbSet<Tariff> Tariffs { get; set; }
		public DbSet<TariffPart> TariffParts { get; set; }
	
		public DatabaseContext(DbConnection connection)
			: base(connection, true)
		{
			Database.SetInitializer<DatabaseContext>(new MigrateDatabaseToLatestVersion<DatabaseContext, Configuration>(true));
		}

		public static bool CheckConnection()
		{
			try
			{
				using (var databaseContext = DatabaseContext.Initialize())
				{
					databaseContext.Measures.FirstOrDefault();
					return true;
				}
			}
			catch (Exception e)
			{
				return false;
			}
		}

		public static DatabaseContext Initialize()
		{
			var connectionFactory = new SqlConnectionFactory();
			var connection = connectionFactory.CreateConnection(@"Data Source=.\sqlexpress;Initial Catalog=RubezhResurs;Integrated Security=True");
			return new DatabaseContext(connection);
		}

		protected override void OnModelCreating(DbModelBuilder modelBuilder)
		{
			modelBuilder.Entity<Tariff>().HasMany(x => x.TariffParts).WithRequired(x => x.Tariff).WillCascadeOnDelete();
			modelBuilder.Entity<Apartment>().HasMany(x => x.Bills).WithRequired(x => x.Apartment).WillCascadeOnDelete();
			modelBuilder.Entity<Device>().HasMany(x => x.Parameters).WithRequired(x => x.Device).WillCascadeOnDelete();
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
