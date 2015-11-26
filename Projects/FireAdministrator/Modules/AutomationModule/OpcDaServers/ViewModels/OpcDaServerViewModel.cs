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

		public OpcDaServerViewModel() { throw new NotImplementedException(); }

		public OpcDaServerViewModel(RubezhAPI.Automation.OpcDaServer server)
		{
			Base = server;
			ServerName = server.ServerName;
			Id = server.Id;
		}

		#endregion

		#region Fields And Properties

		//public OpcDaServer.OpcDaServer Base { get; private set; }
		public RubezhAPI.Automation.OpcDaServer Base { get; protected set; }

		public string ServerName { get; protected set; }
		public Guid Id { get; protected set; }

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
