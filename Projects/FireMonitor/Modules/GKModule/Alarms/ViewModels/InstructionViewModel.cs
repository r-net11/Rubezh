using FiresecAPI.GK;
using FiresecClient;
using Infrastructure.Common.Windows.ViewModels;

namespace GKModule.ViewModels
{
	public class InstructionViewModel : DialogViewModel
	{
		public InstructionViewModel(XDevice device, XZone zone, XDirection direction, XAlarmType alarmType)
		{
			
			AlarmType = alarmType;

			Instruction = FindInstruction(device, zone, direction);
			Title = Instruction != null ? Instruction.Name : "";
			HasContent = Instruction != null;
			
		}

		public bool HasContent { get; private set; }
		public XAlarmType AlarmType { get; private set; }
		public XStateBit StateType { get; private set; }
		public XInstruction Instruction { get; private set; }

		XInstruction FindInstruction(XDevice device, XZone zone, XDirection direction)
		{
			var availableStateTypeInstructions = XManager.DeviceConfiguration.Instructions.FindAll(x => x.AlarmType == AlarmType);

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

			if (zone != null)
			{
				foreach (var instruction in availableStateTypeInstructions)
				{
					if (instruction.ZoneUIDs.Contains(zone.UID))
					{
						return instruction;
					}
				}
			}

			if (direction != null)
			{
				foreach (var instruction in availableStateTypeInstructions)
				{
					if (instruction.Directions == null)
						break;
					if (instruction.Directions.Contains(direction.UID))
					{
						return instruction;
					}
				}
			}

			foreach (var instruction in availableStateTypeInstructions)
			{
				if (instruction.InstructionType == XInstructionType.General)
				{
					return instruction;
				}
			}

			return null;
		}
	}
}