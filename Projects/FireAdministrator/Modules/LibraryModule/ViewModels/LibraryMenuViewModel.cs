using Infrastructure.Common;

namespace LibraryModule.ViewModels
{
    public class LibraryMenuViewModel
    {
        public LibraryMenuViewModel(RelayCommand saveCommand)
        {
            SaveCommand = saveCommand;
        }

        public RelayCommand SaveCommand { get; private set; }
    }
}