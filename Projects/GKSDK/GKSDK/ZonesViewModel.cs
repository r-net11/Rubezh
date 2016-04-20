using System.Collections.ObjectModel;
using RubezhClient;
using Infrastructure.Common;
using Infrastructure.Common.Windows.ViewModels;
using RubezhAPI.GK;
using RubezhAPI.Models;
using RubezhAPI;

namespace GKSDK
{
	public class ZonesViewModel : BaseViewModel
	{
		public ZonesViewModel()
		{
            SetZoneGuardCommand = new RelayCommand(OnSetZoneGuard, CanSetZoneGuard);
            UnSetZoneGuardCommand = new RelayCommand(OnUnSetZoneGuard, CanUnSetZoneGuard);

			Zones = new ObservableCollection<ZoneViewModel>();
			foreach (var zone in GKManager.Zones)
			{
				var deviceViewModel = new ZoneViewModel(zone);
				Zones.Add(deviceViewModel);
			}
		}

		public ObservableCollection<ZoneViewModel> Zones { get; set; }

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
        public RelayCommand SetZoneGuardCommand { get; private set; }
        void OnSetZoneGuard()
        {
        }
        bool CanSetZoneGuard()
        {
            return SelectedZone != null;
        }
        public RelayCommand UnSetZoneGuardCommand { get; private set; }
        void OnUnSetZoneGuard()
        {
        }
        bool CanUnSetZoneGuard()
        {
            return SelectedZone != null;
        }
	}
}