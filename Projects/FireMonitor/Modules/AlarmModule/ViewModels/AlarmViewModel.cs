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
			ResetCommand = new RelayCommand(OnReset);
			RemoveFromIgnoreListCommand = new RelayCommand(OnRemoveFromIgnoreList);

			ShowOnPlanCommand = new RelayCommand(OnShowOnPlan);
			ShowDeviceCommand = new RelayCommand(OnShowDevice);
			ShowZoneCommand = new RelayCommand(OnShowZone, CanShowZone);
			ShowInstructionCommand = new RelayCommand(OnShowInstruction, CanShowInstruction);
		}

		public string Time
		{
			get { return Alarm.Time.ToString(); }
		}

		public string Source
		{
			get
			{
				switch (Alarm.AlarmEntityType)
				{
					case AlarmEntityType.Device:
						var device = FiresecManager.DeviceConfiguration.Devices.FirstOrDefault(x => x.UID == Alarm.DeviceUID);
						return "Устройство " + device.Driver.ShortName + " " + device.DottedAddress;

					case AlarmEntityType.Zone:
						var zone = FiresecManager.DeviceConfiguration.Zones.FirstOrDefault(x => x.No == Alarm.ZoneNo);
						return "Зона " + zone.PresentationName;

					default:
						return "";
				}
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

		public bool CanReset
		{
			get { return Alarm.CanReset(); }
		}

		public RelayCommand ResetCommand { get; private set; }
		void OnReset()
		{
			Alarm.Reset();
		}

		public bool CanRemoveFromIgnoreList
		{
			get { return Alarm.CanRemoveFromIgnoreList(); }
		}

		public RelayCommand RemoveFromIgnoreListCommand { get; private set; }
		void OnRemoveFromIgnoreList()
		{
			if (ServiceFactory.SecurityService.Validate())
			{
				Alarm.RemoveFromIgnoreList();
			}
		}

		public RelayCommand ShowOnPlanCommand { get; private set; }
		void OnShowOnPlan()
		{
			ServiceFactory.Events.GetEvent<ShowDeviceOnPlanEvent>().Publish(Alarm.DeviceUID);
		}

		public RelayCommand ShowDeviceCommand { get; private set; }
		void OnShowDevice()
		{
			ServiceFactory.Events.GetEvent<ShowDeviceEvent>().Publish(Alarm.DeviceUID);
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

		bool CanShowZone()
		{
			return Alarm.ZoneNo.HasValue;
		}

		public RelayCommand ShowZoneCommand { get; private set; }
		void OnShowZone()
		{
			ServiceFactory.Events.GetEvent<ShowZoneEvent>().Publish(Alarm.ZoneNo.Value);
		}
	}
}