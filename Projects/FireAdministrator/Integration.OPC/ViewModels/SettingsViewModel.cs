using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Infrastructure.Common;
using Infrastructure.Common.TreeList;
using Infrastructure.Common.Windows.ViewModels;
using Integration.OPC.Models;

namespace Integration.OPC.ViewModels
{
	public class SettingsViewModel : SaveCancelDialogViewModel
	{
		public OPCSettings Settings { get; set; }

		public RelayCommand PingCommand { get; set; }

		public SettingsViewModel()
		{
			//Settings = settings;
			PingCommand = new RelayCommand(OnPing);
		}

		public void OnPing()
		{
			//TODO: Ping connection with httpIntegrationClient
		}

	}
}
