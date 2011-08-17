using Infrastructure.Common;

namespace PlansModule.ViewModels
{
    public class PlansMenuViewModel
    {
        public PlansMenuViewModel(RelayCommand addCommand, RelayCommand editCommand, RelayCommand removeCommand)
        {
            AddCommand = addCommand;
            RemoveCommand = removeCommand;
            EditCommand = editCommand;
        }

        public RelayCommand AddCommand { get; private set; }
        public RelayCommand EditCommand { get; private set; }
        public RelayCommand RemoveCommand { get; private set; }
    }
}
