﻿using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using FiresecAPI.GK;
using FiresecAPI.Models;
using FiresecAPI.SKD;
using FiresecClient;
using Infrastructure;
using Infrastructure.Common;
using Infrastructure.Common.Windows;
using Infrastructure.Common.Windows.ViewModels;
using Infrastructure.Events;
using Infrustructure.Plans.Elements;

namespace StrazhModule.ViewModels
{
	public class DoorDetailsViewModel : DialogViewModel, IWindowIdentity
	{
		public SKDDoor Door { get; private set; }
		public SKDDoorState State
		{
			get { return Door.State; }
		}

		public DoorDetailsViewModel(SKDDoor door)
		{
			ShowCommand = new RelayCommand(OnShow);
			ShowJournalCommand = new RelayCommand(OnShowJournal);
			OpenCommand = new RelayCommand(OnOpen, CanOpen);
			CloseCommand = new RelayCommand(OnClose, CanClose);
			OpenForeverCommand = new RelayCommand(OnOpenForever, CanOpenForever);
			CloseForeverCommand = new RelayCommand(OnCloseForever, CanCloseForever);
			DoorAccessStateNormalCommand = new RelayCommand(OnDoorAccessStateNormal, CanDoorAccessStateNormal);
			DoorAccessStateCloseAlwaysCommand = new RelayCommand(OnDoorAccessStateCloseAlways, CanDoorAccessStateCloseAlways);
			DoorAccessStateOpenAlwaysCommand = new RelayCommand(OnDoorAccessStateOpenAlways, CanDoorAccessStateOpenAlways);

			Door = door;
			Title = Door.PresentationName;
			State.StateChanged -= new Action(OnStateChanged);
			State.StateChanged += new Action(OnStateChanged);
			InitializePlans();
		}

		void OnStateChanged()
		{
			OnPropertyChanged(() => State);
			CommandManager.InvalidateRequerySuggested();
		}

		public ObservableCollection<PlanLinkViewModel> Plans { get; private set; }
		public bool HasPlans
		{
			get { return Plans.Count > 0; }
		}

		void InitializePlans()
		{
			Plans = new ObservableCollection<PlanLinkViewModel>();
			foreach (var plan in FiresecManager.PlansConfiguration.AllPlans)
			{
				ElementBase elementBase;
				elementBase = plan.ElementDoors.FirstOrDefault(x => x.DoorUID == Door.UID);
				if (elementBase != null)
				{
					var alarmPlanViewModel = new PlanLinkViewModel(plan, elementBase);
					alarmPlanViewModel.Door = Door;
					Plans.Add(alarmPlanViewModel);
					continue;
				}
			}
		}

		public RelayCommand OpenCommand { get; private set; }
		void OnOpen()
		{
			if (ServiceFactory.SecurityService.Validate())
			{
				var result = FiresecManager.FiresecService.SKDOpenDoor(Door);
				if (result.HasError)
				{
					MessageBoxService.ShowWarning(result.Error);
				}
			}
		}
		bool CanOpen()
		{
			return FiresecManager.CheckPermission(PermissionType.Oper_Strazh_Doors_Control) && Door.State.StateClass != XStateClass.On && Door.State.StateClass != XStateClass.ConnectionLost;
		}

		public RelayCommand CloseCommand { get; private set; }
		void OnClose()
		{
			if (ServiceFactory.SecurityService.Validate())
			{
				var result = FiresecManager.FiresecService.SKDCloseDoor(Door);
				if (result.HasError)
				{
					MessageBoxService.ShowWarning(result.Error);
				}
			}
		}
		bool CanClose()
		{
			return FiresecManager.CheckPermission(PermissionType.Oper_Strazh_Doors_Control) && Door.State.StateClass != XStateClass.Off && Door.State.StateClass != XStateClass.ConnectionLost;
		}

		public RelayCommand OpenForeverCommand { get; private set; }
		void OnOpenForever()
		{
			if (ServiceFactory.SecurityService.Validate())
			{
				var result = FiresecManager.FiresecService.SKDOpenDoorForever(Door);
				if (result.HasError)
				{
					MessageBoxService.ShowWarning(result.Error);
				}
			}
		}
		bool CanOpenForever()
		{
			return FiresecManager.CheckPermission(PermissionType.Oper_Strazh_Doors_Control) && State.StateClass != XStateClass.On && State.StateClass != XStateClass.ConnectionLost;
		}

		public RelayCommand CloseForeverCommand { get; private set; }
		void OnCloseForever()
		{
			if (ServiceFactory.SecurityService.Validate())
			{
				var result = FiresecManager.FiresecService.SKDCloseDoorForever(Door);
				if (result.HasError)
				{
					MessageBoxService.ShowWarning(result.Error);
				}
			}
		}
		bool CanCloseForever()
		{
			return FiresecManager.CheckPermission(PermissionType.Oper_Strazh_Doors_Control) && State.StateClass != XStateClass.Off && State.StateClass != XStateClass.ConnectionLost;
		}

		public RelayCommand DoorAccessStateNormalCommand { get; private set; }
		void OnDoorAccessStateNormal()
		{
			if (ServiceFactory.SecurityService.Validate())
			{
				var result = FiresecManager.FiresecService.SKDDoorAccessStateNormal(Door);
				if (result.HasError)
				{
					MessageBoxService.ShowWarning(result.Error);
				}
			}
		}
		bool CanDoorAccessStateNormal()
		{
			//return FiresecManager.CheckPermission(PermissionType.Oper_Strazh_Doors_Control) && State.StateClass != XStateClass.Off && State.StateClass != XStateClass.ConnectionLost;
			return FiresecManager.CheckPermission(PermissionType.Oper_Strazh_Doors_Control) && State.StateClass != XStateClass.ConnectionLost;
		}

		public RelayCommand DoorAccessStateCloseAlwaysCommand { get; private set; }
		void OnDoorAccessStateCloseAlways()
		{
			if (ServiceFactory.SecurityService.Validate())
			{
				var result = FiresecManager.FiresecService.SKDDoorAccessStateCloseAlways(Door);
				if (result.HasError)
				{
					MessageBoxService.ShowWarning(result.Error);
				}
			}
		}
		bool CanDoorAccessStateCloseAlways()
		{
			return FiresecManager.CheckPermission(PermissionType.Oper_Strazh_Doors_Control) && State.StateClass != XStateClass.Off && State.StateClass != XStateClass.ConnectionLost;
		}

		public RelayCommand DoorAccessStateOpenAlwaysCommand { get; private set; }
		void OnDoorAccessStateOpenAlways()
		{
			if (ServiceFactory.SecurityService.Validate())
			{
				var result = FiresecManager.FiresecService.SKDDoorAccessStateOpenAlways(Door);
				if (result.HasError)
				{
					MessageBoxService.ShowWarning(result.Error);
				}
			}
		}
		bool CanDoorAccessStateOpenAlways()
		{
			CommandManager.InvalidateRequerySuggested();
			return FiresecManager.CheckPermission(PermissionType.Oper_Strazh_Doors_Control) && State.StateClass != XStateClass.On && State.StateClass != XStateClass.ConnectionLost;

		}

		public RelayCommand ShowCommand { get; private set; }
		void OnShow()
		{
			ServiceFactory.Events.GetEvent<ShowSKDDoorEvent>().Publish(Door.UID);
		}

		public RelayCommand ShowJournalCommand { get; private set; }
		void OnShowJournal()
		{
			var showSKDArchiveEventArgs = new ShowArchiveEventArgs()
			{
				SKDDoor = Door
			};
			ServiceFactory.Events.GetEvent<ShowArchiveEvent>().Publish(showSKDArchiveEventArgs);
		}

		public bool CanControl
		{
			get { return FiresecManager.CheckPermission(PermissionType.Oper_Strazh_Doors_Control); }
		}

		#region IWindowIdentity Members
		public string Guid
		{
			get { return Door.UID.ToString(); }
		}
		#endregion

		public override void OnClosed()
		{
			State.StateChanged -= new Action(OnStateChanged);
		}
	}
}