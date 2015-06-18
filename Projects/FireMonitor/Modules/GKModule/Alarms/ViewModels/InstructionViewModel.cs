using FiresecAPI.GK;
using FiresecClient;
using Infrastructure.Common.Windows.ViewModels;

namespace GKModule.ViewModels
{
	public class InstructionViewModel : DialogViewModel
	{
		public bool HasContent { get; private set; }
		public GKAlarmType AlarmType { get; private set; }
		public GKStateBit StateType { get; private set; }
		public GKInstruction Instruction { get; private set; }

		public InstructionViewModel(GKDevice device, GKAlarmType alarmType)
		{
			AlarmType = alarmType;
			Instruction = FindInstruction(device);
			Title = Instruction != null ? Instruction.Name : "";
			HasContent = Instruction != null;
		}

		GKInstruction FindInstruction(GKDevice device)
		{
			var availableStateTypeInstructions = GKManager.DeviceConfiguration.Instructions.FindAll(x => x.AlarmType == AlarmType);

			if (device != null)
			{
				foreach (var instruction in availableStateTypeInstructions)
				{
					if (instruction.Devices.Contains(device.UID))
					{
						return instruction;
					}
				}
			}

			foreach (var instruction in availableStateTypeInstructions)
			{
				if (instruction.InstructionType == GKInstructionType.General)
				{
					return instruction;
				}
			}

			return null;
		}
	}
}