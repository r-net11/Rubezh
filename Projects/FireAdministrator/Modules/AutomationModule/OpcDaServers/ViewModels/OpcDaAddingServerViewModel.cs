using System;
using Infrastructure.Common.Windows.ViewModels;
using AutomationModule.Models;

namespace AutomationModule.ViewModels
{
	public class OpcDaAddingServerViewModel : BaseViewModel
	{
		public OpcDaAddingServerViewModel(OpcDaServerModel server)
		{
			if (server == null)
			{
				throw new ArgumentNullException("server");
			}
			Server = server;
		}

		public OpcDaServerModel Server { get; private set; }

		public bool IsSelected { get; set; }
	}
}