using Infrastructure.Common;
using Infrastructure.Common.Windows.ViewModels;
using RubezhDAL.DataClasses;

namespace GKImitator.ViewModels
{
	public partial class DescriptorViewModel
	{
		public void InitializeController()
		{
			GoToTechnoligicalRegimeCommand = new RelayCommand(OnGoToTechnoligicalRegime);
			GoToWorkingRegimeCommand = new RelayCommand(OnGoToWorkingRegime);
			EnterUserCommand = new RelayCommand(OnEnterUser);
			ExitUserCommand = new RelayCommand(OnExitUser);
		}

		public bool HasTechnoligicalCommands { get; private set; }
		public bool HasUserCommands { get; private set; }

		public RelayCommand GoToTechnoligicalRegimeCommand { get; private set; }
		void OnGoToTechnoligicalRegime()
		{
			var journalItem = new ImitatorJournalItem(0, 0, 0, 0);
			AddJournalItem(journalItem);
		}

		public RelayCommand GoToWorkingRegimeCommand { get; private set; }
		void OnGoToWorkingRegime()
		{
			var journalItem = new ImitatorJournalItem(0, 6, 0, 0);
			AddJournalItem(journalItem);
		}

		public RelayCommand EnterUserCommand { get; private set; }
		void OnEnterUser()
		{
			var journalItem = new ImitatorJournalItem(0, 7, 0, 0);
			AddJournalItem(journalItem);
		}

		public RelayCommand ExitUserCommand { get; private set; }
		void OnExitUser()
		{
			var journalItem = new ImitatorJournalItem(0, 8, 0, 0);
			AddJournalItem(journalItem);
		}

		public void SynchronyzeDateTime()
		{
			var journalItem = new ImitatorJournalItem(0, 2, 0, 0);
			AddJournalItem(journalItem);
		}
	}
}