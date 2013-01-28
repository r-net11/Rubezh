using Infrastructure;
using Infrastructure.Common;
using Infrastructure.Common.Windows.ViewModels;
using Infrastructure.Events;
using XFiresecAPI;
using Infrastructure.Common.Windows;

namespace GKModule.ViewModels
{
	public class AlarmViewModel : BaseViewModel
	{
		public Alarm Alarm { get; private set; }

		public AlarmViewModel(Alarm alarm)
		{
			ShowObjectCommand = new RelayCommand(OnShowObject);
			ShowOnPlanCommand = new RelayCommand(OnShowOnPlan, CanShowOnPlan);
			ResetCommand = new RelayCommand(OnReset, CanReset);
			ResetIgnoreCommand = new RelayCommand(OnResetIgnore, CanResetIgnore);
			ShowPropertiesCommand = new RelayCommand(OnShowProperties, CanShowProperties);
			ShowInstructionCommand = new RelayCommand(OnShowInstruction, CanShowInstruction);
			Alarm = alarm;
		}

		public RelayCommand ShowObjectCommand { get; private set; }
		void OnShowObject()
		{
			if (Alarm.Device != null)
			{
				ServiceFactory.Events.GetEvent<ShowXDeviceEvent>().Publish(Alarm.Device.UID);
			}
			if (Alarm.Zone != null)
			{
				ServiceFactory.Events.GetEvent<ShowXZoneEvent>().Publish(Alarm.Zone.UID);
			}
			if (Alarm.Direction != null)
			{
				ServiceFactory.Events.GetEvent<ShowXDirectionEvent>().Publish(Alarm.Direction.UID);
			}
		}

		public RelayCommand ShowOnPlanCommand { get; private set; }
		void OnShowOnPlan()
		{
			if (Alarm.Device != null)
			{
				ShowOnPlanHelper.ShowDevice(Alarm.Device);
			}
			if (Alarm.Zone != null)
			{
				ShowOnPlanHelper.ShowZone(Alarm.Zone);
			}
		}
		bool CanShowOnPlan()
		{
			if (Alarm.Device != null)
			{
				return ShowOnPlanHelper.CanShowDevice(Alarm.Device);
			}
			if (Alarm.Zone != null)
			{
				return ShowOnPlanHelper.CanShowZone(Alarm.Zone);
			}
			return false;
		}

		public RelayCommand ResetCommand { get; private set; }
		void OnReset()
		{
			switch (Alarm.AlarmType)
			{
				case XAlarmType.Fire1:
					ObjectCommandSendHelper.SendControlCommand(Alarm.Zone, 0x02);
					break;

				case XAlarmType.Fire2:
					ObjectCommandSendHelper.SendControlCommand(Alarm.Zone, 0x03);
					break;
			}
		}
		bool CanReset()
		{
			return Alarm.Zone != null && (Alarm.AlarmType == XAlarmType.Fire1 || Alarm.AlarmType == XAlarmType.Fire2);
		}

		public RelayCommand ResetIgnoreCommand { get; private set; }
		void OnResetIgnore()
		{
			if (Alarm.Device != null)
			{
				if (!Alarm.Device.Driver.IsDeviceOnShleif)
					return;

				if (Alarm.Device.DeviceState.States.Contains(XStateType.Ignore))
				{
					ObjectCommandSendHelper.SendControlCommand(Alarm.Device, 0x06);
				}
			}

			if (Alarm.Zone != null)
			{
				if (Alarm.Zone.ZoneState.States.Contains(XStateType.Ignore))
				{
					ObjectCommandSendHelper.SendControlCommand(Alarm.Zone, 0x06);
				}
			}

			if (Alarm.Direction != null)
			{
				if (Alarm.Direction.DirectionState.States.Contains(XStateType.Ignore))
				{
					ObjectCommandSendHelper.SendControlCommand(Alarm.Direction, 0x06);
				}
			}
		}
		bool CanResetIgnore()
		{
			if (Alarm.Device != null)
			{
				if (!Alarm.Device.Driver.IsDeviceOnShleif)
					return false;

				if (Alarm.Device.DeviceState.States.Contains(XStateType.Ignore))
					return true;
			}

			if (Alarm.Zone != null)
			{
				if (Alarm.Zone.ZoneState.States.Contains(XStateType.Ignore))
					return true;
			}

			if (Alarm.Direction != null)
			{
				if (Alarm.Direction.DirectionState.States.Contains(XStateType.Ignore))
					return true;
			}
			return false;
		}

		public RelayCommand ShowPropertiesCommand { get; private set; }
		void OnShowProperties()
		{
			if (Alarm.Device != null)
			{
				ServiceFactory.Events.GetEvent<ShowXDeviceDetailsEvent>().Publish(Alarm.Device.UID);
			}
			if (Alarm.Direction != null)
			{
				DialogService.ShowWindow(new DirectionDetailsViewModel(Alarm.Direction));
			}
		}
		bool CanShowProperties()
		{
			return (Alarm.Device != null) || (Alarm.Direction != null);
		}

		public RelayCommand ShowInstructionCommand { get; private set; }
		void OnShowInstruction()
		{
			var instructionViewModel = new InstructionViewModel(Alarm.Device, Alarm.Zone, Alarm.AlarmType);
			DialogService.ShowModalWindow(instructionViewModel);
		}
		bool CanShowInstruction()
		{
			var instructionViewModel = new InstructionViewModel(Alarm.Device, Alarm.Zone, Alarm.AlarmType);
			return instructionViewModel.HasContent;
		}
		public XInstruction Instruction
		{
			get
			{
				var instructionViewModel = new InstructionViewModel(Alarm.Device, Alarm.Zone, Alarm.AlarmType);
				return instructionViewModel.Instruction;
			}
		}
	}
}