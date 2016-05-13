using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration.Install;
using System.Linq;
using System.ServiceProcess;

namespace StrazhService.WS
{
	[RunInstaller(true)]
	public partial class StrazhWindowsServiceInstaller : System.Configuration.Install.Installer
	{
		ServiceInstaller serviceInstaller;
		ServiceProcessInstaller processInstaller;

		public StrazhWindowsServiceInstaller()
		{
			InitializeComponent();

			serviceInstaller = new ServiceInstaller();
			processInstaller = new ServiceProcessInstaller();

			processInstaller.Account = ServiceAccount.LocalSystem;
			serviceInstaller.StartType = ServiceStartMode.Manual;
			serviceInstaller.ServiceName = "StrazhService";
			Installers.Add(processInstaller);
			Installers.Add(serviceInstaller);
		}
	}
}
