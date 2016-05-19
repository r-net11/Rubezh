using System.Collections.ObjectModel;
using System.Linq;
using RubezhAPI.GK;
using RubezhClient;
using Infrastructure.Common.Windows.ViewModels;
using RubezhAPI;

namespace GKModule.ViewModels
{
	public class GuardZoneSelectionViewModel : SaveCancelDialogViewModel
	{
		public GuardZoneSelectionViewModel(GKGuardZone guardZone)
		{
			Title = "Выбор охранной зоны";
			Zones = new ObservableCollection<GuardZoneViewModel>();
			GKManager.GuardZones.ForEach(x => Zones.Add(new GuardZoneViewModel(x)));
			if (guardZone != null)
				SelectedZone = Zones.FirstOrDefault(x => x.Zone.UID == guardZone.UID);
			if (SelectedZone == null)
				SelectedZone = Zones.FirstOrDefault();
		}

		public ObservableCollection<GuardZoneViewModel> Zones { get; private set; }

		GuardZoneViewModel _selectedZone;
		public GuardZoneViewModel SelectedZone
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