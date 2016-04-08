using RubezhAPI.GK;
using RubezhClient;
using GKModule.Events;
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
	public class DelayViewModel : BaseViewModel
	{
		public GKDelay Delay { get; private set; }
		public GKState State
		{
			get { return Delay.State; }
		}

		public DelayViewModel(GKDelay delay)
		{
			ShowJournalCommand = new RelayCommand(OnShowJournal);
			ShowOnPlanOrPropertiesCommand = new RelayCommand(OnShowOnPlanOrProperties);
			ShowOnPlanCommand = new RelayCommand(OnShowOnPlan, CanShowOnPlan);
			Delay = delay;
			State.StateChanged += new System.Action(OnStateChanged);
			OnStateChanged();
		}

		void OnStateChanged()
		{
			OnPropertyChanged(() => State);
			OnPropertyChanged(() => HasOnDelay);
			OnPropertyChanged(() => HasHoldDelay);
		}

		public RelayCommand ShowJournalCommand { get; private set; }
		void OnShowJournal()
		{
			if (Delay != null)
				ServiceFactory.Events.GetEvent<ShowArchiveEvent>().Publish(new List<Guid> { Delay.UID });
		}

		public RelayCommand ShowOnPlanOrPropertiesCommand { get; private set; }

		void OnShowOnPlanOrProperties()
		{
			var plan = ShowOnPlanHelper.GetPlan(Delay.UID);
			if (plan != null)
				ShowOnPlanHelper.ShowObjectOnPlan(plan, Delay.UID);
			else
				DialogService.ShowWindow(new DelayDetailsViewModel(Delay));
		}

		public RelayCommand ShowOnPlanCommand { get; private set; }
		void OnShowOnPlan()
		{
			ShowOnPlanHelper.ShowObjectOnPlan(ShowOnPlanHelper.GetPlan(Delay.UID), Delay.UID);
		}
		public bool CanShowOnPlan()
		{
			return ShowOnPlanHelper.GetPlan(Delay.UID) != null;
		}

		public bool HasOnDelay
		{
			get { return State.StateClasses.Contains(XStateClass.TurningOn) && State.OnDelay > 0; }
		}
		public bool HasHoldDelay
		{
			get { return State.StateClasses.Contains(XStateClass.On) && State.HoldDelay > 0; }
		}

		public string PresentationLogic
		{
			get
			{
				var presentationZone = GKManager.GetPresentationLogic(Delay.Logic);
				return presentationZone;
			}
		}
	}
}