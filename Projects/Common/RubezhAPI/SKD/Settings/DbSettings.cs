using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RubezhAPI
{
	public class DbSettings
	{
		public string UserName { get; set; }
		public string Password { get; set; }
		public bool IsSQLAuthentication { get; set; }
		public string DbName { get; set; }
		public string Server { get; set; }
		public int Port { get; set; }
		public string DataSource { get; set; }
		public string ConnectionString { get; set; }
		public bool IsFullConnectionString { get; set; }
		public DbType DbType { get; set; }
	}
}
