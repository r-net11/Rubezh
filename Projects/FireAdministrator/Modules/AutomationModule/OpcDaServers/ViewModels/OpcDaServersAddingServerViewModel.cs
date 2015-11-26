using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Infrastructure.Common.Windows.ViewModels;

namespace AutomationModule.ViewModels
{
	public class OpcDaServersAddingServerViewModel : BaseViewModel
	{
		public OpcDaServersAddingServerViewModel(OpcDaServerViewModel server)
		{
			if (server == null)
			{
				throw new ArgumentNullException("server");
			}
			Server = server;
		}

		public OpcDaServerViewModel Server { get; private set; }

		public bool IsSelected { get; set; }
	}
}
