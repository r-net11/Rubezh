using RubezhAPI.GK;
using RubezhClient;
using GKModule.ViewModels;
using Infrastructure;
using Infrastructure.Common;
using Infrastructure.Common.Windows;
using Infrastructure.Common.Windows.ViewModels;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using RubezhAPI;

namespace GKModule.ViewModels
{
	public class DirectionViewModel : BaseViewModel
	{
		public GKDirection Direction { get; private set; }

		public DirectionViewModel(GKDirection direction)
		{
			ShowLogicCommand = new RelayCommand(OnShowLogic);
			Direction = direction;
			Direction.Changed += Update;
			Update();
		}

		public void Update()
		{
			OnPropertyChanged(() => Direction);
			_visualizationState = Direction.PlanElementUIDs.Count == 0 ? VisualizationState.NotPresent : (Direction.PlanElementUIDs.Count > 1 ? VisualizationState.Multiple : VisualizationState.Single);
			OnPropertyChanged(() => VisualizationState);
			OnPropertyChanged(() => PresentationLogic);
		}

		public string PresentationLogic
		{
			get
			{
				var presentationLogic = GKManager.GetPresentationLogic(Direction.Logic);
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
			DirectionsViewModel.Current.SelectedDirection = this;
			var logicViewModel = new LogicViewModel(Direction, Direction.Logic, true, hasStopClause: true);
			if (ServiceFactory.DialogService.ShowModalWindow(logicViewModel))
			{
				GKManager.SetDirectionLogic(Direction, logicViewModel.GetModel());
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