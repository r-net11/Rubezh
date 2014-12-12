using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using FiresecAPI.GK;
using Infrastructure.Common.Windows;
using Infrastructure.Common.Windows.ViewModels;
using FiresecAPI.SKD;

namespace GKModule.ViewModels
{
	[SaveSizeAttribute]
	public class SKDZoneSelectationViewModel : SaveCancelDialogViewModel
	{
		public SKDZoneSelectationViewModel(SKDZone zone)
		{
			Title = "Выбор зоны";
			Zones = new ObservableCollection<SKDZone>(SKDManager.Zones);
			if (zone != null)
				SelectedZone = Zones.FirstOrDefault(x => x.UID == zone.UID);
		}

		public ObservableCollection<SKDZone> Zones { get; private set; }

		SKDZone _selectedZone;
		public SKDZone SelectedZone
		{
			get { return _selectedZone; }
			set
			{
				_selectedZone = value;
				OnPropertyChanged(() => SelectedZone);
			}
		}
	}
}