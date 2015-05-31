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

namespace GKImitator.ViewModels
{
	public partial class DescriptorViewModel : BaseViewModel
	{
		public void InitializeTest()
		{
			TestButtonCommand = new RelayCommand(OnTestButton);
			TestLaserCommand = new RelayCommand(OnTestLaser);
			ResetTestCommand = new RelayCommand(OnResetTest);

			if (GKBase is GKDevice)
			{
				var device = GKBase as GKDevice;
				switch(device.DriverType)
				{
					case GKDriverType.RSR2_SmokeDetector:
					case GKDriverType.RSR2_CombinedDetector:
					case GKDriverType.RSR2_HeatDetector:
						HasTestButton = true;
						HasTestLaser = true;
						HasResetTest = true;
						StateBits.Add(new StateBitViewModel(this, GKStateBit.Test));
						break;
				}
			}
		}

		public bool HasTestButton { get; private set; }
		public bool HasTestLaser { get; private set; }
		public bool HasResetTest { get; private set; }

		public RelayCommand TestButtonCommand { get; private set; }
		void OnTestButton()
		{
			StateBits.FirstOrDefault(x => x.StateBit == GKStateBit.Test).IsActive = true;
			var journalItem = new ImitatorJournalItem();
			journalItem.Source = 2;
			journalItem.NameCode = 6;
			journalItem.DescriptionCode = 1;
			journalItem.YesNoCode = 1;
			AddJournalItem(journalItem);
		}

		public RelayCommand TestLaserCommand { get; private set; }
		void OnTestLaser()
		{
			StateBits.FirstOrDefault(x => x.StateBit == GKStateBit.Test).IsActive = true;
			var journalItem = new ImitatorJournalItem();
			journalItem.Source = 2;
			journalItem.NameCode = 6;
			journalItem.DescriptionCode = 2;
			journalItem.YesNoCode = 1;
			AddJournalItem(journalItem);
		}

		public RelayCommand ResetTestCommand { get; private set; }
		void OnResetTest()
		{
			StateBits.FirstOrDefault(x => x.StateBit == GKStateBit.Test).IsActive = false;
			var journalItem = new ImitatorJournalItem();
			journalItem.Source = 2;
			journalItem.NameCode = 6;
			journalItem.DescriptionCode = 0;
			journalItem.YesNoCode = 0;
			AddJournalItem(journalItem);
		}
	}
}