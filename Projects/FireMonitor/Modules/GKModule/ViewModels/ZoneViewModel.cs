using FiresecAPI;
using FiresecAPI.Models;
using FiresecClient;
using Infrastructure.Common;
using Infrastructure.Common.Windows.ViewModels;
using XFiresecAPI;
using Infrastructure.Common.Windows;
using Infrastructure.Events;
using Infrastructure;

namespace GKModule.ViewModels
{
	public class ZoneViewModel : BaseViewModel
	{
		public XZoneState ZoneState { get; private set; }
		public XZone Zone
		{
			get { return ZoneState.Zone; }
		}

		public ZoneViewModel(XZoneState zoneState)
		{
			ResetFireCommand = new RelayCommand(OnResetFire, CanResetFire);
			SetIgnoreCommand = new RelayCommand(OnSetIgnore, CanSetIgnore);
			ResetIgnoreCommand = new RelayCommand(OnResetIgnore, CanResetIgnore);
			SetIgnoreAllCommand = new RelayCommand(OnSetIgnoreAll, CanSetIgnoreAll);
			ResetIgnoreAllCommand = new RelayCommand(OnResetIgnoreAll, CanResetIgnoreAll);
			ShowOnPlanCommand = new RelayCommand(OnShowOnPlan, CanShowOnPlan);
			ShowJournalCommand = new RelayCommand(OnShowJournal);
			ShowPropertiesCommand = new RelayCommand(OnShowProperties);

			ZoneState = zoneState;
			ZoneState.StateChanged += new System.Action(OnStateChanged);
			OnStateChanged();
		}

		void OnStateChanged()
		{
			OnPropertyChanged("ZoneState");
			OnPropertyChanged("ToolTip");
		}

		public string ToolTip
		{
			get
			{
				var toolTip = Zone.PresentationName;
                toolTip += "\n" + "Состояние: " + ZoneState.StateClass.ToDescription();
				toolTip += "\n" + "Количество датчиков для перевода в Пожар 1: " + Zone.Fire1Count.ToString();
				toolTip += "\n" + "Количество датчиков для перевода в Пожар 2: " + Zone.Fire2Count.ToString();
				return toolTip;
			}
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
			ObjectCommandSendHelper.ResetZone(Zone);
		}
		bool CanResetFire()
		{
			return ZoneState.StateBits.Contains(XStateBit.Fire2) || ZoneState.StateBits.Contains(XStateBit.Fire1) || ZoneState.StateBits.Contains(XStateBit.Attention);
		}

		#region Ignore
		public RelayCommand SetIgnoreCommand { get; private set; }
		void OnSetIgnore()
		{
			ObjectCommandSendHelper.SetIgnoreRegimeForZone(Zone);
		}
		bool CanSetIgnore()
		{
            return !ZoneState.StateBits.Contains(XStateBit.Ignore) && FiresecManager.CheckPermission(PermissionType.Oper_AddToIgnoreList);
		}

		public RelayCommand ResetIgnoreCommand { get; private set; }
		void OnResetIgnore()
		{
			ObjectCommandSendHelper.SetAutomaticRegimeForZone(Zone);
		}
		bool CanResetIgnore()
		{
            return ZoneState.StateBits.Contains(XStateBit.Ignore) && FiresecManager.CheckPermission(PermissionType.Oper_AddToIgnoreList);
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
					if (!device.DeviceState.StateClasses.Contains(XStateClass.Ignore))
					{
						ObjectCommandSendHelper.SetIgnoreRegimeForDevice(device, false);
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
				if (!device.DeviceState.StateClasses.Contains(XStateClass.Ignore))
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
					if (device.DeviceState.StateClasses.Contains(XStateClass.Ignore))
					{
						ObjectCommandSendHelper.SetAutomaticRegimeForDevice(device, false);
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
				if (device.DeviceState.StateClasses.Contains(XStateClass.Ignore))
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
			var zoneDetailsViewModel = new ZoneDetailsViewModel(Zone);
			DialogService.ShowWindow(zoneDetailsViewModel);
		}
	}
}