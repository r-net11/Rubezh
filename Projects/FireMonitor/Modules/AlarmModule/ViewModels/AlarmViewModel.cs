using System;
using System.Linq;
using FiresecAPI.Models;
using FiresecClient;
using Infrastructure;
using Infrastructure.Common;
using Infrastructure.Common.Windows;
using Infrastructure.Common.Windows.ViewModels;
using Infrastructure.Events;

namespace AlarmModule.ViewModels
{
	public class AlarmViewModel : BaseViewModel
	{
		public Alarm Alarm { get; set; }

		public AlarmViewModel(Alarm alarm)
		{
			Alarm = alarm;

			ConfirmCommand = new RelayCommand(OnConfirm, CanConfirm);
			ResetCommand = new RelayCommand(OnReset, CanReset);
			RemoveFromIgnoreListCommand = new RelayCommand(OnRemoveFromIgnoreList, CanRemoveFromIgnoreList);
			ShowOnPlanCommand = new RelayCommand(OnShowOnPlan, CanShowOnPlan);
			ShowDeviceCommand = new RelayCommand(OnShowDevice, CanShowDevice);
			ShowZoneCommand = new RelayCommand(OnShowZone, CanShowZone);
			ShowInstructionCommand = new RelayCommand(OnShowInstruction, CanShowInstruction);
		}

		public string Source
		{
			get
			{
				switch (Alarm.AlarmEntityType)
				{
					case AlarmEntityType.Device:
						var device = FiresecManager.Devices.FirstOrDefault(x => x.UID == Alarm.DeviceUID);
						if (device != null)
						{
							return "Устройство " + device.Driver.ShortName + " " + device.DottedAddress;
						}
						break;

					case AlarmEntityType.Zone:
                        var zone = FiresecManager.Zones.FirstOrDefault(x => x.UID == Alarm.ZoneUID);
						if (zone != null)
						{
							return "Зона " + zone.PresentationName;
						}
						break;
				}
				return "";
			}
		}

		public bool MustConfirm
		{
			get
			{
				return ((Alarm.AlarmType == AlarmType.Fire) &&
					(FiresecManager.CurrentUser.Permissions.Any(x => x == PermissionType.Oper_NoAlarmConfirm) == false));
			}
		}

		bool CanConfirm()
		{
			return !Alarm.IsConfirmed;
		}

		public RelayCommand ConfirmCommand { get; private set; }
		void OnConfirm()
		{
			Alarm.Confirm();
		}

		public bool CanReset()
		{
			return Alarm.CanReset();
		}

		public RelayCommand ResetCommand { get; private set; }
		void OnReset()
		{
			Alarm.Reset();
		}

		public bool CanRemoveFromIgnoreList()
		{
			return Alarm.CanRemoveFromIgnoreList();
		}

		public RelayCommand RemoveFromIgnoreListCommand { get; private set; }
		void OnRemoveFromIgnoreList()
		{
			if (ServiceFactory.SecurityService.Validate())
			{
				Alarm.RemoveFromIgnoreList();
			}
		}

		bool CanShowOnPlan()
		{
			if ((Alarm.DeviceUID == null) || (Alarm.DeviceUID == Guid.Empty))
				return false;

			return FiresecManager.PlansConfiguration.AllPlans.Any(x => { return x.ElementDevices.Any(y => y.DeviceUID == Alarm.DeviceUID); });
		}

		public RelayCommand ShowOnPlanCommand { get; private set; }
		void OnShowOnPlan()
		{
			ServiceFactory.Events.GetEvent<ShowDeviceOnPlanEvent>().Publish(Alarm.DeviceUID);
		}

		bool CanShowDevice()
		{
			return ((Alarm.DeviceUID != null) && (Alarm.DeviceUID != Guid.Empty));
		}

		public RelayCommand ShowDeviceCommand { get; private set; }
		void OnShowDevice()
		{
			ServiceFactory.Events.GetEvent<ShowDeviceEvent>().Publish(Alarm.DeviceUID);
		}

		bool CanShowZone()
		{
            return Alarm.ZoneUID != Guid.Empty;
		}

		public RelayCommand ShowZoneCommand { get; private set; }
		void OnShowZone()
		{
            ServiceFactory.Events.GetEvent<ShowZoneEvent>().Publish(Alarm.ZoneUID);
		}

		bool CanShowInstruction()
		{
			var instructionViewModel = new InstructionViewModel(Alarm.DeviceUID, Alarm.AlarmType);
			return instructionViewModel.HasContent;
		}

		public RelayCommand ShowInstructionCommand { get; private set; }
		void OnShowInstruction()
		{
			var instructionViewModel = new InstructionViewModel(Alarm.DeviceUID, Alarm.AlarmType);
			DialogService.ShowModalWindow(instructionViewModel);
		}
	}
}