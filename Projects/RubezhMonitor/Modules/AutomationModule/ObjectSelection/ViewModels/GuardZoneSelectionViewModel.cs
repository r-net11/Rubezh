using System.Collections.ObjectModel;
using System.Linq;
using RubezhAPI.GK;
using RubezhClient;
using Infrastructure.Common.Windows.ViewModels;
using RubezhAPI;

namespace AutomationModule.ViewModels
{
	public class GuardZoneSelectionViewModel : SaveCancelDialogViewModel
	{
		public GuardZoneSelectionViewModel(GKGuardZone guardZone)
		{
			Title = "Выбор охранной зоны";
			Zones = new ObservableCollection<ZoneViewModel>();
			GKManager.GuardZones.ForEach(x => Zones.Add(new ZoneViewModel(x)));
			if (guardZone != null)
				SelectedZone = Zones.FirstOrDefault(x => x.GuardZone.UID == guardZone.UID);
			if (SelectedZone == null)
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
				OnPropertyChanged(() => SelectedZone);
			}
		}
	}
}