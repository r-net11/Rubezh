using System.Collections.ObjectModel;
using System.Linq;
using RubezhAPI.GK;
using RubezhClient;
using Infrastructure;
using Infrastructure.Common;
using Infrastructure.Common.Windows;
using Infrastructure.Common.Windows.ViewModels;
using Infrastructure.Events;
using System;
using System.Collections.Generic;
using RubezhAPI;

namespace GKModule.ViewModels
{
	public class MPTViewModel : BaseViewModel
	{
		public GKMPT MPT { get; private set; }
		public GKState State
		{
			get { return MPT.State; }
		}
		public MPTDetailsViewModel MPTDetailsViewModel { get; private set; }

		public MPTViewModel(GKMPT mpt)
		{
			ShowJournalCommand = new RelayCommand(OnShowJournal);
			ShowOnPlanCommand = new RelayCommand(OnShowOnPlan, CanShowOnPlan);
			ShowOnPlanOrPropertiesCommand = new RelayCommand(OnShowOnPlanOrProperties);

			MPT = mpt;
			MPTDetailsViewModel = new MPTDetailsViewModel(MPT);
			State.StateChanged -= OnStateChanged;
			State.StateChanged += OnStateChanged;
			OnStateChanged();

			MPTDevices = new ObservableCollection<MPTDeviceViewModel>();
			foreach (var mptDevice in MPT.MPTDevices)
			{
				var deviceViewModel = DevicesViewModel.Current.AllDevices.FirstOrDefault(x => x.Device.UID == mptDevice.DeviceUID);
				MPTDevices.Add(new MPTDeviceViewModel(deviceViewModel, mptDevice.MPTDeviceType));
			}
		}

		void OnStateChanged()
		{
			OnPropertyChanged(() => State);
			OnPropertyChanged(() => HasOnDelay);
		}

		public ObservableCollection<MPTDeviceViewModel> MPTDevices { get; private set; }

		public RelayCommand ShowJournalCommand { get; private set; }
		void OnShowJournal()
		{
			if (MPT != null)
				ServiceFactory.Events.GetEvent<ShowArchiveEvent>().Publish(new List<Guid> { MPT.UID });
		}

		//public RelayCommand ShowPropertiesCommand { get; private set; }
		//void OnShowProperties()
		//{
		//	DialogService.ShowWindow(new MPTDetailsViewModel(MPT));
		//}

		public bool HasOnDelay
		{
			get { return State.StateClasses.Contains(XStateClass.TurningOn) && State.OnDelay > 0; }
		}

		public string StartPresentationName
		{
			get { return GKManager.GetPresentationLogic(MPT.MptLogic.OnClausesGroup); }
		}

		public string StopPresentationName
		{
			get { return GKManager.GetPresentationLogic(MPT.MptLogic.OffClausesGroup); }
		}

		public string SuspendPresentationName
		{
			get { return GKManager.GetPresentationLogic(MPT.MptLogic.StopClausesGroup); }
		}

		public RelayCommand ShowOnPlanCommand { get; private set; }
		void OnShowOnPlan()
		{
			ShowOnPlanHelper.ShowObjectOnPlan(MPT);
		}
		public bool CanShowOnPlan()
		{
			return ShowOnPlanHelper.CanShowOnPlan(MPT);
		}

		public RelayCommand ShowOnPlanOrPropertiesCommand { get; private set; }

		void OnShowOnPlanOrProperties()
		{
			if (ShowOnPlanHelper.ShowObjectOnPlan(MPT))
				DialogService.ShowWindow(new MPTDetailsViewModel(MPT));
		}
	}
}