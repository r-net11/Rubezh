using System;
using System.Linq;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.Migrations;
using System.Data.Common;
using ResursAPI;
using Common;

namespace ResursDAL
{
	public class DatabaseContext : DbContext
	{
		public DbSet<Consumer> Consumers { get; set; }
		public DbSet<Device> Devices { get; set; }
		public DbSet<Measure> Measures { get; set; }
		public DbSet<Parameter> Parameters { get; set; }
		public DbSet<Tariff> Tariffs { get; set; }
		public DbSet<TariffPart> TariffParts { get; set; }
		public DbSet<User> Users { get; set; }
		public DbSet<UserPermission> UserPermissions { get; set; }
		public DbSet<Journal> Journal { get; set; }
		public DbSet<Receipt> Receipts { get; set; }
	
		public DatabaseContext(DbConnection connection)
			: base(connection, true)
		{
			Database.SetInitializer<DatabaseContext>(new MigrateDatabaseToLatestVersion<DatabaseContext, Configuration>(true));
		}

		
		public static DatabaseContext Initialize()
		{
			var connectionFactory = new SqlConnectionFactory();
			var connection = connectionFactory.CreateConnection(SettingsManager.ResursSettings.ConnectionString);
			return new DatabaseContext(connection);
		}

		protected override void OnModelCreating(DbModelBuilder modelBuilder)
		{
			modelBuilder.Entity<Tariff>().HasMany(x => x.TariffParts).WithRequired(x => x.Tariff).WillCascadeOnDelete();
			modelBuilder.Entity<Device>().HasMany(x => x.Parameters).WithRequired(x => x.Device).WillCascadeOnDelete();
			modelBuilder.Entity<User>().HasMany(x => x.UserPermissions).WithRequired(x => x.User).WillCascadeOnDelete();
		}
	}

	internal sealed class Configuration : DbMigrationsConfiguration<DatabaseContext>
	{
		public Configuration()
		{
			AutomaticMigrationsEnabled = true;
			AutomaticMigrationDataLossAllowed = true;
		}

		protected override void Seed(DatabaseContext context)
		{
			if (!context.Users.Any())
			{
				var userpermissions = new List<UserPermission>();
				User user = new User() { Name = "Adm", Login = "Adm", PasswordHash = HashHelper.GetHashFromString("") };
				userpermissions.Add(new UserPermission() { User = user, PermissionType = PermissionType.ViewConsumer });
				userpermissions.Add(new UserPermission() { User = user, PermissionType = PermissionType.ViewDevice });
				userpermissions.Add(new UserPermission() { User = user, PermissionType = PermissionType.EditConsumer });
				userpermissions.Add(new UserPermission() { User = user, PermissionType = PermissionType.EditDevice });
				userpermissions.Add(new UserPermission() { User = user, PermissionType = PermissionType.EditUser });
				userpermissions.Add(new UserPermission() { User = user, PermissionType = PermissionType.ViewUser });
				userpermissions.Add(new UserPermission() { User = user, PermissionType = PermissionType.ViewJournal });
				userpermissions.Add(new UserPermission() { User = user, PermissionType = PermissionType.ViewPlot });
				userpermissions.Add(new UserPermission() { User = user, PermissionType = PermissionType.ViewReport });
				userpermissions.Add(new UserPermission() { User = user, PermissionType = PermissionType.ViewTariff });
				userpermissions.Add(new UserPermission() { User = user, PermissionType = PermissionType.EditTariff });
				user.UserPermissions = userpermissions;
				context.Users.Add(user);
				DBCash.Users = new List<User>();
				DBCash.Users.Add(user);
			}
			base.Seed(context);
		}
	}
}
