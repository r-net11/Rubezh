using System;
using System.Collections.Generic;
using System.Data.Entity.Migrations.Design;
using System.Data.Entity.Migrations.Model;
using System.Data.Entity.Migrations.Utilities;
using System.Linq;
using System.Text;

namespace RubezhDAL.DataContext
{
	public class NonClusteredPrimaryKeyCSharpMigrationCodeGenerator : CSharpMigrationCodeGenerator
	{
		protected override void Generate(AddPrimaryKeyOperation addPrimaryKeyOperation, IndentedTextWriter writer)
		{
			addPrimaryKeyOperation.IsClustered = false;
			base.Generate(addPrimaryKeyOperation, writer);
		}
		protected override void GenerateInline(AddPrimaryKeyOperation addPrimaryKeyOperation, IndentedTextWriter writer)
		{
			addPrimaryKeyOperation.IsClustered = false;
			base.GenerateInline(addPrimaryKeyOperation, writer);
		}

		protected override void Generate(CreateTableOperation createTableOperation, IndentedTextWriter writer)
		{
			createTableOperation.PrimaryKey.IsClustered = false;
			base.Generate(createTableOperation, writer);
		}

		protected override void Generate(MoveTableOperation moveTableOperation, IndentedTextWriter writer)
		{
			moveTableOperation.CreateTableOperation.PrimaryKey.IsClustered = false;
			base.Generate(moveTableOperation, writer);
		}
	}
}
