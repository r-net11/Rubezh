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
		public void InitializeController()
		{
			GoToTechnoligicalRegimeCommand = new RelayCommand(OnGoToTechnoligicalRegime);
			GoToWorkingRegimeCommand = new RelayCommand(OnGoToWorkingRegime);
			EnterUserCommand = new RelayCommand(OnEnterUser);
			ExitUserCommand = new RelayCommand(OnExitUser);

			if (GKBase is GKDevice)
			{
				var device = GKBase as GKDevice;
				switch(device.DriverType)
				{
					case GKDriverType.GK:
						HasTechnoligicalCommands = true;
						HasUserCommands = true;
						break;

					case GKDriverType.RSR2_KAU:
						HasTechnoligicalCommands = true;
						break;
				}
			}
		}

		public bool HasTechnoligicalCommands { get; private set; }
		public bool HasUserCommands { get; private set; }

		public RelayCommand GoToTechnoligicalRegimeCommand { get; private set; }
		void OnGoToTechnoligicalRegime()
		{
			var journalItem = new ImitatorJournalItem();
			journalItem.Source = 0;
			journalItem.NameCode = 0;
			journalItem.DescriptionCode = 0;
			journalItem.YesNoCode = 0;
			AddJournalItem(journalItem);
		}

		public RelayCommand GoToWorkingRegimeCommand { get; private set; }
		void OnGoToWorkingRegime()
		{
			var journalItem = new ImitatorJournalItem();
			journalItem.Source = 0;
			journalItem.NameCode = 6;
			journalItem.DescriptionCode = 0;
			journalItem.YesNoCode = 0;
			AddJournalItem(journalItem);
		}

		public RelayCommand EnterUserCommand { get; private set; }
		void OnEnterUser()
		{
			var journalItem = new ImitatorJournalItem();
			journalItem.Source = 0;
			journalItem.NameCode = 7;
			journalItem.DescriptionCode = 0;
			journalItem.YesNoCode = 0;
			AddJournalItem(journalItem);
		}

		public RelayCommand ExitUserCommand { get; private set; }
		void OnExitUser()
		{
			var journalItem = new ImitatorJournalItem();
			journalItem.Source = 0;
			journalItem.NameCode = 8;
			journalItem.DescriptionCode = 0;
			journalItem.YesNoCode = 0;
			AddJournalItem(journalItem);
		}

		public void SynchronyzeDateTime()
		{

		}
	}
}