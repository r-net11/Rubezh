using System.Collections.ObjectModel;
using System.Linq;
using FiresecAPI.GK;
using FiresecClient;
using Infrastructure.Common.Windows.ViewModels;

namespace AutomationModule.ViewModels
{
	public class ZoneSelectionViewModel : SaveCancelDialogViewModel
	{
		public ZoneSelectionViewModel(XZone zone)
		{
			Zones = new ObservableCollection<ZoneViewModel>();
			XManager.Zones.ForEach(x => Zones.Add(new ZoneViewModel(x)));
			if (zone != null)
				SelectedZone = Zones.FirstOrDefault(x => x.Zone.UID == zone.UID);
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

		protected override bool CanSave()
		{
			return SelectedZone != null;
		}
	}
}