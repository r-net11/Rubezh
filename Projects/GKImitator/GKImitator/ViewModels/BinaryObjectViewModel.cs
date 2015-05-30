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
	public class BinaryObjectViewModel : BaseViewModel
	{
		public BinaryObjectViewModel(BaseDescriptor binaryObject)
		{
			SetAutomaticRegimeCommand = new RelayCommand(OnSetAutomaticRegime);
			SetManualRegimeCommand = new RelayCommand(OnSetManualRegime);
			SetIgnoreRegimeCommand = new RelayCommand(OnSetIgnoreRegime);

			BinaryObject = binaryObject;
			Description = binaryObject.GKBase.PresentationName;

			StateBits = new ObservableCollection<StateBitViewModel>();
			StateBits.Add(new StateBitViewModel(this, GKStateBit.Norm, true));
			StateBits.Add(new StateBitViewModel(this, GKStateBit.Ignore));
			StateBits.Add(new StateBitViewModel(this, GKStateBit.Fire1));
			StateBits.Add(new StateBitViewModel(this, GKStateBit.Fire2));
			StateBits.Add(new StateBitViewModel(this, GKStateBit.On));

			InitializeFailureItems();
		}

		public BaseDescriptor BinaryObject { get; set; }
		public string Description { get; set; }

		public ObservableCollection<StateBitViewModel> StateBits { get; private set; }

		void InitializeFailureItems()
		{
			if (BinaryObject.GKBase is GKDevice)
			{
				Failures = new ObservableCollection<FailureViewModel>();
				Failures.Add(new FailureViewModel(this, JournalEventDescriptionType.Потеря_связи, 255));
			}
			CanSetFailures = Failures.Any();
		}

		public ObservableCollection<FailureViewModel> Failures { get; private set; }
		public bool CanSetFailures { get; private set; }

		public RelayCommand SetAutomaticRegimeCommand { get; private set; }
		void OnSetAutomaticRegime()
		{
			StateBits.FirstOrDefault(x => x.StateBit == GKStateBit.Norm).IsActive = true;
			StateBits.FirstOrDefault(x => x.StateBit == GKStateBit.Ignore).IsActive = false;
			var journalItem = new ImitatorJournalItem();
			journalItem.Source = 2;
			journalItem.Code = 10;
			journalItem.EventDescription = 0;
			journalItem.ObjectNo = 0;
			AddJournalItem(journalItem);
		}

		public RelayCommand SetManualRegimeCommand { get; private set; }
		void OnSetManualRegime()
		{
			StateBits.FirstOrDefault(x => x.StateBit == GKStateBit.Norm).IsActive = false;
			StateBits.FirstOrDefault(x => x.StateBit == GKStateBit.Ignore).IsActive = false;
			var journalItem = new ImitatorJournalItem();
			journalItem.Source = 2;
			journalItem.Code = 10;
			journalItem.EventDescription = 1;
			journalItem.ObjectNo = 0;
			AddJournalItem(journalItem);
		}

		public RelayCommand SetIgnoreRegimeCommand { get; private set; }
		void OnSetIgnoreRegime()
		{
			StateBits.FirstOrDefault(x => x.StateBit == GKStateBit.Norm).IsActive = false;
			StateBits.FirstOrDefault(x => x.StateBit == GKStateBit.Ignore).IsActive = true;
			var journalItem = new ImitatorJournalItem();
			journalItem.Source = 2;
			journalItem.Code = 10;
			journalItem.EventDescription = 2;
			journalItem.ObjectNo = 0;
			AddJournalItem(journalItem);
		}
		public void AddJournalItem(ImitatorJournalItem journalItem)
		{
			var state = 0;
			foreach (var stateBitViewModel in StateBits)
			{
				if (stateBitViewModel.IsActive)
				{
					state += (1 << (int)stateBitViewModel.StateBit);
				}
			}

			journalItem.UNUSED_KauNo = 0;
			journalItem.UNUSED_KauAddress = 0;
			journalItem.GkNo = JournalHelper.ImitatorJournalItemCollection.ImitatorJournalItems.Count + 1;
			journalItem.GkObjectNo = BinaryObject.GetDescriptorNo();
			journalItem.ObjectFactoryNo = 0;
			journalItem.ObjectState = state;
			if (BinaryObject.GKBase is GKDevice)
			{
				journalItem.ObjectDeviceType = (short)(BinaryObject.GKBase as GKDevice).Driver.DriverTypeNo;
				journalItem.ObjectDeviceAddress = (short)(((BinaryObject.GKBase as GKDevice).ShleifNo - 1) * 256 + (BinaryObject.GKBase as GKDevice).IntAddress);
			}
			JournalHelper.ImitatorJournalItemCollection.ImitatorJournalItems.Add(journalItem);
			JournalHelper.Save();
		}
	}
}