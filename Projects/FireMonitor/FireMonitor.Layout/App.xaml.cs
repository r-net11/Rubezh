using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Windows;
using Shell = FireMonitor;

namespace FireMonitor.Layout
{
	public partial class App : Shell.App
	{
		protected override Shell.Bootstrapper CreateBootstrapper()
		{
			return new Bootstrapper();
		}
	}
}