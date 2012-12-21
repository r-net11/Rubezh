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
		private const int InstanceCount = 10;

		protected override void OnStartup(StartupEventArgs e)
		{
			base.OnStartup(e);
			ServiceFactory.Initialize();
			ApplicationService.Run(new ViewModels.MulticlientViewModel(InstanceCount), true);
		}

		protected override void OnExit(ExitEventArgs e)
		{
			base.OnExit(e);
			Environment.Exit(0);
		}
	}
}
