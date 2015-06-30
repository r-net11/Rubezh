using System.Collections.ObjectModel;
using System.Linq;
using FiresecClient;
using Infrastructure.Common.Windows.ViewModels;

namespace GKModule.ViewModels
{
	public class OPCZonesViewModel : ViewPartViewModel
	{
		public void Initialize()
		{
			Zones = new ObservableCollection<ZoneViewModel>();
			foreach (var zone in GKManager.DeviceConfiguration.SortedZones)
			{
				var zoneViewModel = new ZoneViewModel(zone);
				Zones.Add(zoneViewModel);
			}
			SelectedZone = Zones.FirstOrDefault();
		}

		ObservableCollection<ZoneViewModel> _zones;
		public ObservableCollection<ZoneViewModel> Zones
		{
			get { return _zones; }
			set
			{
				_zones = value;
				OnPropertyChanged(() => Zones);
			}
		}

		ZoneViewModel _selectedZone;
		public ZoneViewModel SelectedZone
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