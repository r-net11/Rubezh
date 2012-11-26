using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FSAgentServer
{
	public static class AppSettings
	{
		public static int RemotePort { get; set; }

		static AppSettings()
		{
			RemotePort = Convert.ToInt32(System.Configuration.ConfigurationManager.AppSettings["RemotePort"] as string);
		}
	}
}