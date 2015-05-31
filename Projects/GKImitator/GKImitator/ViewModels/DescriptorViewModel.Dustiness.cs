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
		public void InitializeDustiness()
		{
			SetPreDustinessCommand = new RelayCommand(OnSetPreDustiness);
			SetCriticalDustinessCommand = new RelayCommand(OnSetCriticalDustiness);
			ResetDustinessCommand = new RelayCommand(OnResetDustiness);

			if (GKBase is GKDevice)
			{
				var device = GKBase as GKDevice;
				switch(device.DriverType)
				{
					case GKDriverType.RSR2_SmokeDetector:
					case GKDriverType.RSR2_CombinedDetector:
					case GKDriverType.RSR2_HeatDetector:
						HasDustiness = true;
						break;
				}
			}
		}

		public bool HasDustiness { get; private set; }

		public RelayCommand SetPreDustinessCommand { get; private set; }
		void OnSetPreDustiness()
		{
			var journalItem = new ImitatorJournalItem();
			journalItem.Source = 2;
			journalItem.NameCode = 7;
			journalItem.DescriptionCode = 1;
			journalItem.YesNoCode = 1;
			AddJournalItem(journalItem);
		}

		public RelayCommand SetCriticalDustinessCommand { get; private set; }
		void OnSetCriticalDustiness()
		{
			var journalItem = new ImitatorJournalItem();
			journalItem.Source = 2;
			journalItem.NameCode = 7;
			journalItem.DescriptionCode = 2;
			journalItem.YesNoCode = 1;
			AddJournalItem(journalItem);
		}

		public RelayCommand ResetDustinessCommand { get; private set; }
		void OnResetDustiness()
		{
			var journalItem = new ImitatorJournalItem();
			journalItem.Source = 2;
			journalItem.NameCode = 7;
			journalItem.DescriptionCode = 0;
			journalItem.YesNoCode = 0;
			AddJournalItem(journalItem);
		}
	}
}