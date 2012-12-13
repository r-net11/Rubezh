using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace MultiClientRunner
{
	public class AppItem
	{
		Process Process;

		public void Initialize()
		{

		}

		public void Run()
		{
			var login = "adm";
			var password = "";
			var regime = "multiclient";
			var commandLineArguments = "regime='" + regime + "' login='" + login + "' password='" + password + "'";

			var processStartInfo = new ProcessStartInfo()
			{
				FileName = @"D:/Projects/Projects/FireMonitor/bin/Debug/FireMonitor.exe",
				Arguments = commandLineArguments
			};
			Process = System.Diagnostics.Process.Start(processStartInfo);
		}
	}
}