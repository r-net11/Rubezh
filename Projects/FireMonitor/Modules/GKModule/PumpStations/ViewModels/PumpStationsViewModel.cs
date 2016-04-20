using System;
using System.Collections.Generic;
using System.Linq;
using RubezhClient;
using Infrastructure.Common.Windows.ViewModels;
using RubezhAPI;

namespace GKModule.ViewModels
{
	public class PumpStationsViewModel : ViewPartViewModel, ISelectable<Guid>
	{
		public void Initialize()
		{
			PumpStations = new List<PumpStationViewModel>();
			foreach (var pumpStation in GKManager.PumpStations.OrderBy(x => x.No))
			{
				var pumpStationViewModel = new PumpStationViewModel(pumpStation);
				PumpStations.Add(pumpStationViewModel);
			}
			SelectedPumpStation = PumpStations.FirstOrDefault();
		}

		List<PumpStationViewModel> _pumpStations;
		public List<PumpStationViewModel> PumpStations
		{
			get { return _pumpStations; }
			set
			{
				_pumpStations = value;
				OnPropertyChanged(() => PumpStations);
			}
		}

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

		public void Select(Guid pumpStationUID)
		{
			if (pumpStationUID != Guid.Empty)
			{
				SelectedPumpStation = PumpStations.FirstOrDefault(x => x.PumpStation.UID == pumpStationUID);
			}
		}
	}
}