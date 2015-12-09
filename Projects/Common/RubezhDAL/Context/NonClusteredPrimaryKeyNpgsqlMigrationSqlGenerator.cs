using Npgsql;
using System.Collections.Generic;
using System.Data.Entity.Migrations.Model;
using System.Data.Entity.Migrations.Sql;

namespace RubezhDAL.DataContext
{
	public class NonClusteredPrimaryKeyNpgsqlMigrationSqlGenerator : NpgsqlMigrationSqlGenerator
	{
		public override IEnumerable<MigrationStatement> Generate(IEnumerable<MigrationOperation> migrationOperations, string providerManifestToken)
		{
			foreach (var migrationOperation in migrationOperations)
			{
				if (migrationOperation is AddPrimaryKeyOperation)
				{
					(migrationOperation as AddPrimaryKeyOperation).IsClustered = false;
				}
				if (migrationOperation is CreateTableOperation)
				{
					(migrationOperation as CreateTableOperation).PrimaryKey.IsClustered = false;
				}
				if (migrationOperation is MoveTableOperation)
				{
					(migrationOperation as MoveTableOperation).CreateTableOperation.PrimaryKey.IsClustered = false;
				}
			}
			return base.Generate(migrationOperations, providerManifestToken);
		}
	}
}
