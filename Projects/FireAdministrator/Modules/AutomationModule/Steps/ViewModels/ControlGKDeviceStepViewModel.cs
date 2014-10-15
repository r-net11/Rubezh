using System.Collections.ObjectModel;
using System.Linq;
using FiresecAPI;
using FiresecAPI.Automation;
using FiresecAPI.GK;
using System.ComponentModel;

namespace AutomationModule.ViewModels
{
	public class ControlGKDeviceStepViewModel : BaseStepViewModel
	{
		ControlGKDeviceArguments ControlGkDeviceArguments { get; set; }
		public ArgumentViewModel GKDeviceArgument { get; private set; }

		public ControlGKDeviceStepViewModel(StepViewModel stepViewModel) : base(stepViewModel)
		{
			ControlGkDeviceArguments = stepViewModel.Step.ControlGKDeviceArguments;
			GKDeviceArgument = new ArgumentViewModel(ControlGkDeviceArguments.GKDeviceArgument, stepViewModel.Update, null);
			GKDeviceArgument.UpdateVariableScopeHandler = Update;
			GKDeviceArgument.ExplicitValue.UpdateObjectHandler += Update;
			Commands = new ObservableCollection<CommandType>();
		}

		public void Update()
		{
			if (GKDeviceArgument.SelectedVariableScope != VariableScope.ExplicitValue)
			{
				Commands = ProcedureHelper.GetEnumObs<CommandType>();
				Commands.Remove(CommandType.Unknown);
			}
			else if (GKDeviceArgument.ExplicitValue.Device != null)
				InitializeCommands(GKDeviceArgument.ExplicitValue.Device);
			SelectedCommand = Commands.FirstOrDefault(x => x == XStateBitToCommandType(ControlGkDeviceArguments.Command));
			OnPropertyChanged(() => Commands);
		}

		public ObservableCollection<CommandType> Commands { get; private set; }

		CommandType _selectedCommand;
		public CommandType SelectedCommand
		{
			get { return _selectedCommand; }
			set
			{
				_selectedCommand = value;
				ControlGkDeviceArguments.Command = CommandTypeToXStateBit(value);
				OnPropertyChanged(() => SelectedCommand);
			}
		}

		void InitializeCommands(GKDevice device)
		{
			if (IsBiStateControl(device))
			{
				Commands = new ObservableCollection<CommandType> { CommandType.SetRegime_Automatic2, CommandType.SetRegime_Off };
				if (HasReset(device))
					Commands.Add(CommandType.Reset);
			}
			if (IsTriStateControl(device))
			{
				Commands = new ObservableCollection<CommandType> { CommandType.SetRegime_Automatic, CommandType.SetRegime_Manual, CommandType.SetRegime_Off };
				foreach (var availableCommand in device.Driver.AvailableCommandBits)
				{
					if (device.DriverType == GKDriverType.Valve)
					{
						switch (availableCommand)
						{
							case GKStateBit.TurnOn_InManual:
								Commands.Add(CommandType.TurnOn_InManual2);
								break;
							case GKStateBit.TurnOnNow_InManual:
								Commands.Add(CommandType.TurnOnNow_InManual2);
								break;
							case GKStateBit.TurnOff_InManual:
								Commands.Add(CommandType.TurnOff_InManual2);
								break;
							case GKStateBit.Stop_InManual:
								Commands.Add(CommandType.Stop_InManual);
								break;
						}
					}
					else
						Commands.Add(XStateBitToCommandType(availableCommand));
				}
				if (device.DriverType == GKDriverType.JockeyPump)
					Commands.Add(CommandType.ForbidStart_InManual);
			}
			OnPropertyChanged(() => Commands);
		}

		public bool IsBiStateControl(GKDevice device)
		{
			return device.Driver.IsDeviceOnShleif && !device.Driver.IsControlDevice;
		}

		public bool IsTriStateControl(GKDevice device)
		{
			return device.Driver.IsControlDevice;
		}

		public bool HasReset(GKDevice device)
		{
			return device.DriverType == GKDriverType.AMP_1 || device.DriverType == GKDriverType.RSR2_MAP4;
		}


		public override void UpdateContent()
		{
			GKDeviceArgument.Update(Procedure, ExplicitType.Object, objectType:ObjectType.Device, isList:false);
		}

		public override string Description
		{
			get
			{
				return "Устройство: " + GKDeviceArgument.Description + " Команда: " + SelectedCommand.ToDescription();
			}
		}

		GKStateBit CommandTypeToXStateBit(CommandType commandType)
		{
			switch (commandType)
			{
				case CommandType.SetRegime_Automatic:
					return GKStateBit.SetRegime_Automatic;
				case CommandType.SetRegime_Automatic2:
					return GKStateBit.SetRegime_Automatic;
				case CommandType.SetRegime_Manual:
					return GKStateBit.SetRegime_Manual;
				case CommandType.SetRegime_Off:
					return GKStateBit.SetRegime_Off;
				case CommandType.TurnOn_InManual:
					return GKStateBit.TurnOn_InManual;
				case CommandType.TurnOn_InManual2:
					return GKStateBit.TurnOn_InManual;
				case CommandType.TurnOnNow_InManual:
					return GKStateBit.TurnOnNow_InManual;
				case CommandType.TurnOnNow_InManual2:
					return GKStateBit.TurnOnNow_InManual;
				case CommandType.TurnOff_InManual:
					return GKStateBit.TurnOff_InManual;
				case CommandType.TurnOff_InManual2:
					return GKStateBit.TurnOff_InManual;
				case CommandType.Stop_InManual:
					return GKStateBit.Stop_InManual;
				case CommandType.Reset:
					return GKStateBit.Reset;
				case CommandType.ForbidStart_InManual:
					return GKStateBit.ForbidStart_InManual;
				case CommandType.TurnOffNow_InManual:
					return GKStateBit.TurnOffNow_InManual;
				default:
					return new GKStateBit();
			}
		}

		CommandType XStateBitToCommandType(GKStateBit stateString)
		{
			if (GKDeviceArgument.ExplicitValue.Device != null)
			{
				switch (stateString)
				{
					case GKStateBit.SetRegime_Automatic:
						return IsTriStateControl(GKDeviceArgument.ExplicitValue.Device) ? CommandType.SetRegime_Automatic : CommandType.SetRegime_Automatic2;
					case GKStateBit.SetRegime_Manual:
						return CommandType.SetRegime_Manual;
					case GKStateBit.SetRegime_Off:
						return CommandType.SetRegime_Off;
					case GKStateBit.TurnOn_InManual:
						return (GKDeviceArgument.ExplicitValue.Device.DriverType == GKDriverType.Valve) ? CommandType.TurnOn_InManual2 : CommandType.TurnOn_InManual;
					case GKStateBit.TurnOnNow_InManual:
						return (GKDeviceArgument.ExplicitValue.Device.DriverType == GKDriverType.Valve) ? CommandType.TurnOnNow_InManual2 : CommandType.TurnOnNow_InManual;
					case GKStateBit.TurnOff_InManual:
						return (GKDeviceArgument.ExplicitValue.Device.DriverType == GKDriverType.Valve) ? CommandType.TurnOff_InManual2 : CommandType.TurnOff_InManual;
					case GKStateBit.Stop_InManual:
						return CommandType.Stop_InManual;
					case GKStateBit.Reset:
						return CommandType.Reset;
					case GKStateBit.ForbidStart_InManual:
						return CommandType.ForbidStart_InManual;
					case GKStateBit.TurnOffNow_InManual:
						return CommandType.TurnOffNow_InManual;
					default:
						return CommandType.Unknown;
				}
			}
			switch (stateString)
			{
				case GKStateBit.SetRegime_Automatic:
					return CommandType.SetRegime_Automatic;
				case GKStateBit.SetRegime_Manual:
					return CommandType.SetRegime_Manual;
				case GKStateBit.SetRegime_Off:
					return CommandType.SetRegime_Off;
				case GKStateBit.TurnOn_InManual:
					return CommandType.TurnOn_InManual;
				case GKStateBit.TurnOnNow_InManual:
					return CommandType.TurnOnNow_InManual;
				case GKStateBit.TurnOff_InManual:
					return CommandType.TurnOff_InManual;
				case GKStateBit.Stop_InManual:
					return CommandType.Stop_InManual;
				case GKStateBit.Reset:
					return CommandType.Reset;
				case GKStateBit.ForbidStart_InManual:
					return CommandType.ForbidStart_InManual;
				case GKStateBit.TurnOffNow_InManual:
					return CommandType.TurnOffNow_InManual;
				default:
					return CommandType.Unknown;
			}
		}
	}

	public enum CommandType
	{
		[Description("Автоматика")]
		SetRegime_Automatic,

		[Description("Снять отключение")]
		SetRegime_Automatic2,

		[Description("Ручное")]
		SetRegime_Manual,

		[Description("Отключение")]
		SetRegime_Off,

		[Description("Включить")]
		TurnOn_InManual,

		[Description("Открыть")]
		TurnOn_InManual2,

		[Description("Включить немедленно")]
		TurnOnNow_InManual,

		[Description("Открыть немедленно")]
		TurnOnNow_InManual2,

		[Description("Выключить")]
		TurnOff_InManual,

		[Description("Закрыть")]
		TurnOff_InManual2,

		[Description("Выключить немедленно")]
		TurnOffNow_InManual,

		[Description("Остановить")]
		Stop_InManual,

		[Description("Сбросить")]
		Reset,

		[Description("Запретить пуск")]
		ForbidStart_InManual,

		[Description("Неизвестная команда")]
		Unknown
	}
}
