using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Infrastructure.Common;
using Infrastructure;

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
