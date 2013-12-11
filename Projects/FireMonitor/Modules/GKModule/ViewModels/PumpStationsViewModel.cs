using System;
using System.Collections.Generic;
using System.Linq;
using FiresecClient;
using Infrastructure.Common.Windows.ViewModels;
using XFiresecAPI;
using GKProcessor;

namespace GKModule.ViewModels
{
	public class PumpStationsViewModel : ViewPartViewModel, ISelectable<Guid>
	{
		public void Initialize()
		{
			PumpStations = new List<PumpStationViewModel>();
			foreach (var pumpStation in XManager.PumpStations)
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
				OnPropertyChanged("PumpStations");
			}
		}

		PumpStationViewModel _selectedPumpStation;
		public PumpStationViewModel SelectedPumpStation
		{
			get { return _selectedPumpStation; }
			set
			{
				_selectedPumpStation = value;
				OnPropertyChanged("SelectedPumpStation");
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