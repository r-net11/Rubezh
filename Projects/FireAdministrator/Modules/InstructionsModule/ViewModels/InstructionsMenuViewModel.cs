using Infrastructure.Common;

namespace InstructionsModule.ViewModels
{
    public class InstructionsMenuViewModel
    {
        public InstructionsMenuViewModel(InstructionsViewModel instructionsViewModel)
        {
            Contex = instructionsViewModel;
        }

        public InstructionsViewModel Contex { get; private set; }
    }
}
