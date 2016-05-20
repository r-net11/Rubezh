using Infrastructure.Common.Windows.ViewModels;
using Integration.OPC.Models;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Integration.OPC.ViewModels
{
	public class AddZoneDialogViewModel : SaveCancelDialogViewModel
	{
		public ObservableCollection<OPCZone> Zones { get; private set; }

		public AddZoneDialogViewModel(IEnumerable<OPCZone> zones)
		{
			Zones = new ObservableCollection<OPCZone>(zones);
		}
	}
}
