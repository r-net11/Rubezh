using RubezhAPI.GK;
using Infrastructure.Common;
using Infrastructure.Common.Windows.ViewModels;
using RubezhDAL.DataClasses;

namespace GKImitator.ViewModels
{
	public partial class DescriptorViewModel
	{
		public void InitializeTest()
		{
			TestButtonCommand = new RelayCommand(OnTestButton);
			TestLaserCommand = new RelayCommand(OnTestLaser);
			ResetTestCommand = new RelayCommand(OnResetTest);
		}

		public bool HasTest { get; private set; }

        public RelayCommand TestButtonCommand { get; private set; }
		void OnTestButton()
		{
			SetStateBit(GKStateBit.Test, true);
			var journalItem = new ImitatorJournalItem(2, 6, 1, 1);
			AddJournalItem(journalItem);
			RecalculateOutputLogic();
		}

		public RelayCommand TestLaserCommand { get; private set; }
		void OnTestLaser()
		{
			SetStateBit(GKStateBit.Test, true);
			var journalItem = new ImitatorJournalItem(2, 6, 2, 1);
			AddJournalItem(journalItem);
			RecalculateOutputLogic();
		}

		public RelayCommand ResetTestCommand { get; private set; }
		void OnResetTest()
		{
			SetStateBit(GKStateBit.Test, false);
			var journalItem = new ImitatorJournalItem(2, 6, 0, 0);
			AddJournalItem(journalItem);
			RecalculateOutputLogic();
		}
	}
}