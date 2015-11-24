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
		#endregion

		#region Fields And Properties

		public OpcDaServer.OpcDaServer Base { get; private set; }
		
		public string ServerName { get { return Base.ServerName; }}

		#endregion
	}
}
