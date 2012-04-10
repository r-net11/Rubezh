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
		public static FiresecDbConverterDataContext CreateFiresecDataContext()
		{
			return new FiresecDbConverterDataContext(Settings.Default.FiresecConnectionString);
		}
	}
}
