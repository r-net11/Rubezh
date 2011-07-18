using Infrastructure;
using Infrastructure.Common;

namespace JournalModule.ViewModels
{
    public class JournalViewModel : RegionViewModel
    {
        public JournalViewModel()
        {
            CreateCommand = new RelayCommand(OnCreate);
        }

        public void Initialize()
        {
        }

        public RelayCommand CreateCommand { get; private set; }
        void OnCreate()
        {
            NewJournalViewModel newJournalViewModel = new NewJournalViewModel();
            ServiceFactory.UserDialogs.ShowModalWindow(newJournalViewModel);
        }
    }
}
