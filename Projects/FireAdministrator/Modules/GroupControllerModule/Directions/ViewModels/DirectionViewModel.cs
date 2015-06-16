using FiresecAPI.GK;
using FiresecClient;
using Infrastructure;
using Infrastructure.Common;
using Infrastructure.Common.Windows;
using Infrastructure.Common.Windows.ViewModels;

namespace GKModule.ViewModels
{
	public class DirectionViewModel : BaseViewModel
	{
		private VisualizationState _visualizetionState;
		public GKDirection Direction { get; set; }

		public DirectionViewModel(GKDirection direction)
		{
			ShowLogicCommand = new RelayCommand(OnShowLogic);
			Direction = direction;
			Update();
		}

		public void Update()
		{
			OnPropertyChanged(() => Direction);
			_visualizetionState = Direction.PlanElementUIDs.Count == 0 ? VisualizationState.NotPresent : (Direction.PlanElementUIDs.Count > 1 ? VisualizationState.Multiple : VisualizationState.Single);
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
			var logicViewModel = new LogicViewModel(null, Direction.Logic, true, hasStopClause:true);
			if (DialogService.ShowModalWindow(logicViewModel))
			{
				Direction.Logic = logicViewModel.GetModel();
				OnPropertyChanged(() => PresentationLogic);
				ServiceFactory.SaveService.GKChanged = true;
			}
		}

		public string Name
		{
			get { return Direction.Name; }
			set
			{
				Direction.Name = value;
				Direction.OnChanged();
				OnPropertyChanged(() => Name);
				ServiceFactory.SaveService.GKChanged = true;
			}
		}
		public string Description
		{
			get { return Direction.Description; }
			set
			{
				Direction.Description = value;
				Direction.OnChanged();
				OnPropertyChanged(() => Description);
				ServiceFactory.SaveService.GKChanged = true;
			}
		}
		public VisualizationState VisualizationState
		{
			get { return _visualizetionState; }
		}
		public void Update(GKDirection direction)
		{
			Direction = direction;
			OnPropertyChanged(() => Direction);
			OnPropertyChanged(() => Name);
			OnPropertyChanged(() => Description);
			Update();
		}

		#region OPC
		public bool IsOPCUsed
		{
			get { return Direction.IsOPCUsed; }
			set
			{
				Direction.IsOPCUsed = value;
				OnPropertyChanged("IsOPCUsed");
				ServiceFactory.SaveService.GKChanged = true;
			}
		}
		#endregion
	}
}