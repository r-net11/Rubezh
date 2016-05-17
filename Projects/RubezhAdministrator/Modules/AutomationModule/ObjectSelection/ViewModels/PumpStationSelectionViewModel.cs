using System.Collections.ObjectModel;
using System.Linq;
using RubezhAPI;
using RubezhAPI.GK;
using Infrastructure.Common.Windows.ViewModels;
using RubezhAPI;

namespace AutomationModule.ViewModels
{
	public class PumpStationSelectionViewModel : SaveCancelDialogViewModel
	{
		public PumpStationSelectionViewModel(GKPumpStation pumpStation)
		{
			Title = "Выбор насосной станции";
			PumpStations = new ObservableCollection<PumpStationViewModel>();
			GKManager.PumpStations.ForEach(x => PumpStations.Add(new PumpStationViewModel(x)));
			if (pumpStation != null)
				SelectedPumpStation = PumpStations.FirstOrDefault(x => x.PumpStation.UID == pumpStation.UID);
			if (SelectedPumpStation == null)
				SelectedPumpStation = PumpStations.FirstOrDefault();
		}

		public ObservableCollection<PumpStationViewModel> PumpStations { get; private set; }

		PumpStationViewModel _selectedPumpStation;
		public PumpStationViewModel SelectedPumpStation
		{
			get { return _selectedPumpStation; }
			set
			{
				_selectedPumpStation = value;
				OnPropertyChanged(() => SelectedPumpStation);
			}
		}
	}
}