using RubezhAPI.GK;
using RubezhAPI.Models;
using RubezhClient;
using GKModule.Events;
using Infrastructure;
using Infrastructure.Common;
using Infrastructure.Common.Windows;
using Infrastructure.Common.Windows.ViewModels;
using Infrastructure.Events;
using System;
using System.Collections.Generic;

namespace GKModule.ViewModels
{
	public class ZoneViewModel : BaseViewModel
	{
		public GKZone Zone { get; private set; }
		public GKState State
		{
			get { return Zone.State; }
		}

		public ZoneViewModel(GKZone zone)
		{
			ResetFireCommand = new RelayCommand(OnResetFire, CanResetFire);
			SetIgnoreCommand = new RelayCommand(OnSetIgnore, CanSetIgnore);
			ResetIgnoreCommand = new RelayCommand(OnResetIgnore, CanResetIgnore);
			SetIgnoreAllCommand = new RelayCommand(OnSetIgnoreAll, CanSetIgnoreAll);
			ResetIgnoreAllCommand = new RelayCommand(OnResetIgnoreAll, CanResetIgnoreAll);
			ShowOnPlanCommand = new RelayCommand(OnShowOnPlan, CanShowOnPlan);
			ShowJournalCommand = new RelayCommand(OnShowJournal);
			ShowPropertiesCommand = new RelayCommand(OnShowProperties);
			ShowOnPlanOrPropertiesCommand = new RelayCommand(OnShowOnPlanOrProperties);

			Zone = zone;
			State.StateChanged += new System.Action(OnStateChanged);
			OnStateChanged();
		}

		void OnStateChanged()
		{
			OnPropertyChanged(() => State);
		}

		public RelayCommand ShowOnPlanOrPropertiesCommand { get; private set; }
		void OnShowOnPlanOrProperties()
		{
			if (ShowOnPlanHelper.ShowObjectOnPlan(Zone))
				DialogService.ShowWindow(new ZoneDetailsViewModel(Zone));
			ZonesViewModel.Current.SelectedZone = this;
		}
		
		public RelayCommand ShowOnPlanCommand { get; private set; }
		void OnShowOnPlan()
		{
			ShowOnPlanHelper.ShowObjectOnPlan(Zone);
		}

		public bool CanShowOnPlan()
		{
			return ShowOnPlanHelper.CanShowOnPlan(Zone);
		}

		public RelayCommand ResetFireCommand { get; private set; }
		void OnResetFire()
		{
			if (ServiceFactory.SecurityService.Validate())
			{
				ClientManager.RubezhService.GKReset(Zone);
			}
		}
		bool CanResetFire()
		{
			return State.StateClasses.Contains(XStateClass.Fire2) || State.StateClasses.Contains(XStateClass.Fire1) || State.StateClasses.Contains(XStateClass.Attention);
		}

		#region Ignore
		public RelayCommand SetIgnoreCommand { get; private set; }
		void OnSetIgnore()
		{
			if (ServiceFactory.SecurityService.Validate())
			{
				ClientManager.RubezhService.GKSetIgnoreRegime(Zone);
			}
		}
		bool CanSetIgnore()
		{
			return !State.StateClasses.Contains(XStateClass.Ignore) && ClientManager.CheckPermission(PermissionType.Oper_Zone_Control);
		}

		public RelayCommand ResetIgnoreCommand { get; private set; }
		void OnResetIgnore()
		{
			if (ServiceFactory.SecurityService.Validate())
			{
				ClientManager.RubezhService.GKSetAutomaticRegime(Zone);
			}
		}
		bool CanResetIgnore()
		{
			return State.StateClasses.Contains(XStateClass.Ignore) && ClientManager.CheckPermission(PermissionType.Oper_Zone_Control);
		}
		#endregion

		#region IgnoreAll
		public RelayCommand SetIgnoreAllCommand { get; private set; }
		void OnSetIgnoreAll()
		{
			if (ServiceFactory.SecurityService.Validate())
			{
				foreach (var device in Zone.Devices)
				{
					if (!device.State.StateClasses.Contains(XStateClass.Ignore))
					{
						ClientManager.RubezhService.GKSetIgnoreRegime(device);
					}
				}
			}
		}
		bool CanSetIgnoreAll()
		{
			if (!ClientManager.CheckPermission(PermissionType.Oper_Zone_Control))
				return false;
			foreach (var device in Zone.Devices)
			{
				if (!device.State.StateClasses.Contains(XStateClass.Ignore))
					return true;
			}
			return false;
		}

		public RelayCommand ResetIgnoreAllCommand { get; private set; }
		void OnResetIgnoreAll()
		{
			if (ServiceFactory.SecurityService.Validate())
			{
				foreach (var device in Zone.Devices)
				{
					if (device.State.StateClasses.Contains(XStateClass.Ignore))
					{
						ClientManager.RubezhService.GKSetAutomaticRegime(device);
					}
				}
			}
		}
		bool CanResetIgnoreAll()
		{
			if (!ClientManager.CheckPermission(PermissionType.Oper_Zone_Control))
				return false;
			foreach (var device in Zone.Devices)
			{
				if (device.State.StateClasses.Contains(XStateClass.Ignore))
					return true;
			}
			return false;
		}
		#endregion

		public RelayCommand ShowJournalCommand { get; private set; }
		void OnShowJournal()
		{
			if (Zone != null)
				ServiceFactory.Events.GetEvent<ShowArchiveEvent>().Publish(new List<Guid> { Zone.UID });
		}

		public RelayCommand ShowPropertiesCommand { get; private set; }
		void OnShowProperties()
		{
			DialogService.ShowWindow(new ZoneDetailsViewModel(Zone));
		}
	}
}