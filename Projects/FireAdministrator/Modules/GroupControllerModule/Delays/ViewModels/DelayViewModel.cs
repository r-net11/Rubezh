using RubezhAPI.GK;
using RubezhClient;
using Infrastructure;
using Infrastructure.Common.Windows;
using Infrastructure.Common.Windows.Windows;
using Infrastructure.Common.Windows.Windows.ViewModels;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using RubezhAPI;

namespace GKModule.ViewModels
{
	public class DelayViewModel : BaseViewModel
	{
		public GKDelay Delay { get; private set; }
		public DelayViewModel(GKDelay delay)
		{
			ShowLogicCommand = new RelayCommand(OnShowLogic);
			Delay = delay;
			Delay.Changed += Update;
			Update();
		}

		public void Update()
		{
			this.VisualizationState = Delay.PlanElementUIDs.Count == 0 ? VisualizationState.NotPresent : (Delay.PlanElementUIDs.Count > 1 ? VisualizationState.Multiple : VisualizationState.Single);
			OnPropertyChanged(() => Delay);
			OnPropertyChanged(() => PresentationLogic);
		}

		public string PresentationLogic
		{
			get
			{
				var presentationLogic = GKManager.GetPresentationLogic(Delay.Logic);
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

		VisualizationState _visualizationState;
		public VisualizationState VisualizationState
		{
			get { return _visualizationState; }
			private set
			{
				this._visualizationState = value;
				base.OnPropertyChanged(() => this.VisualizationState);
			}
		}

		public RelayCommand ShowLogicCommand { get; private set; }
		void OnShowLogic()
		{
			DelaysViewModel.Current.SelectedDelay = this;
			var logicViewModel = new LogicViewModel(Delay, Delay.Logic, true);
			if (ServiceFactory.DialogService.ShowModalWindow(logicViewModel))
			{
				GKManager.SetDelayLogic(Delay, logicViewModel.GetModel());
				OnPropertyChanged(() => PresentationLogic);
				ServiceFactory.SaveService.GKChanged = true;
			}
		}
	}
}