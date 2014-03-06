using FiresecAPI.Models;
using FiresecClient;
using GKModule.Events;
using Infrastructure;
using Infrastructure.Common;
using Infrastructure.Common.Windows;
using Infrastructure.Common.Windows.ViewModels;
using XFiresecAPI;

namespace GKModule.ViewModels
{
	public class ZoneViewModel : BaseViewModel
	{
		public XZone Zone { get; private set; }
		public XState State
		{
			get { return Zone.State; }
		}

		public ZoneViewModel(XZone zone)
		{
			ResetFireCommand = new RelayCommand(OnResetFire, CanResetFire);
			SetIgnoreCommand = new RelayCommand(OnSetIgnore, CanSetIgnore);
			ResetIgnoreCommand = new RelayCommand(OnResetIgnore, CanResetIgnore);
			SetIgnoreAllCommand = new RelayCommand(OnSetIgnoreAll, CanSetIgnoreAll);
			ResetIgnoreAllCommand = new RelayCommand(OnResetIgnoreAll, CanResetIgnoreAll);
			ShowOnPlanCommand = new RelayCommand(OnShowOnPlan, CanShowOnPlan);
			ShowJournalCommand = new RelayCommand(OnShowJournal);
			ShowPropertiesCommand = new RelayCommand(OnShowProperties);

			Zone = zone;
			State.StateChanged += new System.Action(OnStateChanged);
			OnStateChanged();
		}

		void OnStateChanged()
		{
			OnPropertyChanged("State");
		}
		
		public RelayCommand ShowOnPlanCommand { get; private set; }
		void OnShowOnPlan()
		{
			ShowOnPlanHelper.ShowZone(Zone);
		}
		public bool CanShowOnPlan()
		{
			return ShowOnPlanHelper.CanShowZone(Zone);
		}

		public RelayCommand ResetFireCommand { get; private set; }
		void OnResetFire()
		{
			if (ServiceFactory.SecurityService.Validate())
			{
				FiresecManager.FiresecService.GKReset(Zone);
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
				FiresecManager.FiresecService.GKSetIgnoreRegime(Zone);
			}
		}
		bool CanSetIgnore()
		{
			return !State.StateClasses.Contains(XStateClass.Ignore) && FiresecManager.CheckPermission(PermissionType.Oper_ControlDevices);
		}

		public RelayCommand ResetIgnoreCommand { get; private set; }
		void OnResetIgnore()
		{
			if (ServiceFactory.SecurityService.Validate())
			{
				FiresecManager.FiresecService.GKSetAutomaticRegime(Zone);
			}
		}
		bool CanResetIgnore()
		{
			return State.StateClasses.Contains(XStateClass.Ignore) && FiresecManager.CheckPermission(PermissionType.Oper_ControlDevices);
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
						FiresecManager.FiresecService.GKSetIgnoreRegime(device);
					}
				}
			}
		}
		bool CanSetIgnoreAll()
		{
			if (!FiresecManager.CheckPermission(PermissionType.Oper_AddToIgnoreList))
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
						FiresecManager.FiresecService.GKSetAutomaticRegime(device);
					}
				}
			}
		}
		bool CanResetIgnoreAll()
		{
			if (!FiresecManager.CheckPermission(PermissionType.Oper_AddToIgnoreList))
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
			var showXArchiveEventArgs = new ShowXArchiveEventArgs()
			{
				Zone = Zone
			};
			ServiceFactory.Events.GetEvent<ShowXArchiveEvent>().Publish(showXArchiveEventArgs);
		}

		public RelayCommand ShowPropertiesCommand { get; private set; }
		void OnShowProperties()
		{
			DialogService.ShowWindow(new ZoneDetailsViewModel(Zone));
		}
	}
}