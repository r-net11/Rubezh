using System;
using System.Collections.Generic;
using System.Linq;
using FiresecAPI.Models;
using Infrastructure.Common;
using Infrastructure.Common.Windows.ViewModels;

namespace InstructionsModule.ViewModels
{
    public class InstructionViewModel : BaseViewModel
    {
        public Instruction Instruction { get; private set; }

        public InstructionViewModel(Instruction instruction)
        {
            Instruction = instruction;
        }

        public InstructionType InstructionType
        {
            get { return Instruction.InstructionType; }
            set
            {
                Instruction.InstructionType = value;
                OnPropertyChanged("InstructionType");
            }
        }

        public List<InstructionType> InstructionTypes
        {
            get { return Enum.GetValues(typeof(InstructionType)).Cast<InstructionType>().ToList(); }
        }

        public void Update()
        {
            OnPropertyChanged("Instruction");
        }
    }
}