using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Infrastructure.Common.Windows.ViewModels;

namespace AutomationModule.ViewModels
{
	public class OpcDaServersAddingServerViewModel
	{
		private OpcDaServersAddingServerViewModel() { throw new NotImplementedException(); }
		public OpcDaServersAddingServerViewModel(OpcDaServerViewModel server)
		{
			if (server == null)
			{
				throw new ArgumentNullException("server");
			}
			Base = server;
		}

		public OpcDaServerViewModel Base { get; private set; }

		public bool IsSelected { get; set; }
		public string ServerName { get { return Base.ServerName; } }
	}
}
