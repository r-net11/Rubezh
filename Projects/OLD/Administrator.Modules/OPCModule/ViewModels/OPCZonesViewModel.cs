using System;
using System.Collections.ObjectModel;
using System.Linq;
using FiresecClient;
using Infrastructure.Common.Windows.ViewModels;

namespace OPCModule.ViewModels
{
	public class OPCZonesViewModel : ViewPartViewModel, ISelectable<Guid>
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
				OnPropertyChanged(() => Zones);
			}
		}

		OPCZoneViewModel _selectedZone;
		public OPCZoneViewModel SelectedZone
		{
			get { return _selectedZone; }
			set
			{
				_selectedZone = value;
				OnPropertyChanged(() => SelectedZone);
			}
		}

		#region ISelectable<Guid> Members
		public void Select(Guid zoneUID)
		{
			Initialize();
			if (zoneUID != Guid.Empty)
				SelectedZone = Zones.FirstOrDefault(x => x.Zone.UID == zoneUID);
		}
		#endregion

		public override void OnShow()
		{
			//Initialize();
		}
	}
}