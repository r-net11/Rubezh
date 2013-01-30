using System.Collections.Generic;
using System.Collections.ObjectModel;
using Infrastructure.Common.Windows.ViewModels;
using ServerFS2;

namespace ClientFS2.ViewModels
{
    public class JournalViewModel : DialogViewModel
    {
        public JournalViewModel(Device device)
        {
            Title = "Журнал событий";
            JournalInitialize(device.JournalItems);
        }

        void JournalInitialize(List<JournalItem> journalItems)
        {
            JournalItems = new ObservableCollection<JournalItem>();
            foreach (var journalItem in journalItems)
            {
                JournalItems.Add(journalItem);
            }
            OnPropertyChanged("JournalItems");
        }

        public ObservableCollection<JournalItem> JournalItems { get; set; }
    }
}
