using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Windows;
using Infrastructure.Common.Windows;
using FireMonitor.Multiclient.ViewModels;
using Infrastructure.Common;
using Infrastructure.Common.Navigation;

namespace FireMonitor.Multiclient
{
	public partial class App : Application
	{
		protected override void OnStartup(StartupEventArgs e)
		{
			base.OnStartup(e);
			ServiceFactory.Initialize();
			ApplicationService.Run(new ViewModels.MulticlientViewModel(1), true);
		}

		protected override void OnExit(ExitEventArgs e)
		{
			base.OnExit(e);
			Environment.Exit(0);
		}
	}
}