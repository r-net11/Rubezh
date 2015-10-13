using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Practices.Prism.Events;

namespace ControllerSDK
{
	public class ServiceFactory
	{
		private static ServiceFactory _instance;
		public static ServiceFactory Instance {
			get { return _instance ?? (_instance = new ServiceFactory()); }
		}

		protected ServiceFactory()
		{
			Events = new EventAggregator();
		}

		public IEventAggregator Events { get; private set; }
	}
}
