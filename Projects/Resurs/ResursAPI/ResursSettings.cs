using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace ResursAPI
{
	[DataContract]
	public class ResursSettings
	{
		public ResursSettings()
		{
			//DbConnectionString =  @"Server=localhost;Database=RubezhResurs;User Id=asd;Password=1;";
			//DbType = DbType.Postgres;
			ConnectionString = @"Data Source=.\sqlexpress;Initial Catalog=RubezhResurs;Integrated Security=True";
			DbType = DbType.MsSql;

		}
		[DataMember]
		public string ConnectionString { get; set; }

		[DataMember]
		public DbType DbType { get; set; }
	}

	public enum DbType
	{
		[Description("Postgre SQL")]
		Postgres,
		[Description("Microsoft SQL Server")]
		MsSql
	}
}