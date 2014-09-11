using System;
using System.Collections.ObjectModel;
using System.Linq;
using FiresecAPI;
using FiresecAPI.Automation;
using FiresecAPI.GK;
using FiresecClient;
using Infrastructure;
using Infrastructure.Common;
using Infrastructure.Common.Windows;
using Infrastructure.Common.Windows.ViewModels;
using System.ComponentModel;
using FiresecAPI;

namespace AutomationModule.ViewModels
{
	public class ControlGKDeviceStepViewModel : BaseStepViewModel
	{
		ControlGKDeviceArguments ControlGkDeviceArguments { get; set; }
		public ArithmeticParameterViewModel Variable1 { get; private set; }

		public ControlGKDeviceStepViewModel(StepViewModel stepViewModel) : base(stepViewModel)
		{
			ControlGkDeviceArguments = stepViewModel.Step.ControlGKDeviceArguments;
			Variable1 = new ArithmeticParameterViewModel(ControlGkDeviceArguments.Variable1, stepViewModel.Update);
			Variable1.ObjectType = ObjectType.Device;
			Variable1.ExplicitType = ExplicitType.Object;
			Variable1.UpdateVariableScopeHandler = Update;
			Commands = new ObservableCollection<CommandType>();
			UpdateContent();
		}

		public void Update()
		{
			if (Variable1.SelectedVariableScope != VariableScope.ExplicitValue)
				Commands = ProcedureHelper.GetEnumObs<CommandType>();
			else if (Variable1.CurrentVariableItem.Device != null)
				InitializeCommands(Variable1.CurrentVariableItem.Device);
			SelectedCommand = Commands.FirstOrDefault();
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

		void InitializeCommands(XDevice device)
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
					if (device.DriverType == XDriverType.Valve)
					{
						switch (availableCommand)
						{
							case XStateBit.TurnOn_InManual:
								Commands.Add(CommandType.TurnOn_InManual2);
								break;
							case XStateBit.TurnOnNow_InManual:
								Commands.Add(CommandType.TurnOnNow_InManual2);
								break;
							case XStateBit.TurnOff_InManual:
								Commands.Add(CommandType.TurnOff_InManual2);
								break;
							case XStateBit.Stop_InManual:
								Commands.Add(CommandType.Stop_InManual);
								break;
						}
					}
					else
						Commands.Add(XStateBitToCommandType(availableCommand));
				}
				if (device.DriverType == XDriverType.JockeyPump)
					Commands.Add(CommandType.ForbidStart_InManual);
			}
			OnPropertyChanged(() => Commands);
		}

		public bool IsBiStateControl(XDevice device)
		{
			return device.Driver.IsDeviceOnShleif && !device.Driver.IsControlDevice;
		}

		public bool IsTriStateControl(XDevice device)
		{
			return device.Driver.IsControlDevice;
		}

		public bool HasReset(XDevice device)
		{
			return device.DriverType == XDriverType.AMP_1 || device.DriverType == XDriverType.RSR2_MAP4;
		}


		public override void UpdateContent()
		{
			Variable1.Update(ProcedureHelper.GetAllVariables(Procedure).FindAll(x => x.ExplicitType == ExplicitType.Object && x.ObjectType == ObjectType.Device && !x.IsList));
		}

		public override string Description
		{
			get
			{
				return "Устройство: " + Variable1.Description + "Команда: " + SelectedCommand.ToDescription();
			}
		}

		XStateBit CommandTypeToXStateBit(CommandType commandType)
		{
			switch (commandType)
			{
				case CommandType.SetRegime_Automatic:
					return XStateBit.SetRegime_Automatic;
				case CommandType.SetRegime_Automatic2:
					return XStateBit.SetRegime_Automatic;
				case CommandType.SetRegime_Manual:
					return XStateBit.SetRegime_Manual;
				case CommandType.SetRegime_Off:
					return XStateBit.SetRegime_Off;
				case CommandType.TurnOn_InManual:
					return XStateBit.TurnOn_InManual;
				case CommandType.TurnOn_InManual2:
					return XStateBit.TurnOn_InManual;
				case CommandType.TurnOnNow_InManual:
					return XStateBit.TurnOnNow_InManual;
				case CommandType.TurnOnNow_InManual2:
					return XStateBit.TurnOnNow_InManual;
				case CommandType.TurnOff_InManual:
					return XStateBit.TurnOff_InManual;
				case CommandType.TurnOff_InManual2:
					return XStateBit.TurnOff_InManual;
				case CommandType.Stop_InManual:
					return XStateBit.Stop_InManual;
				case CommandType.Reset:
					return XStateBit.Reset;
				case CommandType.ForbidStart_InManual:
					return XStateBit.ForbidStart_InManual;
				default:
					return new XStateBit();
			}
		}

		CommandType XStateBitToCommandType(XStateBit stateString)
		{
			switch (stateString)
			{
				case XStateBit.SetRegime_Automatic:
					return IsTriStateControl(Variable1.CurrentVariableItem.Device) ? CommandType.SetRegime_Automatic : CommandType.SetRegime_Automatic2;
				case XStateBit.SetRegime_Manual:
					return CommandType.SetRegime_Manual;
				case XStateBit.SetRegime_Off:
					return CommandType.SetRegime_Off;
				case XStateBit.TurnOn_InManual:
					return (Variable1.CurrentVariableItem.Device.DriverType == XDriverType.Valve) ? CommandType.TurnOn_InManual2 : CommandType.TurnOn_InManual;
				case XStateBit.TurnOnNow_InManual:
					return (Variable1.CurrentVariableItem.Device.DriverType == XDriverType.Valve) ? CommandType.TurnOnNow_InManual2 : CommandType.TurnOnNow_InManual;
				case XStateBit.TurnOff_InManual:
					return (Variable1.CurrentVariableItem.Device.DriverType == XDriverType.Valve) ? CommandType.TurnOff_InManual2 : CommandType.TurnOff_InManual;
				case XStateBit.Stop_InManual:
					return CommandType.Stop_InManual;
				case XStateBit.Reset:
					return CommandType.Reset;
				case XStateBit.ForbidStart_InManual:
					return CommandType.ForbidStart_InManual;
				default:
					return (CommandType) 111;
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

		[Description("Остановить")]
		Stop_InManual,

		[Description("Сбросить")]
		Reset,

		[Description("Запретить пуск")]
		ForbidStart_InManual,
	}
}
