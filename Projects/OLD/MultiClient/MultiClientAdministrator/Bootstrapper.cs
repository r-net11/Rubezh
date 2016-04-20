using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Infrastructure.Common.Windows;
using Infrastructure.Common.Windows.Windows;
using Common;

namespace MultiClientAdministrator
{
	public static class Bootstrapper
	{
		public static void Run()
		{
			var resourceService = new ResourceService();
			resourceService.AddResource(new ResourceDescription(typeof(Bootstrapper).Assembly, "DataTemplates/Dictionary.xaml"));
			resourceService.AddResource(new ResourceDescription(typeof(ApplicationService).Assembly, "Windows/DataTemplates/Dictionary.xaml"));
		}
	}
}