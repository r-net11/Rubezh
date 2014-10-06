using FiresecAPI.GK;
using Infrastructure.Common.Windows.ViewModels;

namespace GKModule.ViewModels
{
	public class InstructionViewModel : BaseViewModel
	{
		public GKInstruction Instruction { get; private set; }

		public InstructionViewModel(GKInstruction instruction)
		{
			Instruction = instruction;
		}

		public GKInstructionType InstructionType
		{
			get { return Instruction.InstructionType; }
		}

		public bool HasDevices
		{
			get { return Instruction.Devices.Count > 0; }
		}

		public bool HasZones
		{
			get { return Instruction.ZoneUIDs.Count > 0; }
		}

		public bool HasDirections
		{
			get { return Instruction.Directions.Count > 0; }
		}
		
		public void Update()
		{
			OnPropertyChanged(() => Instruction);
			OnPropertyChanged(() => InstructionType);
			OnPropertyChanged(() => HasDevices);
			OnPropertyChanged(() => HasZones);
			OnPropertyChanged(() => HasDirections);
		}
	}
}