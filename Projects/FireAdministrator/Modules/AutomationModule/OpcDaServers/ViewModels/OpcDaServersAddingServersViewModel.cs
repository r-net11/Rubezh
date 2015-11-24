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
			_allServers = new List<OpcDaServersAddingServerViewModel>(servers
				.Select(server => new OpcDaServersAddingServerViewModel(server) { IsSelected = false }));
		}

		private List<OpcDaServersAddingServerViewModel> _allServers;

		public List<OpcDaServersAddingServerViewModel> AllServers
		{
			get { return _allServers; }
		}

		private List<OpcDaServerViewModel> _selectedServers = 
			new List<OpcDaServerViewModel>();
		
		public OpcDaServerViewModel[] SelectedServers 
		{ 
			get { return _selectedServers.ToArray(); }
		}

		protected override bool Save()
		{
			_selectedServers.AddRange(_allServers
				.Where(server => server.IsSelected == true)
				.Select(s => s.Base));
			return base.Save();
		}
	}
}
