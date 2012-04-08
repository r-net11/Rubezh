namespace InstructionsModule.ViewModels
{
    public class InstructionsMenuViewModel
    {
        public InstructionsMenuViewModel(InstructionsViewModel instructionsViewModel)
        {
            Context = instructionsViewModel;
        }

        public InstructionsViewModel Context { get; private set; }
    }
}
