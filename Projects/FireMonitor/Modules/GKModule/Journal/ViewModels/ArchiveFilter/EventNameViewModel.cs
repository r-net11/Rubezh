using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Infrastructure.Common.Windows.ViewModels;
using XFiresecAPI;

namespace GKModule.ViewModels
{
    public class JournalDescriptionStateViewModel : BaseViewModel
    {
        public JournalDescriptionStateViewModel(JournalDescriptionState journalDescriptionState)
        {
            JournalDescriptionState = journalDescriptionState;
        }

        public JournalDescriptionState JournalDescriptionState { get; private set; }

        bool _isChecked;
        public bool IsChecked
        {
            get { return _isChecked; }
            set
            {
                _isChecked = value;
                OnPropertyChanged("IsChecked");
            }
        }
    }
}
