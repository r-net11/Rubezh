using Infrastructure.Common.Windows.ViewModels;
using Infrastructure.Models;

namespace GKModule.ViewModels
{
	public class JournalColumnTypeViewModel : BaseViewModel
	{
		public JournalColumnTypeViewModel(XJournalColumnType journalColumnType)
		{
			JournalColumnType = journalColumnType;
		}

		public XJournalColumnType JournalColumnType { get; private set; }

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