using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace RubezhAPI
{
	public enum DbType
	{
		[Description("Postgre SQL")]
		Postgres,
		[Description("Microsoft SQL Server")]
		MsSql
	}
}
