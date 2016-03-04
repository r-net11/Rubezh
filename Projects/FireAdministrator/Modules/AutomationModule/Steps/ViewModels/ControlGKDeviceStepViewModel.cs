using Infrastructure.Automation;
using RubezhAPI;
using RubezhAPI.Automation;
using RubezhAPI.GK;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;

namespace AutomationModule.ViewModels
{
	public class ControlGKDeviceStepViewModel : BaseStepViewModel
	{
		ControlGKDeviceArguments ControlGkDeviceArguments { get; set; }
		public ArgumentViewModel GKDeviceArgument { get; private set; }

		public ControlGKDeviceStepViewModel(StepViewModel stepViewModel)
			: base(stepViewModel)
		{
			ControlGkDeviceArguments = stepViewModel.Step.ControlGKDeviceArguments;
			GKDeviceArgument = new ArgumentViewModel(ControlGkDeviceArguments.GKDeviceArgument, stepViewModel.Update, null);
			GKDeviceArgument.UpdateVariableScopeHandler = Update;
			GKDeviceArgument.ExplicitValue.UpdateObjectHandler += Update;
			Commands = new ObservableCollection<CommandType>();
			Update();
		}

		public void Update()
		{
			if (GKDeviceArgument.SelectedVariableScope != VariableScope.ExplicitValue)
			{
				Commands = AutomationHelper.GetEnumObs<CommandType>();
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
				Commands = new ObservableCollection<CommandType> { CommandType.SetRegime_Automatic, CommandType.SetRegime_Off };
				if (HasReset(device))
					Commands.Add(CommandType.Reset);
			}
			if (IsTriStateControl(device))
			{
				Commands = new ObservableCollection<CommandType> { CommandType.SetRegime_Automatic, CommandType.SetRegime_Manual, CommandType.SetRegime_Off };
				foreach (var availableCommand in device.Driver.AvailableCommandBits)
				{
					Commands.Add(XStateBitToCommandType(availableCommand));
				}
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
			return device.DriverType == GKDriverType.RSR2_MAP4;
		}


		public override void UpdateContent()
		{
			GKDeviceArgument.Update(Procedure, ExplicitType.Object, objectType: ObjectType.Device, isList: false);
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
				case CommandType.SetRegime_Manual:
					return GKStateBit.SetRegime_Manual;
				case CommandType.SetRegime_Off:
					return GKStateBit.SetRegime_Off;
				case CommandType.TurnOn_InManual:
					return GKStateBit.TurnOn_InManual;
				case CommandType.TurnOnNow_InManual:
					return GKStateBit.TurnOnNow_InManual;
				case CommandType.TurnOff_InManual:
					return GKStateBit.TurnOff_InManual;
				case CommandType.Stop_InManual:
					return GKStateBit.Stop_InManual;
				case CommandType.Reset:
					return GKStateBit.Reset;
				case CommandType.ForbidStart_InManual:
					return GKStateBit.ForbidStart_InManual;
				case CommandType.TurnOffNow_InManual:
					return GKStateBit.TurnOffNow_InManual;
				case CommandType.TurnOn_InAutomatic:
					return GKStateBit.TurnOn_InAutomatic;
				case CommandType.TurnOnNow_InAutomatic:
					return GKStateBit.TurnOnNow_InAutomatic;
				case CommandType.TurnOff_InAutomatic:
					return GKStateBit.TurnOff_InAutomatic;
				case CommandType.TurnOffNow_InAutomatic:
					return GKStateBit.TurnOffNow_InAutomatic;
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
					case GKStateBit.TurnOn_InAutomatic:
						return CommandType.TurnOn_InAutomatic;
					case GKStateBit.TurnOnNow_InAutomatic:
						return CommandType.TurnOnNow_InAutomatic;
					case GKStateBit.TurnOff_InAutomatic:
						return CommandType.TurnOff_InAutomatic;
					case GKStateBit.TurnOffNow_InAutomatic:
						return CommandType.TurnOffNow_InAutomatic;
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

		[Description("Ручное")]
		SetRegime_Manual,

		[Description("Отключение")]
		SetRegime_Off,

		[Description("Включить")]
		TurnOn_InManual,

		[Description("Включить немедленно")]
		TurnOnNow_InManual,

		[Description("Выключить")]
		TurnOff_InManual,

		[Description("Выключить немедленно")]
		TurnOffNow_InManual,

		[Description("Включить в автоматическом режиме")]
		TurnOn_InAutomatic,

		[Description("Включить немедленно в автоматическом режиме")]
		TurnOnNow_InAutomatic,

		[Description("Выключить в автоматическом режиме")]
		TurnOff_InAutomatic,

		[Description("Выключить немедленно в автоматическом режиме")]
		TurnOffNow_InAutomatic,

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