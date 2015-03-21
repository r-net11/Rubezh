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
			ShowJournalCommand = new RelayCommand(OnShowJournal);
			OpenCommand = new RelayCommand(OnOpen, CanOpen);
			CloseCommand = new RelayCommand(OnClose, CanClose);
			OpenForeverCommand = new RelayCommand(OnOpenForever, CanOpenForever);
			CloseForeverCommand = new RelayCommand(OnCloseForever, CanCloseForever);
			DetectEmployeesCommand = new RelayCommand(OnDetectEmployees, CanDetectEmployees);

			Zone = zone;
			Title = Zone.PresentationName;
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
				var result = FiresecManager.FiresecService.SKDOpenZone(Zone);
				if (result.HasError)
				{
					MessageBoxService.ShowWarning(result.Error);
				}
			}
		}
		bool CanOpen()
		{
			return FiresecManager.CheckPermission(PermissionType.Oper_Strazh_Zones_Control) && Zone.State.StateClass != XStateClass.On && Zone.State.StateClass != XStateClass.ConnectionLost;
		}

		public RelayCommand CloseCommand { get; private set; }
		void OnClose()
		{
			if (ServiceFactory.SecurityService.Validate())
			{
				var result = FiresecManager.FiresecService.SKDCloseZone(Zone);
				if (result.HasError)
				{
					MessageBoxService.ShowWarning(result.Error);
				}
			}
		}
		bool CanClose()
		{
			return FiresecManager.CheckPermission(PermissionType.Oper_Strazh_Zones_Control) && Zone.State.StateClass != XStateClass.Off && Zone.State.StateClass != XStateClass.ConnectionLost;
		}

		public RelayCommand OpenForeverCommand { get; private set; }
		void OnOpenForever()
		{
			if (ServiceFactory.SecurityService.Validate())
			{
				var result = FiresecManager.FiresecService.SKDOpenZoneForever(Zone);
				if (result.HasError)
				{
					MessageBoxService.ShowWarning(result.Error);
				}
			}
		}
		bool CanOpenForever()
		{
			return FiresecManager.CheckPermission(PermissionType.Oper_Strazh_Zones_Control) && State.StateClass != XStateClass.On && State.StateClass != XStateClass.ConnectionLost;
		}

		public RelayCommand CloseForeverCommand { get; private set; }
		void OnCloseForever()
		{
			if (ServiceFactory.SecurityService.Validate())
			{
				var result = FiresecManager.FiresecService.SKDCloseZoneForever(Zone);
				if (result.HasError)
				{
					MessageBoxService.ShowWarning(result.Error);
				}
			}
		}
		bool CanCloseForever()
		{
			return FiresecManager.CheckPermission(PermissionType.Oper_Strazh_Zones_Control) && State.StateClass != XStateClass.Off && State.StateClass != XStateClass.ConnectionLost;
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

		public RelayCommand ShowJournalCommand { get; private set; }
		void OnShowJournal()
		{
			var showSKDArchiveEventArgs = new ShowArchiveEventArgs()
			{
				SKDZone = Zone
			};
			ServiceFactory.Events.GetEvent<ShowArchiveEvent>().Publish(showSKDArchiveEventArgs);
		}

		public bool CanControl
		{
			get { return FiresecManager.CheckPermission(PermissionType.Oper_Strazh_Zones_Control); }
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