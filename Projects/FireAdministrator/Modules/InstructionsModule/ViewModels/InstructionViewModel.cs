using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Infrastructure.Common;
using FiresecAPI.Models;

namespace InstructionsModule.ViewModels
{
    public class InstructionViewModel : BaseViewModel
    {
        public InstructionViewModel(Instruction instruction)
        {
            Instruction = instruction;
        }

        public Instruction Instruction { get; private set; }

        public void Update()
        {
            OnPropertyChanged("Instruction");
        }
    }
}
