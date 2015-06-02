using System;
using FiresecAPI.GK;
using FiresecAPI.Models;
using FiresecAPI.SKD;
using FiresecClient;
using Infrastructure;
using Infrastructure.Common;
using Infrastructure.Common.Windows;
using Infrastructure.Common.Windows.ViewModels;
using Infrastructure.Events;

namespace StrazhModule.ViewModels
{
	public class ZoneViewModel : BaseViewModel
	{
		public SKDZone Zone { get; private set; }
		public SKDZoneState State { get { return Zone.State; } }

		public ZoneViewModel(SKDZone zone)
		{
			Zone = zone;
			State.StateChanged -= new Action(OnStateChanged);
			State.StateChanged += new Action(OnStateChanged);
			OnStateChanged();

			OpenCommand = new RelayCommand(OnOpen, CanOpen);
			CloseCommand = new RelayCommand(OnClose, CanClose);
			OpenForeverCommand = new RelayCommand(OnOpenForever, CanOpenForever);
			CloseForeverCommand = new RelayCommand(OnCloseForever, CanCloseForever);
			ZoneAccessStateNormalCommand = new RelayCommand(OnZoneAccessStateNormal, CanZoneAccessStateNormal);
			ZoneAccessStateCloseAlwaysCommand = new RelayCommand(OnZoneAccessStateCloseAlways, CanZoneAccessStateCloseAlways);
			ZoneAccessStateOpenAlwaysCommand = new RelayCommand(OnZoneAccessStateOpenAlways, CanZoneAccessStateOpenAlways);
			ShowOnPlanCommand = new RelayCommand(OnShowOnPlan, CanShowOnPlan);
			ShowJournalCommand = new RelayCommand(OnShowJournal);
			ShowPropertiesCommand = new RelayCommand(OnShowProperties);
		}

		void OnStateChanged()
		{
			OnPropertyChanged(() => State);
		}

		public string PresentationName
		{
			get { return Zone.Name; }
		}

		public string PresentationDescription
		{
			get { return Zone.Description; }
		}

		public RelayCommand ShowOnPlanCommand { get; private set; }
		private void OnShowOnPlan()
		{
			ShowOnPlanHelper.ShowSKDZone(Zone);
		}
		private bool CanShowOnPlan()
		{
			return ShowOnPlanHelper.CanShowSKDZone(Zone);
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

		public RelayCommand ShowPropertiesCommand { get; private set; }
		private void OnShowProperties()
		{
			DialogService.ShowWindow(new ZoneDetailsViewModel(Zone));
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

		public RelayCommand ZoneAccessStateNormalCommand { get; private set; }
		void OnZoneAccessStateNormal()
		{
			if (ServiceFactory.SecurityService.Validate())
			{
				var result = FiresecManager.FiresecService.SKDZoneAccessStateNormal(Zone);
				if (result.HasError)
				{
					MessageBoxService.ShowWarning(result.Error);
				}
			}
		}
		bool CanZoneAccessStateNormal()
		{
			//return FiresecManager.CheckPermission(PermissionType.Oper_Strazh_Zones_Control) && State.StateClass != XStateClass.Off && State.StateClass != XStateClass.ConnectionLost;
			return FiresecManager.CheckPermission(PermissionType.Oper_Strazh_Zones_Control) && State.StateClass != XStateClass.ConnectionLost;
		}

		public RelayCommand ZoneAccessStateCloseAlwaysCommand { get; private set; }
		void OnZoneAccessStateCloseAlways()
		{
			if (ServiceFactory.SecurityService.Validate())
			{
				var result = FiresecManager.FiresecService.SKDZoneAccessStateCloseAlways(Zone);
				if (result.HasError)
				{
					MessageBoxService.ShowWarning(result.Error);
				}
			}
		}
		bool CanZoneAccessStateCloseAlways()
		{
			return FiresecManager.CheckPermission(PermissionType.Oper_Strazh_Zones_Control) && State.StateClass != XStateClass.Off && State.StateClass != XStateClass.ConnectionLost;
		}

		public RelayCommand ZoneAccessStateOpenAlwaysCommand { get; private set; }
		void OnZoneAccessStateOpenAlways()
		{
			if (ServiceFactory.SecurityService.Validate())
			{
				var result = FiresecManager.FiresecService.SKDZoneAccessStateOpenAlways(Zone);
				if (result.HasError)
				{
					MessageBoxService.ShowWarning(result.Error);
				}
			}
		}
		bool CanZoneAccessStateOpenAlways()
		{
			return FiresecManager.CheckPermission(PermissionType.Oper_Strazh_Zones_Control) && State.StateClass != XStateClass.On && State.StateClass != XStateClass.ConnectionLost;
		}

		public RelayCommand DetectEmployeesCommand { get; private set; }
		void OnDetectEmployees()
		{
		}
		bool CanDetectEmployees()
		{
			return true;
		}

		public bool IsBold { get; set; }
	}
}