using System.Collections.ObjectModel;
using System.Linq;
using FiresecClient;
using Infrastructure.Common.Windows.Windows.ViewModels;

namespace AdministratorTestClientFS2.ViewModels
{
	public class ZonesViewModel : BaseViewModel
	{
		public static ZonesViewModel Current { get; private set; }

		public ZonesViewModel()
		{
			Current = this;
		}

		public void Initialize()
		{
			ZoneDevices = new ZoneDevicesViewModel();
			Zones = new ObservableCollection<ZoneViewModel>(
				from zone in FiresecManager.Zones
				orderby zone.No
				select new ZoneViewModel(zone));
			SelectedZone = Zones.FirstOrDefault();
		}

		ZoneDevicesViewModel _zoneDevices;
		public ZoneDevicesViewModel ZoneDevices
		{
			get { return _zoneDevices; }
			set
			{
				_zoneDevices = value;
				OnPropertyChanged("ZoneDevices");
			}
		}

		ObservableCollection<ZoneViewModel> _zones;
		public ObservableCollection<ZoneViewModel> Zones
		{
			get { return _zones; }
			set
			{
				_zones = value;
				OnPropertyChanged("Zones");
			}
		}

		ZoneViewModel _selectedZone;
		public ZoneViewModel SelectedZone
		{
			get { return _selectedZone; }
			set
			{
				_selectedZone = value;
				if (value != null)
					ZoneDevices.Initialize(value.Zone);
				else
					ZoneDevices.Clear();

				OnPropertyChanged("SelectedZone");
			}
		}
	}
}