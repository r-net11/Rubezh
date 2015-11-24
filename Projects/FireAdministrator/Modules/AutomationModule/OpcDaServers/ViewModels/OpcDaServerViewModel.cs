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
			if (server == null)
			{
				throw new ArgumentNullException("server");
			}

			Base = server;
		}

		public OpcDaServerViewModel(RubezhAPI.Automation.OpcDaServer server)
		{
			if (server == null)
			{
				throw new ArgumentNullException("server");
			}

			Base = new OpcDaServer.OpcDaServer { Id = server.Id, ServerName = server.ServerName };
		}

		#endregion

		#region Fields And Properties

		public OpcDaServer.OpcDaServer Base { get; private set; }
		
		public string ServerName { get { return Base.ServerName; } }

		public RubezhAPI.Automation.OpcDaServer ConvertTo()
		{
			return new RubezhAPI.Automation.OpcDaServer 
						{ 
							ServerName = Base.ServerName, 
							Id = Base.Id 
						};
		}

		#endregion
	}
}
