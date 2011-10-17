using Infrastructure.Common;

namespace PlansModule.ViewModels
{
    public class PlansContextMenuViewModel
    {
        public PlansContextMenuViewModel(RelayCommand addCommand, RelayCommand addSubCommand, RelayCommand editCommand, RelayCommand removeCommand)
        {
            AddCommand = addCommand;
            AddSubCommand = addSubCommand;
            RemoveCommand = removeCommand;
            EditCommand = editCommand;
        }

        public RelayCommand AddCommand { get; private set; }
        public RelayCommand AddSubCommand { get; private set; }
        public RelayCommand EditCommand { get; private set; }
        public RelayCommand RemoveCommand { get; private set; }
    }
}