using System.Collections.ObjectModel;
using FiresecClient;
using Infrastructure;
using Infrastructure.Common;

namespace JournalModule.ViewModels
{
    public class JournalsViewModel : RegionViewModel
    {
        public JournalsViewModel()
        {
            CreateCommand = new RelayCommand(OnCreate);
            EditCommand = new RelayCommand(OnEdit);
            RemoveCommand = new RelayCommand(OnRemove);
        }

        public void Initialize()
        {
            var journals = new ObservableCollection<JournalViewModel>();
            foreach (var journal in FiresecManager.SystemConfiguration.Journals)
            {
                journals.Add(new JournalViewModel(journal));
            }
            Journals = journals;
        }

        ObservableCollection<JournalViewModel> _journals;
        public ObservableCollection<JournalViewModel> Journals
        {
            get { return _journals; }
            set
            {
                _journals = value;
                OnPropertyChanged("Journals");
            }
        }

        JournalViewModel _selectedJournal;
        public JournalViewModel SelectedJournal
        {
            get { return _selectedJournal; }
            set
            {
                _selectedJournal = value;
                OnPropertyChanged("SelectedJournal");
            }
        }

        public RelayCommand CreateCommand { get; private set; }
        void OnCreate()
        {
            EditJournalViewModel editJournalViewModel = new EditJournalViewModel();
            if (ServiceFactory.UserDialogs.ShowModalWindow(editJournalViewModel))
            {
                editJournalViewModel.JournalViewModel.UpdateProperties();
                Journals.Add(editJournalViewModel.JournalViewModel);
                FiresecManager.SystemConfiguration.Journals.Add(editJournalViewModel.JournalViewModel.Journal);
            }
        }

        public RelayCommand EditCommand { get; private set; }
        void OnEdit()
        {
            if (SelectedJournal != null)
            {
                EditJournalViewModel editJournalViewModel = new EditJournalViewModel(SelectedJournal);
                if (ServiceFactory.UserDialogs.ShowModalWindow(editJournalViewModel))
                {
                    Helper.CopyContent(editJournalViewModel.JournalViewModel, SelectedJournal);
                    SelectedJournal.UpdateProperties();
                }
            }
        }

        public RelayCommand RemoveCommand { get; private set; }
        void OnRemove()
        {
            if (SelectedJournal != null)
            {
                FiresecManager.SystemConfiguration.Journals.Remove(SelectedJournal.Journal);
                Journals.Remove(SelectedJournal);
            }
        }

        public override void OnShow()
        {
            JournalsMenuViewModel journalsMenuViewModel = new JournalsMenuViewModel(CreateCommand, EditCommand, RemoveCommand);
            ServiceFactory.Layout.ShowMenu(journalsMenuViewModel);
        }
    }
}
