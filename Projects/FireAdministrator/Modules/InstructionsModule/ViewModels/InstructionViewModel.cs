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

        public string InstructionName
        {
            get { return Instruction.Name; }
        }

        public StateType StateType
        {
            get { return Instruction.StateType; }
        }

        public string InstructionText
        {
            get { return Instruction.Text; }
        }

        public List<InstructionZone> InstructionZones
        {
            get { return Instruction.InstructionZones; }
        }

        public Dictionary<string, string> InstructionDevices
        {
            get { return Instruction.InstructionDevices; }
        }

        public Cover InstructionCover
        {
            get { return Instruction.Cover; }
        }

        public void Update()
        {
            OnPropertyChanged("Instruction");
        }
    }
}
