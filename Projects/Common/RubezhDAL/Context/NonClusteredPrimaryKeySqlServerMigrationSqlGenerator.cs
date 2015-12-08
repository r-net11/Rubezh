using System;
using System.Collections.Generic;
using System.Data.Entity.Migrations.Model;
using System.Data.Entity.SqlServer;
using System.Linq;
using System.Text;

namespace RubezhDAL.DataContext
{
	public class NonClusteredPrimaryKeySqlServerMigrationSqlGenerator : SqlServerMigrationSqlGenerator
	{
		protected override void Generate(AddPrimaryKeyOperation addPrimaryKeyOperation)
		{
			addPrimaryKeyOperation.IsClustered = false;
			base.Generate(addPrimaryKeyOperation);
		}

		protected override void Generate(CreateTableOperation createTableOperation)
		{
			createTableOperation.PrimaryKey.IsClustered = false;
			base.Generate(createTableOperation);
		}

		protected override void Generate(MoveTableOperation moveTableOperation)
		{
			moveTableOperation.CreateTableOperation.PrimaryKey.IsClustered = false;
			base.Generate(moveTableOperation);
		}
	}
}
