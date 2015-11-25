using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OpcDaServer;
using Infrastructure.Common.Windows.ViewModels;

namespace AutomationModule.ViewModels
{
	public class OpcDaServerViewModel: BaseViewModel
	{
		#region Constructors

		private OpcDaServerViewModel() { throw new NotImplementedException(); }

		public OpcDaServerViewModel(OpcDaServer.OpcDaServer server)
		{
			ServerName = server.ServerName;
			Id = server.Id;
		}

		public OpcDaServerViewModel(RubezhAPI.Automation.OpcDaServer server)
		{
			Base = server;
			ServerName = server.ServerName;
			Id = server.Id;
		}

		#endregion

		#region Fields And Properties

		//public OpcDaServer.OpcDaServer Base { get; private set; }
		public RubezhAPI.Automation.OpcDaServer Base { get; private set; }

		public string ServerName { get; private set; }
		public Guid Id { get; private set; }

		public RubezhAPI.Automation.OpcDaServer ConvertTo()
		{
			return new RubezhAPI.Automation.OpcDaServer 
						{ 
							ServerName = ServerName, 
							Id = Id 
						};
		}

		#endregion
	}
}
