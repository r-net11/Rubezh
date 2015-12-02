using System.Collections.Generic;
using System.Linq;
using Infrastructure.Common.Windows.ViewModels;
using AutomationModule.Models;
using RubezhAPI.Automation;

namespace AutomationModule.ViewModels
{
	public class OpcDaAddingServersViewModel : SaveCancelDialogViewModel
	{
		public OpcDaAddingServersViewModel(IEnumerable<RubezhAPI.Automation.OpcDaServer> servers)
		{
			Title = "Добавить серверы";

			AllServers = new List<OpcDaAddingServerViewModel>(servers
				.Select(server => new OpcDaAddingServerViewModel(server)));
		}

		public List<OpcDaAddingServerViewModel> AllServers { get; private set; }

		public RubezhAPI.Automation.OpcDaServer[] SelectedServers { get; private set; }

		protected override bool Save()
		{
			SelectedServers = AllServers
				.Where(server => server.IsSelected == true)
				.Select(s => s.Server).ToArray();
			return base.Save();
		}
	}
}