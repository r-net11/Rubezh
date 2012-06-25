using System.Collections.ObjectModel;
using System.Linq;
using FiresecClient;
using Infrastructure.Common.Windows.ViewModels;

namespace OPCModule.ViewModels
{
	public class OPCZonesViewModel : ViewPartViewModel
	{
		public OPCZonesViewModel()
		{
		}

		public void Initialize()
		{
			Zones = new ObservableCollection<OPCZoneViewModel>(
				from zone in FiresecManager.DeviceConfiguration.Zones
				orderby zone.No
				select new OPCZoneViewModel(zone));

			if (Zones.Count > 0)
				SelectedZone = Zones[0];
		}

		ObservableCollection<OPCZoneViewModel> _zones;
		public ObservableCollection<OPCZoneViewModel> Zones
		{
			get { return _zones; }
			set
			{
				_zones = value;
				OnPropertyChanged("Zones");
			}
		}

		OPCZoneViewModel _selectedZone;
		public OPCZoneViewModel SelectedZone
		{
			get { return _selectedZone; }
			set
			{
				_selectedZone = value;
				OnPropertyChanged("SelectedZone");
			}
		}

		void SelectFirstZone()
		{
			if (Zones.Count > 0)
				SelectedZone = Zones[0];
			else
				SelectedZone = null;
		}

		public override void OnShow()
		{
		}

		public override void OnHide()
		{
		}
	}
}