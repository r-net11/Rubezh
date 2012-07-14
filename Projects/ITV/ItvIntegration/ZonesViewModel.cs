using System.Collections.ObjectModel;
using FiresecClient;

namespace ItvIntegration
{
	public class ZonesViewModel : BaseViewModel
	{
		public ZonesViewModel()
		{
			SetZoneGuardCommand = new RelayCommand(OnSetZoneGuard, CanSetZoneGuard);
			UnSetZoneGuardCommand = new RelayCommand(OnUnSetZoneGuard, CanUnSetZoneGuard);

			Zones = new ObservableCollection<ZoneViewModel>();
			foreach (var zoneState in ItvManager.DeviceStates.ZoneStates)
			{
				var deviceViewModel = new ZoneViewModel(zoneState);
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
				OnPropertyChanged("SelectedZone");
			}
		}

		bool CanSetZoneGuard()
		{
			return SelectedZone != null;
		}
		public RelayCommand SetZoneGuardCommand { get; private set; }
		void OnSetZoneGuard()
		{
			ItvManager.SetZoneGuard(SelectedZone.ZoneState.No);
		}

		bool CanUnSetZoneGuard()
		{
			return SelectedZone != null;
		}
		public RelayCommand UnSetZoneGuardCommand { get; private set; }
		void OnUnSetZoneGuard()
		{
			ItvManager.UnSetZoneGuard(SelectedZone.ZoneState.No);
		}
	}
}