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
		public InstructionViewModel(Guid deviceUID, Guid zoneUID, AlarmType alarmType)
		{
			Title = "Инструкции";
			StateType = AlarmTypeToStateType(alarmType);
			DeviceId = deviceUID;

			Instruction = FindInstruction(deviceUID, zoneUID);
			HasContent = Instruction != null;
		}

		public bool HasContent { get; private set; }
		public Guid DeviceId { get; private set; }
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

		Instruction FindInstruction(Guid deviceUID, Guid zoneUID)
		{
			var availableStateTypeInstructions = FiresecClient.FiresecManager.SystemConfiguration.Instructions.FindAll(x => x.StateType == StateType);

			if (deviceUID != Guid.Empty)
			{
				var device = FiresecManager.Devices.FirstOrDefault(x => x.UID == DeviceId);
				if (device != null)
				{
					foreach (var instruction in availableStateTypeInstructions)
					{
						if (instruction.Devices.Contains(deviceUID))
						{
							return instruction;
						}
					}
				}
			}

			if (zoneUID != Guid.Empty)
			{
				var zone = FiresecManager.Zones.FirstOrDefault(x => x.UID == zoneUID);
				if (zone != null)
				{
					foreach (var instruction in availableStateTypeInstructions)
					{
						if (instruction.ZoneUIDs.Contains(zoneUID))
						{
							return instruction;
						}
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