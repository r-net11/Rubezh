using Infrastructure;
using Infrastructure.Common;
using Infrastructure.Common.Windows.ViewModels;
using RubezhAPI;
using RubezhAPI.GK;

namespace GKModule.ViewModels
{
	public class PumpStationViewModel : BaseViewModel
	{
		public GKPumpStation PumpStation { get; private set; }

		public PumpStationViewModel(GKPumpStation pumpStation)
		{
			ShowLogicCommand = new RelayCommand(OnShowLogic);
			PumpStation = pumpStation;
			PumpStation.Changed += Update;
			Update();
		}

		public void Update()
		{
			OnPropertyChanged(() => PumpStation);
			_visualizationState = PumpStation.PlanElementUIDs.Count == 0 ? VisualizationState.NotPresent : (PumpStation.PlanElementUIDs.Count > 1 ? VisualizationState.Multiple : VisualizationState.Single);
			OnPropertyChanged(() => VisualizationState);
			OnPropertyChanged(() => PresentationLogic);
		}

		public string PresentationLogic
		{
			get
			{
				var presentationLogic = GKManager.GetPresentationLogic(PumpStation.StartLogic);
				IsLogicGrayed = string.IsNullOrEmpty(presentationLogic);
				if (string.IsNullOrEmpty(presentationLogic))
				{
					presentationLogic = "Нажмите для настройки логики";
				}
				return presentationLogic;
			}
		}

		bool _isLogicGrayed;
		public bool IsLogicGrayed
		{
			get { return _isLogicGrayed; }
			set
			{
				_isLogicGrayed = value;
				OnPropertyChanged(() => IsLogicGrayed);
			}
		}

		public RelayCommand ShowLogicCommand { get; private set; }
		void OnShowLogic()
		{
			PumpStationsViewModel.Current.SelectedPumpStation = this;
			var logicViewModel = new LogicViewModel(PumpStation, PumpStation.StartLogic, true, hasStopClause: true);
			if (ServiceFactory.DialogService.ShowModalWindow(logicViewModel))
			{
				GKManager.SetPumpStationStartLogic(PumpStation, logicViewModel.GetModel());
				OnPropertyChanged(() => PresentationLogic);
				ServiceFactory.SaveService.GKChanged = true;
			}
		}

		VisualizationState _visualizationState;
		public VisualizationState VisualizationState
		{
			get { return _visualizationState; }
		}
	}
}