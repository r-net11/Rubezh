﻿using FiresecAPI.GK;
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

		public InstructionViewModel(GKDevice device, GKZone zone, GKGuardZone guardZone, GKDirection direction, GKAlarmType alarmType)
		{
			AlarmType = alarmType;
			Instruction = FindInstruction(device, zone, guardZone, direction);
			Title = Instruction != null ? Instruction.Name : "";
			HasContent = Instruction != null;
			
		}

		GKInstruction FindInstruction(GKDevice device, GKZone zone, GKGuardZone guardZone, GKDirection direction)
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

			if (guardZone != null)
			{
				foreach (var instruction in availableStateTypeInstructions)
				{
					if (instruction.ZoneUIDs.Contains(guardZone.UID))
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
				if (instruction.InstructionType == GKInstructionType.General)
				{
					return instruction;
				}
			}

			return null;
		}
	}
}