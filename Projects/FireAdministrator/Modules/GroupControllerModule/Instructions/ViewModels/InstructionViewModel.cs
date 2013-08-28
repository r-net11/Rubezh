using System;
using System.Collections.Generic;
using System.Linq;
using Infrastructure.Common.Windows.ViewModels;
using XFiresecAPI;

namespace GKModule.ViewModels
{
	public class InstructionViewModel : BaseViewModel
	{
		public XInstruction Instruction { get; private set; }

		public InstructionViewModel(XInstruction instruction)
		{
			Instruction = instruction;
		}

		public XInstructionType InstructionType
		{
			get { return Instruction.InstructionType; }
			set
			{
				Instruction.InstructionType = value;
				OnPropertyChanged("InstructionType");
			}
		}

		public List<XInstructionType> InstructionTypes
		{
			get { return Enum.GetValues(typeof(XInstructionType)).Cast<XInstructionType>().ToList(); }
		}

		public void Update()
		{
			OnPropertyChanged("Instruction");
		}
	}
}