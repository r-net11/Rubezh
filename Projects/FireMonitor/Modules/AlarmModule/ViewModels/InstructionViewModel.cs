using System;
using System.Collections.Generic;
using System.Linq;
using FiresecAPI;
using FiresecAPI.Models;
using FiresecClient;
using Infrastructure.Common.Windows.ViewModels;

namespace AlarmModule.ViewModels
{
	public class InstructionViewModel : DialogViewModel
	{
		public InstructionViewModel(Device device, Zone zone, AlarmType alarmType)
		{
			Title = "Инструкция ";
			StateType = AlarmTypeToStateType(alarmType);
            AlarmType = alarmType;

            Instruction = FindInstruction(device, zone);
			HasContent = Instruction != null;
			if (Instruction != null)
			{
				Title += Instruction.Name;
			}
		}

		public bool HasContent { get; private set; }
        public AlarmType AlarmType { get; private set; }
		public StateType StateType { get; private set; }
		public Instruction Instruction { get; private set; }

		StateType AlarmTypeToStateType(AlarmType alarmType)
		{
			switch (alarmType)
			{
				case AlarmType.Guard:
				case AlarmType.Fire:
					return StateType.Fire;

				case AlarmType.Attention:
					return StateType.Attention;

				case AlarmType.Failure:
					return StateType.Failure;

				case AlarmType.Off:
					return StateType.Off;

				case AlarmType.Info:
					return StateType.Info;

				case AlarmType.Auto:
				case AlarmType.Service:
					return StateType.Service;

				default:
					return StateType.No;
			}
		}

		Instruction FindInstruction(Device device, Zone zone)
		{
            var availableStateTypeInstructions = FiresecClient.FiresecManager.SystemConfiguration.Instructions.FindAll(x => x.AlarmType == AlarmType);

            if (device != null && device.ParentPanel != null)
            {
                foreach (var instruction in availableStateTypeInstructions)
                {
					if (instruction.Devices.Contains(device.ParentPanel.UID))
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
				if (instruction.InstructionType == InstructionType.General)
				{
					return instruction;
				}
			}

			return null;
		}
	}
}