using Infrastructure.Common;
using Infrastructure.Common.Windows.ViewModels;
using RubezhDAL.DataClasses;

namespace GKImitator.ViewModels
{
	public partial class DescriptorViewModel
	{
		public void InitializeDustiness()
		{
			SetPreDustinessCommand = new RelayCommand(OnSetPreDustiness);
			SetCriticalDustinessCommand = new RelayCommand(OnSetCriticalDustiness);
			ResetDustinessCommand = new RelayCommand(OnResetDustiness);
		}

		public bool HasDustiness { get; private set; }

		public RelayCommand SetPreDustinessCommand { get; private set; }
		void OnSetPreDustiness()
		{
			var journalItem = new ImitatorJournalItem(2, 7, 1, 1);
			AddJournalItem(journalItem);
		}

		public RelayCommand SetCriticalDustinessCommand { get; private set; }
		void OnSetCriticalDustiness()
		{
			var journalItem = new ImitatorJournalItem(2, 7, 2, 1);
			AddJournalItem(journalItem);
		}

		public RelayCommand ResetDustinessCommand { get; private set; }
		void OnResetDustiness()
		{
			var journalItem = new ImitatorJournalItem(2, 7, 0, 0);
			AddJournalItem(journalItem);
		}
	}
}