using FiresecAPI;
using FiresecAPI.Models;
using Infrastructure.Common.Windows.ViewModels;

namespace AlarmModule.ViewModels
{
	public class InstructionViewModel : DialogViewModel, IWindowIdentity
	{
		public InstructionViewModel(Device device, Zone zone, AlarmType alarmType)
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
		public AlarmType AlarmType { get; private set; }
		public StateType StateType { get; private set; }
		public Instruction Instruction { get; private set; }

		Instruction FindInstruction(Device device, Zone zone)
		{
			var availableStateTypeInstructions = FiresecClient.FiresecManager.SystemConfiguration.Instructions.FindAll(x => x.AlarmType == AlarmType);

			if (device != null)
			{
				foreach (var instruction in availableStateTypeInstructions)
				{
					if (device.ParentPanel != null && instruction.Devices.Contains(device.ParentPanel.UID))
					{
						return instruction;
					}
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

				foreach (var deviceInZone in zone.DevicesInZone)
				{
					foreach (var instruction in availableStateTypeInstructions)
					{
						if (instruction.Devices.Contains(deviceInZone.ParentPanel.UID))
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

		#region IWindowIdentity Members
		public string Guid
		{
			get { return "Instruction"; }
		}
		#endregion
	}
}