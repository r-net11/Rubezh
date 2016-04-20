using System.Collections.ObjectModel;
using System.Linq;
using RubezhAPI.GK;
using RubezhClient;
using GKModule.Events;
using Infrastructure;
using Infrastructure.Common.Windows;
using Infrastructure.Common.Windows.Windows;
using Infrastructure.Common.Windows.Windows.ViewModels;
using Infrastructure.Events;
using System;
using System.Collections.Generic;
using RubezhAPI;

namespace GKModule.ViewModels
{
	public class PumpStationViewModel : BaseViewModel
	{
		public GKPumpStation PumpStation { get; private set; }
		public GKState State
		{
			get { return PumpStation.State; }
		}

		public PumpStationViewModel(GKPumpStation pumpStation)
		{
			ShowJournalCommand = new RelayCommand(OnShowJournal);
			ShowOnPlanOrPropertiesCommand = new RelayCommand(ShowOnPlanOrProperties);
			ShowOnPlanCommand = new RelayCommand(OnShowOnPlan, CanShowOnPlan);
			PumpStation = pumpStation;
			State.StateChanged += new System.Action(OnStateChanged);
			OnStateChanged();

			Pumps = new ObservableCollection<DeviceViewModel>();
			foreach (var device in PumpStation.NSDevices)
			{
				var deviceViewModel = DevicesViewModel.Current.AllDevices.FirstOrDefault(x => x.Device == device);
				Pumps.Add(deviceViewModel);
			}
		}

		void OnStateChanged()
		{
			OnPropertyChanged(() => State);
			OnPropertyChanged(() => HasOnDelay);
			OnPropertyChanged(() => HasHoldDelay);
		}

		public ObservableCollection<DeviceViewModel> Pumps { get; private set; }

		public RelayCommand ShowJournalCommand { get; private set; }
		void OnShowJournal()
		{
			if (PumpStation != null)
				ServiceFactory.Events.GetEvent<ShowArchiveEvent>().Publish(new List<Guid> { PumpStation.UID });
		}

		public RelayCommand ShowOnPlanOrPropertiesCommand { get; private set; }
		void ShowOnPlanOrProperties()
		{
			if(ShowOnPlanHelper.ShowObjectOnPlan(PumpStation))
			DialogService.ShowWindow(new PumpStationDetailsViewModel(PumpStation));
		}

		public RelayCommand ShowOnPlanCommand { get; private set; }
		void OnShowOnPlan()
		{
			ShowOnPlanHelper.ShowObjectOnPlan(PumpStation);
		}
		public bool CanShowOnPlan()
		{
			return ShowOnPlanHelper.CanShowOnPlan(PumpStation);
		}

		public bool HasOnDelay
		{
			get { return State.StateClasses.Contains(XStateClass.TurningOn) && State.OnDelay > 0; }
		}
		public bool HasHoldDelay
		{
			get { return State.StateClasses.Contains(XStateClass.On) && State.HoldDelay > 0; }
		}

		public string StartPresentationName
		{
			get { return GKManager.GetPresentationLogic(PumpStation.StartLogic.OnClausesGroup); }
		}
		public string StopPresentationName
		{
			get { return GKManager.GetPresentationLogic(PumpStation.StopLogic.OnClausesGroup); }
		}
		public string AutomaticOffPresentationName
		{
			get { return GKManager.GetPresentationLogic(PumpStation.AutomaticOffLogic.OnClausesGroup); }
		}
	}
}