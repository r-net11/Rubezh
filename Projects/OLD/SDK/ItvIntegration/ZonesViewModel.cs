using System.Collections.ObjectModel;
using FiresecClient.Itv;
using Infrastructure.Common;
using Infrastructure.Common.Windows.ViewModels;

namespace ItvIntegration
{
	public class ZonesViewModel : BaseViewModel
	{
		public ZonesViewModel()
		{
			SetZoneGuardCommand = new RelayCommand(OnSetZoneGuard, CanSetZoneGuard);
			UnSetZoneGuardCommand = new RelayCommand(OnUnSetZoneGuard, CanUnSetZoneGuard);

			Zones = new ObservableCollection<ZoneViewModel>();
			foreach (var zone in ItvManager.DeviceConfiguration.Zones)
			{
				var deviceViewModel = new ZoneViewModel(zone.ZoneState);
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
			ItvManager.SetZoneGuard(SelectedZone.ZoneState.Zone);
		}

		bool CanUnSetZoneGuard()
		{
			return SelectedZone != null;
		}
		public RelayCommand UnSetZoneGuardCommand { get; private set; }
		void OnUnSetZoneGuard()
		{
            ItvManager.UnSetZoneGuard(SelectedZone.ZoneState.Zone);
		}
	}
}