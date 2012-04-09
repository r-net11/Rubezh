using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;
using FiresecService.Properties;

namespace FiresecService.DatabaseConverter
{
	public static class ConnectionManager
	{
		public static ConnectionStringSettings ConnectionSettings
		{
			get { return ConfigurationManager.ConnectionStrings[Settings.Default.FiresecConnection]; }
		}
		public static string ConnectionString
		{
			get { return ConnectionSettings.ConnectionString; }
		}

		public static FiresecDbConverterDataContext CreateFiresecDataContext()
		{
			return new FiresecDbConverterDataContext(ConnectionString);
		}
	}
}
