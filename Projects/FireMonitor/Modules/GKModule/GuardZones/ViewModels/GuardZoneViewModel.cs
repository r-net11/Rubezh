using FiresecAPI.GK;
using FiresecAPI.Models;
using GKModule.Events;
using Infrastructure;
using Infrastructure.Common;
using Infrastructure.Common.Windows;
using Infrastructure.Common.Windows.ViewModels;
using FiresecClient;

namespace GKModule.ViewModels
{
	public class GuardZoneViewModel : BaseViewModel
	{
		public XGuardZone Zone { get; private set; }
		public XGuardZone GuardZone { get { return Zone; } }
		public XState State
		{
			get
			{
				if (Zone.State == null)
					Zone.State = new XState();
				return Zone.State;
			}
		}

		public GuardZoneViewModel(XGuardZone zone)
		{
			TurnOnCommand = new RelayCommand(OnTurnOn);
			TurnOnNowCommand = new RelayCommand(OnTurnOnNow);
			TurnOffCommand = new RelayCommand(OnTurnOff);
			ResetCommand = new RelayCommand(OnReset, CanReset);
			ShowOnPlanCommand = new RelayCommand(OnShowOnPlan, CanShowOnPlan);
			ShowJournalCommand = new RelayCommand(OnShowJournal);
			ShowPropertiesCommand = new RelayCommand(OnShowProperties);

			Zone = zone;
			State.StateChanged += new System.Action(OnStateChanged);
			OnStateChanged();
		}

		void OnStateChanged()
		{
			OnPropertyChanged(() => State);
			OnPropertyChanged(() => Zone);
			OnPropertyChanged(() => GuardZone);
		}

		public RelayCommand ShowOnPlanCommand { get; private set; }
		public void OnShowOnPlan()
		{
			ShowOnPlanHelper.ShowGuardZone(Zone);
		}
		public bool CanShowOnPlan()
		{
			return ShowOnPlanHelper.CanShowGuardZone(Zone);
		}


		public RelayCommand TurnOnCommand { get; private set; }
		void OnTurnOn()
		{
			if (ServiceFactory.SecurityService.Validate())
			{
				FiresecManager.FiresecService.GKTurnOn(Zone);
			}
		}

		public RelayCommand TurnOnNowCommand { get; private set; }
		void OnTurnOnNow()
		{
			if (ServiceFactory.SecurityService.Validate())
			{
				FiresecManager.FiresecService.GKTurnOnNow(Zone);
			}
		}

		public RelayCommand TurnOffCommand { get; private set; }
		void OnTurnOff()
		{
			if (ServiceFactory.SecurityService.Validate())
			{
				FiresecManager.FiresecService.GKTurnOff(Zone);
			}
		}

		public RelayCommand ResetCommand { get; private set; }
		void OnReset()
		{
			if (ServiceFactory.SecurityService.Validate())
			{
				FiresecManager.FiresecService.GKReset(Zone);
			}
		}
		bool CanReset()
		{
			return State.StateClasses.Contains(XStateClass.Fire1);
		}

		public RelayCommand ShowJournalCommand { get; private set; }
		void OnShowJournal()
		{
			var showXArchiveEventArgs = new ShowXArchiveEventArgs()
			{
				GuardZone = Zone
			};
			ServiceFactory.Events.GetEvent<ShowXArchiveEvent>().Publish(showXArchiveEventArgs);
		}

		public RelayCommand ShowPropertiesCommand { get; private set; }
		void OnShowProperties()
		{
			DialogService.ShowWindow(new GuardZoneDetailsViewModel(Zone));
		}
	}
}