using FiresecAPI.Models;
using Infrastructure.Common;

namespace InstructionsModule.ViewModels
{
    public class InstructionViewModel : BaseViewModel
    {
        public InstructionViewModel(Instruction instruction)
        {
            Instruction = instruction;
        }

        public Instruction Instruction { get; private set; }

        public StateType StateType
        {
            get { return Instruction.StateType; }
        }

        public string InstructionText
        {
            get { return Instruction.Text; }
        }

        public ulong InstructionNo
        {
            get { return Instruction.No; }
        }

        public InstructionType InstructionType
        {
            get { return Instruction.InstructionType; }
        }

        public void Update()
        {
            OnPropertyChanged("Instruction");
        }
    }
}