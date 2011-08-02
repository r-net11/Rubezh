using Infrastructure.Common;

namespace FiltersModule.ViewModels
{
    public class FiltersMenuViewModel
    {
        public FiltersMenuViewModel(RelayCommand addCommand, RelayCommand editCommand, RelayCommand removeCommand, RelayCommand saveCommand)
        {
            AddCommand = addCommand;
            RemoveCommand = removeCommand;
            EditCommand = editCommand;
            SaveCommand = saveCommand;
        }

        public RelayCommand AddCommand { get; private set; }
        public RelayCommand EditCommand { get; private set; }
        public RelayCommand RemoveCommand { get; private set; }
        public RelayCommand SaveCommand { get; private set; }
    }
}
