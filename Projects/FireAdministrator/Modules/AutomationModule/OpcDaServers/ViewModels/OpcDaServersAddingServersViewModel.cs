using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Infrastructure.Common.Windows.ViewModels;

namespace AutomationModule.ViewModels
{
	public class OpcDaServersAddingServersViewModel : SaveCancelDialogViewModel
	{
		private OpcDaServersAddingServersViewModel() { throw new NotImplementedException(); }

		public OpcDaServersAddingServersViewModel(IEnumerable<OpcDaServerViewModel> servers)
		{
			Title = "Добавить серверы";

			if (servers == null)
			{
				throw new ArgumentNullException("servers");
			}
			AllServers = new List<OpcDaServersAddingServerViewModel>(servers
				.Select(server => new OpcDaServersAddingServerViewModel(server)));
		}

		public List<OpcDaServersAddingServerViewModel> AllServers { get; private set; }

		public OpcDaServerViewModel[] SelectedServers { get; private set; }

		protected override bool Save()
		{
			SelectedServers = AllServers
				.Where(server => server.IsSelected == true)
				.Select(s => s.Base).ToArray();
			return base.Save();
		}
	}
}
