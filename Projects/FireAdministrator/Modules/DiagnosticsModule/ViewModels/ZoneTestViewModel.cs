using System.Collections.ObjectModel;
using System.Diagnostics;
using DevicesModule.ViewModels;
using Infrastructure.Common.Windows.ViewModels;

namespace DiagnosticsModule.ViewModels
{
	public class ZoneTestViewModel : SaveCancelDialogViewModel
	{
		public static Stopwatch Stopwatch = new Stopwatch();

		public ZoneTestViewModel()
		{
			Title = "Выбор зоны устройства";

			Zones = new ObservableCollection<ZoneViewModel>();
			//foreach (var zone in FiresecManager.Zones)
			//{
			//    var zoneViewModel = new ZoneViewModel(zone);
			//    Zones.Add(zoneViewModel);
			//}
		}

		public ObservableCollection<ZoneViewModel> Zones { get; private set; }

		private ZoneViewModel _selectedZone;

		public ZoneViewModel SelectedZone
		{
			get { return _selectedZone; }
			set
			{
				_selectedZone = value;
				OnPropertyChanged("SelectedZone");
			}
		}
	}
}