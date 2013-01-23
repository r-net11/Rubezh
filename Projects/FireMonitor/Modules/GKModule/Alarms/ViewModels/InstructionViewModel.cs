using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Infrastructure.Common.Windows.ViewModels;
using XFiresecAPI;
using FiresecClient;

namespace GKModule.ViewModels
{
	public class InstructionViewModel : DialogViewModel
	{
		public InstructionViewModel(XDevice device, XZone zone, XAlarmType alarmType)
		{
			Title = "Инструкция ";
			AlarmType = alarmType;

			Instruction = FindInstruction(device, zone);
			HasContent = Instruction != null;
			if (Instruction != null)
			{
				Title += Instruction.Name;
			}
		}

		public bool HasContent { get; private set; }
		public XAlarmType AlarmType { get; private set; }
		public XStateType StateType { get; private set; }
		public XInstruction Instruction { get; private set; }

		XInstruction FindInstruction(XDevice device, XZone zone)
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