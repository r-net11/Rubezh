using Infrastructure.Common;

namespace InstructionsModule.ViewModels
{
    public class InstructionsMenuViewModel
    {
        public InstructionsMenuViewModel(RelayCommand addCommand, RelayCommand editCommand,
            RelayCommand deleteCommand, RelayCommand deleteAllCommand)
        {
            AddCommand = addCommand;
            DeleteCommand = deleteCommand;
            EditCommand = editCommand;
            DeleteAllCommand = deleteAllCommand;
        }

        public RelayCommand AddCommand { get; private set; }
        public RelayCommand EditCommand { get; private set; }
        public RelayCommand DeleteCommand { get; private set; }
        public RelayCommand DeleteAllCommand { get; private set; }
    }
}
