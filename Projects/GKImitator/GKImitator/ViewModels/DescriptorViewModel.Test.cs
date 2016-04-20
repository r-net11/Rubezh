using RubezhAPI.GK;
using Infrastructure.Common.Windows;
using Infrastructure.Common.Windows.Windows.ViewModels;
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
			var journalItem = new ImitatorJournalItem(2, 6, 1, 1);
			SetStateBit(GKStateBit.Test, true, journalItem);
		}

		public RelayCommand TestLaserCommand { get; private set; }
		void OnTestLaser()
		{
			var journalItem = new ImitatorJournalItem(2, 6, 2, 1);
			SetStateBit(GKStateBit.Test, true, journalItem);
		}

		public RelayCommand ResetTestCommand { get; private set; }
		void OnResetTest()
		{
			var journalItem = new ImitatorJournalItem(2, 6, 0, 0);
			SetStateBit(GKStateBit.Test, false, journalItem);
		}
	}
}