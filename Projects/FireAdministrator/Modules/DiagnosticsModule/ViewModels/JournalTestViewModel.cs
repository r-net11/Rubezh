using System.Collections.ObjectModel;
using Common.GK.DB;
using Infrastructure.Common;
using Infrastructure.Common.Windows.ViewModels;

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