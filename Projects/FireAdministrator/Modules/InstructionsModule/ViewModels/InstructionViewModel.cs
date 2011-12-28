using FiresecAPI.Models;
using Infrastructure.Common;

namespace InstructionsModule.ViewModels
{
    public class InstructionViewModel : BaseViewModel
    {
        public Instruction Instruction { get; private set; }

        public InstructionViewModel(Instruction instruction)
        {
            Instruction = instruction;
        }

        public void Update()
        {
            OnPropertyChanged("Instruction");
        }
    }
}