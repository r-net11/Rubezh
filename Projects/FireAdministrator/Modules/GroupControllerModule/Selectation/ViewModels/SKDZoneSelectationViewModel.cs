using System.Collections.ObjectModel;
using System.Linq;
using RubezhAPI.GK;
using Infrastructure.Common.Windows;
using Infrastructure.Common.Windows.ViewModels;
using RubezhAPI;
using System;

namespace GKModule.ViewModels
{
    [SaveSizeAttribute]
	public class SKDZoneSelectationViewModel : SaveCancelDialogViewModel
	{
		public SKDZoneSelectationViewModel(GKSKDZone zone)
		{
			Title = "Выбор зоны";
			Zones = new ObservableCollection<GKSKDZone>(GKManager.SKDZones);
			if (zone != null)
				SelectedZone = Zones.FirstOrDefault(x => x.UID == zone.UID);
		}

		public ObservableCollection<GKSKDZone> Zones { get; private set; }

		GKSKDZone _selectedZone;
		public GKSKDZone SelectedZone
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