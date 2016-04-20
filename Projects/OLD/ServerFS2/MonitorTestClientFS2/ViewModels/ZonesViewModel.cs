using System.Collections.ObjectModel;
using System.Linq;
using Infrastructure.Common.Windows.Windows.ViewModels;
using ServerFS2;

namespace MonitorTestClientFS2.ViewModels
{
	public class ZonesViewModel : BaseViewModel
	{
		public ZonesViewModel()
		{
			Zones = new ObservableCollection<ZoneViewModel>();
			foreach (var zone in ConfigurationManager.Zones)
			{
				var zoneViewModel = new ZoneViewModel(zone);
				Zones.Add(zoneViewModel);
			}
			SelectedZone = Zones.FirstOrDefault();
		}

		public ObservableCollection<ZoneViewModel> Zones { get; private set; }

		ZoneViewModel _selectedZone;
		public ZoneViewModel SelectedZone
		{
			get { return _selectedZone; }
			set
			{
				_selectedZone = value;
				OnPropertyChanged("SelectedZone");
			}
		}
	}
}