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
			ShowObjectOrPlanCommand = new RelayCommand(OnShowObjectOrPlan);
			ShowObjectCommand = new RelayCommand(OnShowObject);
			ShowOnPlanCommand = new RelayCommand(OnShowOnPlan, CanShowOnPlan);
			ResetCommand = new RelayCommand(OnReset, CanReset);
			ResetIgnoreCommand = new RelayCommand(OnResetIgnore, CanResetIgnore);
			TurnOnAutomaticCommand = new RelayCommand(OnTurnOnAutomatic, CanTurnOnAutomatic);
			TurnOffCommand = new RelayCommand(OnTurnOff, CanTurnOff);
			ShowPropertiesCommand = new RelayCommand(OnShowProperties, CanShowProperties);
			ShowInstructionCommand = new RelayCommand(OnShowInstruction, CanShowInstruction);
			Alarm = alarm;
		}

		public string ObjectName
		{
			get
			{
				if (Alarm.Device != null)
                    return "Устройство " + Alarm.Device.PresentationDriverAndAddress;
				if (Alarm.Zone != null)
					return "Зона " + Alarm.Zone.PresentationName;
				if (Alarm.Direction != null)
					return "Направление " + Alarm.Direction.PresentationName;
				return null;
			}
		}

		public string ImageSource
		{
			get
			{
				if (Alarm.Device != null)
					return Alarm.Device.Driver.ImageSource;
				if (Alarm.Zone != null)
					return "/Controls;component/Images/BZones.png";
				if (Alarm.Direction != null)
					return "/Controls;component/Images/BDirection.png";
				return null;
			}
		}

		public RelayCommand ShowObjectOrPlanCommand { get; private set; }
		void OnShowObjectOrPlan()
		{
			if (CanShowOnPlan())
				OnShowOnPlan();
			else
				OnShowObject();
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
			if (Alarm.Direction != null)
			{
				ShowOnPlanHelper.ShowDirection(Alarm.Direction);
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
			if (Alarm.Direction != null)
			{
				return ShowOnPlanHelper.CanShowDirection(Alarm.Direction);
			}
			return false;
		}

		public RelayCommand ResetCommand { get; private set; }
		void OnReset()
		{
			if (Alarm.Zone != null)
			{
				switch (Alarm.AlarmType)
				{
					case XAlarmType.Fire1:
						ObjectCommandSendHelper.ResetFire1(Alarm.Zone);
						break;

					case XAlarmType.Fire2:
						ObjectCommandSendHelper.ResetFire2(Alarm.Zone);
						break;
				}
			}
			if (Alarm.Device != null)
			{
				ObjectCommandSendHelper.ResetDevice(Alarm.Device);
			}
		}
		bool CanReset()
		{
			if (Alarm.Zone != null)
				return (Alarm.AlarmType == XAlarmType.Fire1 || Alarm.AlarmType == XAlarmType.Fire2);
			if (Alarm.Device != null)
				return Alarm.Device.DeviceState.StateBits.Contains(XStateBit.Fire2) || Alarm.Device.DeviceState.StateBits.Contains(XStateBit.Fire1);
			return false;
		}
		public bool CanResetCommand
		{
			get { return CanReset(); }
		}

		public RelayCommand ResetIgnoreCommand { get; private set; }
		void OnResetIgnore()
		{
			if (Alarm.Device != null)
			{
				if (Alarm.Device.DeviceState.StateBits.Contains(XStateBit.Ignore))
				{
					ObjectCommandSendHelper.SetAutomaticRegimeForDevice(Alarm.Device);
				}
			}

			if (Alarm.Zone != null)
			{
				if (Alarm.Zone.ZoneState.StateBits.Contains(XStateBit.Ignore))
				{
					ObjectCommandSendHelper.SetAutomaticRegimeForZone(Alarm.Zone);
				}
			}

			if (Alarm.Direction != null)
			{
				if (Alarm.Direction.DirectionState.StateBits.Contains(XStateBit.Ignore))
				{
					ObjectCommandSendHelper.SetAutomaticRegimeForDirection(Alarm.Direction);
				}
			}
		}
		bool CanResetIgnore()
		{
			if (Alarm.AlarmType != XAlarmType.Ignore)
				return false;

			if (Alarm.Device != null)
			{
				if (Alarm.Device.DeviceState.StateBits.Contains(XStateBit.Ignore))
					return true;
			}

			if (Alarm.Zone != null)
			{
				if (Alarm.Zone.ZoneState.StateBits.Contains(XStateBit.Ignore))
					return true;
			}

			if (Alarm.Direction != null)
			{
				if (Alarm.Direction.DirectionState.StateBits.Contains(XStateBit.Ignore))
					return true;
			}
			return false;
		}
		public bool CanResetIgnoreCommand
		{
			get { return CanResetIgnore(); }
		}

		public RelayCommand TurnOnAutomaticCommand { get; private set; }
		void OnTurnOnAutomatic()
		{
			if (Alarm.Device != null)
			{
				if (!Alarm.Device.DeviceState.StateBits.Contains(XStateBit.Norm))
				{
					ObjectCommandSendHelper.SetAutomaticRegimeForDevice(Alarm.Device);
				}
			}
			if (Alarm.Direction != null)
			{
				if (!Alarm.Direction.DirectionState.StateBits.Contains(XStateBit.Norm))
				{
					ObjectCommandSendHelper.SetAutomaticRegimeForDirection(Alarm.Direction);
				}
			}
		}
		bool CanTurnOnAutomatic()
		{
			if (Alarm.AlarmType != XAlarmType.AutoOff)
				return false;

			if (Alarm.Device != null)
			{
				if (!Alarm.Device.Driver.IsControlDevice)
					return false;

				if (!Alarm.Device.DeviceState.StateBits.Contains(XStateBit.Norm))
					return true;
			}
			if (Alarm.Direction != null)
			{
				if (!Alarm.Direction.DirectionState.StateBits.Contains(XStateBit.Norm))
					return true;
			}
			return false;
		}
		public bool CanTurnOnAutomaticCommand
		{
			get { return CanTurnOnAutomatic(); }
		}

		public RelayCommand TurnOffCommand { get; private set; }
		void OnTurnOff()
		{
			if (Alarm.Device != null)
			{
				if (Alarm.Device.DeviceState.StateBits.Contains(XStateBit.On) || Alarm.Device.DeviceState.StateBits.Contains(XStateBit.TurningOn))
				{
					//var code = 0x80;
					//if (Alarm.Device.DeviceState.States.Contains(XStateType.Norm))
					//    code += (int)XStateType.TurnOff_InAutomatic;
					//else
					//    code += (int)XStateType.TurnOff_InManual;
					//ObjectCommandSendHelper.SendControlCommand(Alarm.Device, (byte)code);
					//JournaActionlHelper.Add("Команда оператора", "Выключение", XStateClass.Info, Alarm.Device);

					ObjectCommandSendHelper.TurnOffDeviceAnyway(Alarm.Device);
				}
			}
			if (Alarm.Direction != null)
			{
				if (Alarm.Direction.DirectionState.StateBits.Contains(XStateBit.On) || Alarm.Direction.DirectionState.StateBits.Contains(XStateBit.TurningOn))
				{
					//var code = 0x80;
					//if (Alarm.Direction.DirectionState.States.Contains(XStateType.Norm))
					//    code += (int)XStateType.TurnOff_InAutomatic;
					//else
					//    code += (int)XStateType.TurnOff_InManual;
					//ObjectCommandSendHelper.SendControlCommand(Alarm.Direction, (byte)code);
					//JournaActionlHelper.Add("Команда оператора", "Выключение", XStateClass.Info, Alarm.Direction);

					ObjectCommandSendHelper.TurnOffDirectionAnyway(Alarm.Direction);
				}
			}
		}
		bool CanTurnOff()
		{
			if (Alarm.Device != null)
			{
				if (Alarm.AlarmType != XAlarmType.Turning)
					return false;
				if (!Alarm.Device.Driver.IsControlDevice)
					return false;

				return Alarm.Device.DeviceState.StateBits.Contains(XStateBit.On) || Alarm.Device.DeviceState.StateBits.Contains(XStateBit.TurningOn);
			}
			if (Alarm.Direction != null)
			{
				if (Alarm.AlarmType != XAlarmType.NPTOn)
					return false;

				return Alarm.Direction.DirectionState.StateBits.Contains(XStateBit.On) || Alarm.Direction.DirectionState.StateBits.Contains(XStateBit.TurningOn);
			}
			return false;
		}
		public bool CanTurnOffCommand
		{
			get { return CanTurnOff(); }
		}

		public RelayCommand ShowPropertiesCommand { get; private set; }
		void OnShowProperties()
		{
			if (Alarm.Device != null)
			{
				ServiceFactory.Events.GetEvent<ShowXDeviceDetailsEvent>().Publish(Alarm.Device.UID);
			}
			if (Alarm.Zone != null)
			{
				DialogService.ShowWindow(new ZoneDetailsViewModel(Alarm.Zone));
			}
			if (Alarm.Direction != null)
			{
				DialogService.ShowWindow(new DirectionDetailsViewModel(Alarm.Direction));
			}
		}
		bool CanShowProperties()
		{
			return (Alarm.Device != null) || (Alarm.Direction != null) || (Alarm.Zone != null);
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