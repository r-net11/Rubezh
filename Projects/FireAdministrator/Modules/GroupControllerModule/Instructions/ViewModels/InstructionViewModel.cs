using System;
using System.Collections.Generic;
using System.Linq;
using Infrastructure.Common.Windows.ViewModels;
using XFiresecAPI;
using FiresecClient;
using System.Windows.Documents;

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
		}

		public bool HasDevices
		{
			get { return Instruction.Devices.Count > 0; }
		}
		
		public void Update()
		{
			OnPropertyChanged("Instruction");
			OnPropertyChanged("HasDevices");
			OnPropertyChanged("InstructionType");
		}
	}
}