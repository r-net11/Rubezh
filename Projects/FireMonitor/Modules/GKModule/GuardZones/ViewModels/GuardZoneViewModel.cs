using Infrastructure;
using Infrastructure.Common.Windows;
using Infrastructure.Common.Windows.Windows;
using Infrastructure.Common.Windows.Windows.ViewModels;
using Infrastructure.Events;
using Infrustructure.Plans.Events;
using RubezhAPI.GK;
using RubezhAPI.Models;
using RubezhClient;
using System;
using System.Collections.Generic;
using System.Linq;

namespace GKModule.ViewModels
{
	public class GuardZoneViewModel : BaseViewModel
	{
		public GKGuardZone GuardZone { get; private set; }
		public GKState State
		{
			get { return GuardZone.State; }
		}

		public GuardZoneViewModel(GKGuardZone guardZone)
		{
			TurnOnCommand = new RelayCommand(OnTurnOn, CanControl);
			TurnOnNowCommand = new RelayCommand(OnTurnOnNow, CanControl);
			TurnOnInAutomaticCommand = new RelayCommand(OnTurnOnInAutomatic, CanControl);
			TurnOnNowInAutomaticCommand = new RelayCommand(OnTurnOnNowInAutomatic, CanControl);
			TurnOffCommand = new RelayCommand(OnTurnOff, CanControl);
			TurnOffInAutomaticCommand = new RelayCommand(OnTurnOffInAutomatic, CanControl);
			TurnOffNowInAutomaticCommand = new RelayCommand(OnTurnOffNowInAutomatic, CanControl);
			ResetCommand = new RelayCommand(OnReset, CanReset);
			ShowOnPlanCommand = new RelayCommand(OnShowOnPlan, CanShowOnPlan);
			ShowJournalCommand = new RelayCommand(OnShowJournal);
			ShowPropertiesCommand = new RelayCommand(OnShowProperties);
			ShowOnPlanOrPropertiesCommand = new RelayCommand(OnShowOnPlanOrProperties);

			GuardZone = guardZone;
			State.StateChanged += OnStateChanged;
			OnStateChanged();
		}

		void OnStateChanged()
		{
			OnPropertyChanged(() => State);
			OnPropertyChanged(() => GuardZone);
			OnPropertyChanged(() => State.StateClasses);
		}

		public RelayCommand ShowOnPlanOrPropertiesCommand { get; private set; }
		void OnShowOnPlanOrProperties()
		{
			if (ShowOnPlanHelper.ShowObjectOnPlan(GuardZone)) 
			DialogService.ShowWindow(new GuardZoneDetailsViewModel(GuardZone));
			GuardZonesViewModel.Current.SelectedZone = this;
		}

		public RelayCommand ShowOnPlanCommand { get; private set; }
		public void OnShowOnPlan()
		{
			ShowOnPlanHelper.ShowObjectOnPlan(GuardZone);
		}
		public bool CanShowOnPlan()
		{
			return ShowOnPlanHelper.CanShowOnPlan(GuardZone);
		}


		public RelayCommand TurnOnCommand { get; private set; }
		void OnTurnOn()
		{
			if (ServiceFactory.SecurityService.Validate())
			{
				ClientManager.FiresecService.GKTurnOn(GuardZone);
			}
		}

		public RelayCommand TurnOnNowCommand { get; private set; }
		void OnTurnOnNow()
		{
			if (ServiceFactory.SecurityService.Validate())
			{
				ClientManager.FiresecService.GKTurnOnNow(GuardZone);
			}
		}

		public RelayCommand TurnOnInAutomaticCommand { get; private set; }
		void OnTurnOnInAutomatic()
		{
			if (ServiceFactory.SecurityService.Validate())
			{
				if (!State.StateClasses.Contains(XStateClass.AutoOff))
					ClientManager.FiresecService.GKTurnOnInAutomatic(GuardZone);
				else
					ClientManager.FiresecService.GKTurnOn(GuardZone);
			}
		}

		public RelayCommand TurnOnNowInAutomaticCommand { get; private set; }
		void OnTurnOnNowInAutomatic()
		{
			if (ServiceFactory.SecurityService.Validate())
			{
				if (!State.StateClasses.Contains(XStateClass.AutoOff))
					ClientManager.FiresecService.GKTurnOnNowInAutomatic(GuardZone);
				else
					ClientManager.FiresecService.GKTurnOnNow(GuardZone);
			}
		}

		public RelayCommand TurnOffCommand { get; private set; }
		void OnTurnOff()
		{
			if (ServiceFactory.SecurityService.Validate())
			{
				ClientManager.FiresecService.GKTurnOff(GuardZone);
			}
		}

		public RelayCommand TurnOffInAutomaticCommand { get; private set; }
		void OnTurnOffInAutomatic()
		{
			if (ServiceFactory.SecurityService.Validate())
			{
				if (!State.StateClasses.Contains(XStateClass.AutoOff))
					ClientManager.FiresecService.GKTurnOffInAutomatic(GuardZone);
				else
					ClientManager.FiresecService.GKTurnOff(GuardZone);
			}
		}

		public RelayCommand TurnOffNowInAutomaticCommand { get; private set; }
		void OnTurnOffNowInAutomatic()
		{
			if (ServiceFactory.SecurityService.Validate())
			{
				if (!State.StateClasses.Contains(XStateClass.AutoOff))
					ClientManager.FiresecService.GKTurnOffNowInAutomatic(GuardZone);
				else
					ClientManager.FiresecService.GKTurnOffNow(GuardZone);
			}
		}

		public RelayCommand ResetCommand { get; private set; }
		void OnReset()
		{
			if (ServiceFactory.SecurityService.Validate())
			{
				ClientManager.FiresecService.GKReset(GuardZone);
			}
		}
		bool CanReset()
		{
			return State.StateClasses.Contains(XStateClass.Fire1);
		}

		public RelayCommand ShowJournalCommand { get; private set; }
		void OnShowJournal()
		{
			if (GuardZone != null)
				ServiceFactory.Events.GetEvent<ShowArchiveEvent>().Publish(new List<Guid> { GuardZone.UID });
		}

		public RelayCommand ShowPropertiesCommand { get; private set; }
		void OnShowProperties()
		{
			DialogService.ShowWindow(new GuardZoneDetailsViewModel(GuardZone));
		}

		public bool CanControl()
		{
			if (State != null && !State.StateClasses.Contains(XStateClass.Ignore))
			{
				if (GuardZone.IsExtraProtected && ClientManager.CheckPermission(PermissionType.Oper_ExtraGuardZone))
				{
					return ClientManager.CheckPermission(PermissionType.Oper_ExtraGuardZone);
				}
				return !GuardZone.IsExtraProtected && ClientManager.CheckPermission(PermissionType.Oper_GuardZone_Control);
			}
			return false;

		}
	}
}