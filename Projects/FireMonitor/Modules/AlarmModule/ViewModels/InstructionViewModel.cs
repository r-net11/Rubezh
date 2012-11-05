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
			Title = "Инструкции";
			StateType = AlarmTypeToStateType(alarmType);

            Instruction = FindInstruction(device, zone);
			HasContent = Instruction != null;
		}

		public bool HasContent { get; private set; }
		public StateType StateType { get; private set; }
		public Instruction Instruction { get; private set; }

		StateType AlarmTypeToStateType(AlarmType alarmType)
		{
			switch (alarmType)
			{
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

				case AlarmType.Service:
					return StateType.Service;

				default:
					return StateType.No;
			}
		}

		Instruction FindInstruction(Device device, Zone zone)
		{
			var availableStateTypeInstructions = FiresecClient.FiresecManager.SystemConfiguration.Instructions.FindAll(x => x.StateType == StateType);

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
				if (instruction.InstructionType == InstructionType.General)
				{
					return instruction;
				}
			}

			return null;
		}
	}
}