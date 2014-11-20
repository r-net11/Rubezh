using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
			if (Direction.PlanElementUIDs == null)
				Direction.PlanElementUIDs = new List<Guid>();
			_visualizetionState = Direction.PlanElementUIDs.Count == 0 ? VisualizationState.NotPresent : (Direction.PlanElementUIDs.Count > 1 ? VisualizationState.Multiple : VisualizationState.Single);
			OnPropertyChanged(() => VisualizationState);
		}

		public string PresentationLogic
		{
			get
			{
				var presentationZone = GKManager.GetPresentationZone(Direction.Logic);
				IsZoneGrayed = string.IsNullOrEmpty(presentationZone);
				if (string.IsNullOrEmpty(presentationZone))
				{
					presentationZone = "Нажмите для настройки логики";
				}
				return presentationZone;
			}
		}

		bool _isZoneGrayed;
		public bool IsZoneGrayed
		{
			get { return _isZoneGrayed; }
			set
			{
				_isZoneGrayed = value;
				OnPropertyChanged(() => IsZoneGrayed);
			}
		}

		public RelayCommand ShowLogicCommand { get; private set; }
		void OnShowLogic()
		{
			DirectionsViewModel.Current.SelectedDirection = this;
			var logicViewModel = new LogicViewModel(null, Direction.Logic, true);
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