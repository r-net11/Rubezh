using System.Collections.ObjectModel;
using System.Linq;
using FiresecAPI.GK;
using FiresecAPI.SKD;
using Infrastructure.Common.Windows.ViewModels;

namespace AutomationModule.ViewModels
{
	public class SKDZoneSelectionViewModel : SaveCancelDialogViewModel
	{
		public SKDZoneSelectionViewModel(SKDZone zone)
		{
			Title = Resources.Language.ObjectSelection.ViewModels.SKDZoneSelectionViewModel.Title;
			Zones = new ObservableCollection<ZoneViewModel>();
			SKDManager.Zones.ForEach(x => Zones.Add(new ZoneViewModel(x)));
			if (zone != null)
				SelectedZone = Zones.FirstOrDefault(x => x.SKDZone.UID == zone.UID);
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