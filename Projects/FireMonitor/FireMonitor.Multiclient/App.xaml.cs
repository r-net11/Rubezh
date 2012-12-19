using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Windows;

namespace FireMonitor.Multiclient
{
	public partial class App : Application
	{
		private const int InstanceCount = 2;

		protected override void OnStartup(StartupEventArgs e)
		{
			base.OnStartup(e);
		}

		protected override void OnExit(ExitEventArgs e)
		{
			base.OnExit(e);
			Environment.Exit(0);
		}
	}
}
