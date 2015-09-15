using System;
using System.Collections.ObjectModel;
using System.Linq;
using FiresecAPI.GK;
using FiresecClient;
using GKImitator.Processor;
using GKProcessor;
using Infrastructure.Common;
using Infrastructure.Common.Windows.ViewModels;
using FiresecAPI.Journal;
using SKDDriver.DataClasses;

namespace GKImitator.ViewModels
{
	public partial class DescriptorViewModel : BaseViewModel
	{
		public void InitializeTest()
		{
			TestButtonCommand = new RelayCommand(OnTestButton);
			TestLaserCommand = new RelayCommand(OnTestLaser);
			ResetTestCommand = new RelayCommand(OnResetTest);
		}

		public bool HasTestButton { get; private set; }
		public bool HasTestLaser { get; private set; }
		public bool HasResetTest { get; private set; }

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