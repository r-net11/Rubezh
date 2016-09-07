using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration.Install;
using System.Linq;
using System.ServiceProcess;
using Localization.StrazhService.WS.Common;

namespace StrazhService.WS
{
	[RunInstaller(true)]
	public partial class StrazhWindowsServiceInstaller : Installer
	{
		public StrazhWindowsServiceInstaller()
		{
			InitializeComponent();

			Installers.Add(new ServiceProcessInstaller
			{
				Account = ServiceAccount.LocalSystem
			});
			Installers.Add(new ServiceInstaller
			{
				StartType = ServiceStartMode.Automatic,
				ServiceName = "StrazhService",
				Description = CommonResources.Server
			});
		}
	}
}
