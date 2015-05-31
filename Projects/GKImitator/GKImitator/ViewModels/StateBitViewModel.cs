using System;
using FiresecAPI.GK;
using GKImitator.Processor;
using GKProcessor;
using Infrastructure.Common.Windows.ViewModels;

namespace GKImitator.ViewModels
{
	public class StateBitViewModel : BaseViewModel
	{
		public GKStateBit StateBit { get; private set; }
		DescriptorViewModel DescriptorViewModel;

		public StateBitViewModel(DescriptorViewModel descriptorViewModel, GKStateBit stateBit, bool isActive = false)
		{
			DescriptorViewModel = descriptorViewModel;
			StateBit = stateBit;
			_isActive = isActive;
		}

		public bool IsEnabled
		{
			get { return StateBit != GKStateBit.Norm && StateBit != GKStateBit.Ignore && StateBit != GKStateBit.Test; }
		}

		bool _isActive;
		public bool IsActive
		{
			get { return _isActive; }
			set
			{
				if (_isActive != value)
				{
					_isActive = value;
					OnPropertyChanged(() => IsActive);

					switch (StateBit)
					{
						case GKStateBit.Fire1:
							if (_isActive)
							{
								AddJournalItem(2, 0, 0);
							}
							else
							{
								AddJournalItem(14, 0, 0);
							}
							break;

						case GKStateBit.Fire2:
							if (_isActive)
							{
								AddJournalItem(3, 0, 0);
							}
							else
							{
								AddJournalItem(14, 0, 0);
							}
							break;

						case GKStateBit.On:
							if (_isActive)
							{
								AddJournalItem(9, 2, 0);
							}
							break;

						case GKStateBit.Off:
							if (_isActive)
							{
								AddJournalItem(9, 3, 0);
							}
							break;

						case GKStateBit.TurningOn:
							if (_isActive)
							{
								AddJournalItem(9, 4, 0);
							}
							break;

						case GKStateBit.TurningOff:
							if (_isActive)
							{
								AddJournalItem(9, 5, 0);
							}
							break;
					}

					MainViewModel.Current.RebuildIndicators();
				}
			}
		}

		void AddJournalItem(byte nameCode, byte descriptionCode, byte yesNoCode)
		{
			var journalItem = new ImitatorJournalItem();
			journalItem.Source = 2;
			journalItem.NameCode = nameCode;
			journalItem.DescriptionCode = descriptionCode;
			journalItem.YesNoCode = yesNoCode;
			DescriptorViewModel.AddJournalItem(journalItem);
		}
	}
}