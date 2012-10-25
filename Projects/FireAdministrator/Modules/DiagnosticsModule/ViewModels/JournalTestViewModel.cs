using System;
using System.Linq;
using System.Collections.Generic;
using System.Windows.Input;
using Common.GK;
using Infrastructure.Common;
using Infrastructure.Common.Windows.ViewModels;
using System.Collections.ObjectModel;
using System.Diagnostics;
using Common.GK.DB;

namespace DiagnosticsModule.ViewModels
{
	public class JournalTestViewModel : DialogViewModel
	{
		public JournalTestViewModel()
		{
            AddCommand = new RelayCommand(OnAdd);
            SelectCommand = new RelayCommand(OnSelect);
            JournalRecords = new ObservableCollection<Journal>();
		}

        public ObservableCollection<Journal> JournalRecords { get; private set; }

        public RelayCommand AddCommand { get; private set; }
        void OnAdd()
        {
        }

        public RelayCommand SelectCommand { get; private set; }
        void OnSelect()
        {
        }
	}
}