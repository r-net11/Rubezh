using System.Collections.Generic;
using System.Linq;
using Common.GK;
using FiresecAPI.Models;
using FiresecClient;
using GKModule.Events;
using Infrastructure;
using Infrastructure.Common.Windows;
using Infrastructure.Common.Windows.ViewModels;
using XFiresecAPI;

namespace GKModule.ViewModels
{
    public class JournalsViewModel : ViewPartViewModel
    {
        public JournalsViewModel()
        {
            Journals = new List<JournalViewModel>();
            Journals.Add(new JournalViewModel(new XJournalFilter() { Name = " Все события" }));
            SelectedJournal = Journals.FirstOrDefault();

            ServiceFactory.Events.GetEvent<NewXJournalEvent>().Unsubscribe(OnNewJournal);
            ServiceFactory.Events.GetEvent<NewXJournalEvent>().Subscribe(OnNewJournal);

			var journalItems = GKDBHelper.GetTopLast(100);
			Journals.ForEach(x => x.OnNewJournal(journalItems));
        }

        public void Initialize()
        {
            if (XManager.DeviceConfiguration.JournalFilters != null)
                foreach (var journalFilter in XManager.DeviceConfiguration.JournalFilters)
                {
                    var filteredJournalViewModel = new JournalViewModel(journalFilter);
                    Journals.Add(filteredJournalViewModel);
                }
        }

        List<JournalViewModel> _journals;
        public List<JournalViewModel> Journals
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

        void OnNewJournal(List<JournalItem> journalItems)
        {
            foreach (var journalItem in journalItems)
            {
                if (journalItem.StateClass == XStateClass.Fire1 || journalItem.StateClass == XStateClass.Fire2 || journalItem.StateClass == XStateClass.Attention)
                {
                    if (FiresecManager.CheckPermission(PermissionType.Oper_NoAlarmConfirm) == false)
                    {
                        var confirmationViewModel = new ConfirmationViewModel(journalItem);
                        DialogService.ShowWindow(confirmationViewModel);
                    }
                }
            }
        }
    }
}