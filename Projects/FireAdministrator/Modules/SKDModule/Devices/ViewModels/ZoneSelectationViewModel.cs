using System;
using System.Collections.ObjectModel;
using System.Linq;
using FiresecAPI.SKD;
using Infrastructure.Common.Windows;
using Infrastructure.Common.Windows.ViewModels;

namespace SKDModule.ViewModels
{
	[SaveSizeAttribute]
	public class ZoneSelectationViewModel : SaveCancelDialogViewModel
	{
		public ZoneSelectationViewModel(Guid zoneUID)
		{
			Title = "Выбор зоны";
			Zones = new ObservableCollection<ZoneViewModel>();
			foreach (var zone in SKDManager.Zones)
			{
				var zoneViewModel = new ZoneViewModel(zone);
				Zones.Add(zoneViewModel);
			}
			SelectedZone = Zones.FirstOrDefault(x => x.Zone.UID == zoneUID);
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

		protected override bool Save()
		{
			return base.Save();
		}
	}
}