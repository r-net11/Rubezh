﻿using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using FiresecAPI.Models;
using FiresecAPI.SKD;
using FiresecClient;
using Infrastructure;
using Infrastructure.Common;
using Infrastructure.Common.Windows.ViewModels;
using Infrustructure.Plans.Elements;
using SKDModule.Events;
using Infrastructure.Events;
using Infrastructure.Common.Windows;
using FiresecAPI.GK;

namespace SKDModule.ViewModels
{
	public class ZoneDetailsViewModel : DialogViewModel, IWindowIdentity
	{
		public SKDZone Zone { get; private set; }
		public SKDZoneState State
		{
			get { return Zone.State; }
		}

		public ZoneDetailsViewModel(SKDZone zone)
		{
			ShowCommand = new RelayCommand(OnShow);
			OpenCommand = new RelayCommand(OnOpen, CanOpen);
			CloseCommand = new RelayCommand(OnClose, CanClose);
			DetectEmployeesCommand = new RelayCommand(OnDetectEmployees, CanDetectEmployees);

			Zone = zone;
			State.StateChanged += new Action(OnStateChanged);
			InitializePlans();

			Title = Zone.Name;
			TopMost = true;
		}

		void OnStateChanged()
		{
			OnPropertyChanged("State");
			OnPropertyChanged("ResetFireCommand");
			OnPropertyChanged("SetIgnoreCommand");
			OnPropertyChanged("ResetIgnoreCommand");
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
				elementBase = plan.ElementRectangleSKDZones.FirstOrDefault(x => x.ZoneUID == Zone.UID);
				if (elementBase != null)
				{
					var alarmPlanViewModel = new PlanLinkViewModel(plan, elementBase);
					alarmPlanViewModel.Zone = Zone;
					Plans.Add(alarmPlanViewModel);
					continue;
				}

				elementBase = plan.ElementPolygonSKDZones.FirstOrDefault(x => x.ZoneUID == Zone.UID);
				if (elementBase != null)
				{
					var alarmPlanViewModel = new PlanLinkViewModel(plan, elementBase);
					alarmPlanViewModel.Zone = Zone;
					Plans.Add(alarmPlanViewModel);
				}
			}
		}

		public RelayCommand OpenCommand { get; private set; }
		void OnOpen()
		{
			if (ServiceFactory.SecurityService.Validate())
			{
				var result = FiresecManager.FiresecService.SKDOpenZone(Zone.UID);
				if (result.HasError)
				{
					MessageBoxService.ShowWarning(result.Error);
				}
			}
		}
		bool CanOpen()
		{
			return FiresecManager.CheckPermission(PermissionType.Oper_ControlDevices) && Zone.State.StateClass != XStateClass.On && Zone.State.StateClass != XStateClass.ConnectionLost;
		}

		public RelayCommand CloseCommand { get; private set; }
		void OnClose()
		{
			if (ServiceFactory.SecurityService.Validate())
			{
				var result = FiresecManager.FiresecService.SKDCloseZone(Zone.UID);
				if (result.HasError)
				{
					MessageBoxService.ShowWarning(result.Error);
				}
			}
		}
		bool CanClose()
		{
			return FiresecManager.CheckPermission(PermissionType.Oper_ControlDevices) && Zone.State.StateClass != XStateClass.Off && Zone.State.StateClass != XStateClass.ConnectionLost;
		}

		public RelayCommand DetectEmployeesCommand { get; private set; }
		void OnDetectEmployees()
		{
		}
		bool CanDetectEmployees()
		{
			return true;
		}

		public RelayCommand ShowCommand { get; private set; }
		void OnShow()
		{
			ServiceFactory.Events.GetEvent<ShowSKDZoneEvent>().Publish(Zone.UID);
		}

		public bool CanControl
		{
			get { return FiresecManager.CheckPermission(PermissionType.Oper_ControlDevices); }
		}

		#region IWindowIdentity Members
		public string Guid
		{
			get { return Zone.UID.ToString(); }
		}
		#endregion

		public override void OnClosed()
		{
			State.StateChanged -= new Action(OnStateChanged);
		}
	}
}