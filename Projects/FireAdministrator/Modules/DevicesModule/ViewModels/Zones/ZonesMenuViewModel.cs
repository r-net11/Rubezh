using Infrastructure.Common;

namespace DevicesModule.ViewModels
{
    public class ZonesMenuViewModel
    {
        public ZonesMenuViewModel(RelayCommand addCommand, RelayCommand deleteCommand, RelayCommand editCommand,
            RelayCommand deleteAllCommand, RelayCommand deleteAllEmptyCommand)
        {
            AddCommand = addCommand;
            DeleteCommand = deleteCommand;
            EditCommand = editCommand;
            DeleteAllCommand = deleteAllCommand;
            DeleteAllEmptyCommand = deleteAllEmptyCommand;
        }

        public RelayCommand AddCommand { get; private set; }
        public RelayCommand DeleteCommand { get; private set; }
        public RelayCommand EditCommand { get; private set; }
        public RelayCommand DeleteAllCommand { get; private set; }
        public RelayCommand DeleteAllEmptyCommand { get; private set; }
    }
}