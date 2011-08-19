using Infrastructure.Common;

namespace DevicesModule.ViewModels
{
    public class DevicesMenuViewModel
    {
        public DevicesMenuViewModel(RelayCommand copyCommand, RelayCommand cutCommand, RelayCommand pasteCommand)
        {
            CopyCommand = copyCommand;
            CutCommand = cutCommand;
            PasteCommand = pasteCommand;
        }

        public RelayCommand CopyCommand { get; private set; }
        public RelayCommand CutCommand { get; private set; }
        public RelayCommand PasteCommand { get; private set; }
    }
}