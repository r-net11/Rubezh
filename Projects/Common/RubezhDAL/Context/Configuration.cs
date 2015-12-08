using RubezhDAL.DataClasses;
using System;
using System.Collections.Generic;
using System.Data.Entity.Migrations;
using System.Linq;
using System.Text;

namespace RubezhDAL.DataContext
{
	internal sealed class Configuration : DbMigrationsConfiguration<DatabaseContext>
	{
		public Configuration()
		{
			CodeGenerator = new NonClusteredPrimaryKeyCSharpMigrationCodeGenerator();
			if (DatabaseContext.IsPostgres)
				SetSqlGenerator("Npgsql", new NonClusteredPrimaryKeyNpgsqlMigrationSqlGenerator());
			else
				SetSqlGenerator("System.Data.SqlClient", new NonClusteredPrimaryKeySqlServerMigrationSqlGenerator());
			AutomaticMigrationsEnabled = true;
			AutomaticMigrationDataLossAllowed = true;
			ContextKey = "SKDDriver.DataClasses.Configuration";
		}

		protected override void Seed(DatabaseContext context)
		{
			context.Seed();
			base.Seed(context);
		}
	}
}
