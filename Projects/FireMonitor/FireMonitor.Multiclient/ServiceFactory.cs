using System;
using System.Linq;
using System.Collections.Generic;
using System.Windows;
using FiresecAPI.Models;
using Infrastructure.Client.Login;
using Infrastructure.Common;
using Microsoft.Practices.Prism.Events;
using System.Threading;
using System.Diagnostics;
using FiresecAPI;

namespace FireMonitor.Multiclient
{
	public class ServiceFactory : ServiceFactoryBase
	{
		public static void Initialize()
		{
			Events = new EventAggregator();
			ResourceService = new ResourceService();
		}
	}
}