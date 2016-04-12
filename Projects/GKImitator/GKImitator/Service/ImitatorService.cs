using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Windows.Documents;
using GKImitator.ViewModels;
using RubezhAPI;
using RubezhAPI.GK;

namespace GKImitator
{
	[ServiceBehavior(MaxItemsInObjectGraph = Int32.MaxValue, UseSynchronizationContext = false,
	InstanceContextMode = InstanceContextMode.Single, ConcurrencyMode = ConcurrencyMode.Multiple)]
	public class ImitatorService : IImitatorService
	{
		public OperationResult<string> TestImitator()
		{
			return new OperationResult<string>("Hello from Imitator");
		}

		public OperationResult<bool> ConrtolGKBase(Guid uid, GKStateBit command)
		{
			var descriptor = MainViewModel.Current.Descriptors.FirstOrDefault(x => x.GKBase.UID == uid);
			if (descriptor == null)
				return OperationResult<bool>.FromError("Не найден элемент " + uid + " в конфигурации");
			if (!ValidateCommand(descriptor, command))
				return OperationResult<bool>.FromError("Команда " + command.ToDescription() + " для типа " + descriptor.GKBase.GetType() + " запрещена");
			switch (command)
			{
				case GKStateBit.Fire1:
					descriptor.SetFire1Command.Execute();
					break;
				case GKStateBit.Fire2:
					descriptor.SetFire2Command.Execute();
					break;
				case GKStateBit.Reset:
					descriptor.ResetFireCommand.Execute();
					break;
				case GKStateBit.TurnOn_InAutomatic:
					descriptor.TurnOnCommand.Execute();
					break;
				case GKStateBit.TurnOnNow_InAutomatic:
					descriptor.TurnOnNowCommand.Execute();
					break;
				case GKStateBit.TurnOff_InAutomatic:
					descriptor.TurnOffCommand.Execute();
					break;
				case GKStateBit.TurnOffNow_InAutomatic:
					descriptor.TurnOffNowCommand.Execute();
					break;  
				default:
					return OperationResult<bool>.FromError("Такая команда ещё не реализована");
			}
			return new OperationResult<bool>(true);
		}

		bool ValidateCommand(DescriptorViewModel descriptor, GKStateBit command)
		{
			var availableCommands = new List<GKStateBit>();
			if (descriptor.HasAutomaticRegime)
				availableCommands.Add(GKStateBit.SetRegime_Automatic);
			if (descriptor.HasManualRegime)
				availableCommands.Add(GKStateBit.SetRegime_Manual);
			if (descriptor.HasIgnoreRegime)
				availableCommands.Add(GKStateBit.Ignore);
			if (descriptor.HasReset || descriptor.HasResetFire)
				availableCommands.Add(GKStateBit.Reset);
			if (descriptor.HasSetFireHandDetector || descriptor.HasFire12)
				availableCommands.Add(GKStateBit.Fire2);
			if (descriptor.HasSetFireSmoke || descriptor.HasSetFireTemperature || descriptor.HasSetFireTemperatureGradient || descriptor.HasFire12)
				availableCommands.Add(GKStateBit.Fire1);
			if (descriptor.HasTest)
				availableCommands.Add(GKStateBit.Test);
			if (descriptor.HasTurnOn)
			{ 
				availableCommands.Add(GKStateBit.TurnOn_InManual);
				availableCommands.Add(GKStateBit.TurnOn_InAutomatic);
			}
			if (descriptor.HasTurnOnNow)
			{
				availableCommands.Add(GKStateBit.TurnOnNow_InManual);
				availableCommands.Add(GKStateBit.TurnOnNow_InAutomatic);
			}
			if (descriptor.HasTurnOff)
			{
				availableCommands.Add(GKStateBit.TurnOff_InManual);
				availableCommands.Add(GKStateBit.TurnOff_InAutomatic);
			}
			if (descriptor.HasTurnOffNow)
			{
				availableCommands.Add(GKStateBit.TurnOffNow_InManual);
				availableCommands.Add(GKStateBit.TurnOffNow_InAutomatic);
			}
			if (descriptor.HasAlarm)
				availableCommands.Add(GKStateBit.Fire1);

			return availableCommands.Contains(command);
		}
	}
}
