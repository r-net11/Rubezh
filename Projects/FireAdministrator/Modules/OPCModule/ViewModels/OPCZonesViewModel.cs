using System.Collections.ObjectModel;
using System.Linq;
using FiresecClient;
using Infrastructure.Common.Windows.ViewModels;

namespace OPCModule.ViewModels
{
	public class OPCZonesViewModel : ViewPartViewModel, ISelectable<int>
	{
		public void Initialize()
		{
			Zones = new ObservableCollection<OPCZoneViewModel>(
				from zone in FiresecManager.Zones
				orderby zone.No
				select new OPCZoneViewModel(zone));
			SelectedZone = Zones.FirstOrDefault();
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

		#region ISelectable<int> Members

		public void Select(int zoneNo)
		{
			Initialize();
			if (zoneNo != 0)
				SelectedZone = Zones.FirstOrDefault(x => x.Zone.No == zoneNo);
		}

		#endregion
	}
}