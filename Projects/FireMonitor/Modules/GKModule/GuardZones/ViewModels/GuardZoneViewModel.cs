using FiresecAPI.GK;
using FiresecAPI.Models;
using GKModule.Events;
using Infrastructure;
using Infrastructure.Common;
using Infrastructure.Common.Windows;
using Infrastructure.Common.Windows.ViewModels;

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
			SetCommand = new RelayCommand(OnSet, CanSet);
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


		public RelayCommand SetCommand { get; private set; }
		void OnSet()
		{
			if (ServiceFactory.SecurityService.Validate())
			{
				//FiresecManager.FiresecService.GKSetGuardZone(Zone);
			}
		}
		bool CanSet()
		{
			return true;
		}

		public RelayCommand ResetCommand { get; private set; }
		void OnReset()
		{
			if (ServiceFactory.SecurityService.Validate())
			{
				//FiresecManager.FiresecService.GKResetGuardZone(Zone);
			}
		}
		bool CanReset()
		{
			return true;
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