using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.IO;

namespace FiresecNTService
{
	public partial class FiresecNTService : ServiceBase
	{
		public FiresecNTService()
		{
			InitializeComponent();
		}

		static void Main(string[] args)
		{
			ServiceBase[] ServicesToRun;
			ServicesToRun = new ServiceBase[] { new FiresecNTService() };
			ServiceBase.Run(ServicesToRun);
		}

		protected override void OnStart(string[] args)
		{
			base.OnStart(args);
			var commandLineArgs = Environment.GetCommandLineArgs();
			DirectoryInfo dirInfo = new DirectoryInfo(commandLineArgs[0]);
			var nameFiresecExe = commandLineArgs[1];
			var pathFiresecExe = dirInfo.FullName.Replace(dirInfo.Name, "") + nameFiresecExe;
			Process.Start(pathFiresecExe);
		}

		protected override void OnStop()
		{
			base.OnStop();
			var processes = Process.GetProcesses();
			foreach (var process in processes)
			{
				if (process.ProcessName == "FiresecService")
				{
					process.Kill();
				}
			}
		}
	}
}
