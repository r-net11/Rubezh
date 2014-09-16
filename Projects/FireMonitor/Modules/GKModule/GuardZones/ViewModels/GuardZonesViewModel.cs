using System;
using System.Collections.ObjectModel;
using System.Linq;
using FiresecClient;
using Infrastructure.Common.Windows.ViewModels;

namespace GKModule.ViewModels
{
	public class GuardZonesViewModel : ViewPartViewModel, ISelectable<Guid>
	{
		public static GuardZonesViewModel Current { get; private set; }
		public GuardZonesViewModel()
		{
			Current = this;
		}

		public void Initialize()
		{
			Zones = new ObservableCollection<GuardZoneViewModel>();
			foreach (var guardZone in XManager.DeviceConfiguration.GuardZones)
			{
				var zoneViewModel = new GuardZoneViewModel(guardZone);
				Zones.Add(zoneViewModel);
			}
			SelectedZone = Zones.FirstOrDefault();
		}

		ObservableCollection<GuardZoneViewModel> _zones;
		public ObservableCollection<GuardZoneViewModel> Zones
		{
			get { return _zones; }
			set
			{
				_zones = value;
				OnPropertyChanged(() => Zones);
			}
		}

		GuardZoneViewModel _selectedZone;
		public GuardZoneViewModel SelectedZone
		{
			get { return _selectedZone; }
			set
			{
				_selectedZone = value;
				OnPropertyChanged(() => SelectedZone);
			}
		}

		public void Select(Guid zoneUID)
		{
			if (zoneUID != Guid.Empty)
			{
				SelectedZone = Zones.FirstOrDefault(x => x.GuardZone.BaseUID == zoneUID);
			}
		}
	}
}