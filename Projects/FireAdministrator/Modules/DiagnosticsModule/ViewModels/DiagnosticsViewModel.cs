using System;
using System.Linq;
using System.Windows.Media;
using FiresecAPI.Journal;
using FiresecClient;
using Infrastructure.Common;
using Infrastructure.Common.Windows.ViewModels;
using System.ServiceModel;

namespace DiagnosticsModule.ViewModels
{
	public class DiagnosticsViewModel : ViewPartViewModel
	{
		public DiagnosticsViewModel()
		{
			AddJournalCommand = new RelayCommand(OnAddJournal);
		}

		public RelayCommand AddJournalCommand { get; private set; }
		void OnAddJournal()
		{
		}
	}
}