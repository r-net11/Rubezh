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
		public BaseDescriptor BaseDescriptor { get; private set; }
		public GKBase GKBase { get { return BaseDescriptor.GKBase; } }
		public int DescriptorNo { get; private set; }

		public DescriptorViewModel(BaseDescriptor descriptor)
		{
			BaseDescriptor = descriptor;
			DescriptorNo = descriptor.GetDescriptorNo();

			SetAutomaticRegimeCommand = new RelayCommand(OnSetAutomaticRegime);
			SetManualRegimeCommand = new RelayCommand(OnSetManualRegime);
			SetIgnoreRegimeCommand = new RelayCommand(OnSetIgnoreRegime);

			InitializeStateBits();
			InitializeFailureItems();
			InitializeTest();
			InitializeDustiness();
			InitializeController();
		}

		public ObservableCollection<StateBitViewModel> StateBits { get; private set; }
		public ObservableCollection<FailureViewModel> Failures { get; private set; }

		void InitializeStateBits()
		{
			StateBits = new ObservableCollection<StateBitViewModel>();
			StateBits.Add(new StateBitViewModel(this, GKStateBit.Norm, true));
			StateBits.Add(new StateBitViewModel(this, GKStateBit.Ignore));

			if (GKBase is GKDevice)
			{
				var device = GKBase as GKDevice;
				switch (device.DriverType)
				{
					case GKDriverType.RSR2_HandDetector:
						StateBits.Add(new StateBitViewModel(this, GKStateBit.Fire2));
						break;

					case GKDriverType.RSR2_SmokeDetector:
					case GKDriverType.RSR2_CombinedDetector:
					case GKDriverType.RSR2_HeatDetector:
						StateBits.Add(new StateBitViewModel(this, GKStateBit.Fire1));
						break;

					case GKDriverType.RSR2_AM_1:
					case GKDriverType.RSR2_MAP4:
						StateBits.Add(new StateBitViewModel(this, GKStateBit.Fire1));
						StateBits.Add(new StateBitViewModel(this, GKStateBit.Fire2));
						break;

					case GKDriverType.RSR2_RM_1:
					case GKDriverType.RSR2_MDU:
					case GKDriverType.RSR2_MDU24:
					case GKDriverType.RSR2_MVK8:
					case GKDriverType.RSR2_Bush_Drenazh:
					case GKDriverType.RSR2_Bush_Jokey:
					case GKDriverType.RSR2_Bush_Fire:
					case GKDriverType.RSR2_Bush_Shuv:
					case GKDriverType.RSR2_Valve_KV:
					case GKDriverType.RSR2_Valve_KVMV:
					case GKDriverType.RSR2_Valve_DU:
					case GKDriverType.RSR2_OPK:
					case GKDriverType.RSR2_OPS:
					case GKDriverType.RSR2_OPZ:
					case GKDriverType.RSR2_Buz_KV:
					case GKDriverType.RSR2_Buz_KVMV:
					case GKDriverType.RSR2_Buz_KVDU:
						StateBits.Add(new StateBitViewModel(this, GKStateBit.On));
						StateBits.Add(new StateBitViewModel(this, GKStateBit.Off));
						StateBits.Add(new StateBitViewModel(this, GKStateBit.TurningOn));
						StateBits.Add(new StateBitViewModel(this, GKStateBit.TurningOff));
						break;

					case GKDriverType.RSR2_MVP:
					case GKDriverType.RSR2_MVP_Part:
					case GKDriverType.RSR2_CodeReader:
					case GKDriverType.RSR2_GuardDetector:
					case GKDriverType.RSR2_CardReader:
					case GKDriverType.RSR2_GuardDetectorSound:
						break;

				}
			}

			if (GKBase is GKZone)
			{
				StateBits.Add(new StateBitViewModel(this, GKStateBit.Attention));
				StateBits.Add(new StateBitViewModel(this, GKStateBit.Fire1));
				StateBits.Add(new StateBitViewModel(this, GKStateBit.Fire2));
			}

			if (GKBase is GKDirection)
			{
				StateBits.Add(new StateBitViewModel(this, GKStateBit.On));
				StateBits.Add(new StateBitViewModel(this, GKStateBit.Off));
				StateBits.Add(new StateBitViewModel(this, GKStateBit.TurningOn));
				StateBits.Add(new StateBitViewModel(this, GKStateBit.TurningOff));
			}
		}

		void InitializeFailureItems()
		{
			if (BaseDescriptor.GKBase is GKDevice)
			{
				Failures = new ObservableCollection<FailureViewModel>();
				Failures.Add(new FailureViewModel(this, JournalEventDescriptionType.Потеря_связи, 255));
			}
		}

		public RelayCommand SetAutomaticRegimeCommand { get; private set; }
		void OnSetAutomaticRegime()
		{
			StateBits.FirstOrDefault(x => x.StateBit == GKStateBit.Norm).IsActive = true;
			StateBits.FirstOrDefault(x => x.StateBit == GKStateBit.Ignore).IsActive = false;
			var journalItem = new ImitatorJournalItem();
			journalItem.Source = 2;
			journalItem.NameCode = 10;
			journalItem.DescriptionCode = 0;
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
			journalItem.NameCode = 10;
			journalItem.DescriptionCode = 1;
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
			journalItem.NameCode = 10;
			journalItem.DescriptionCode = 2;
			journalItem.ObjectNo = 0;
			AddJournalItem(journalItem);
		}

		public void SetParameters()
		{
			var journalItem = new ImitatorJournalItem();
			journalItem.Source = 2;
			journalItem.NameCode = 13;
			journalItem.DescriptionCode = 0;
			AddJournalItem(journalItem);
		}

		public void GetParameters()
		{
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
			journalItem.GkObjectNo = BaseDescriptor.GetDescriptorNo();
			journalItem.ObjectFactoryNo = 0;
			journalItem.ObjectState = state;
			if (BaseDescriptor.GKBase is GKDevice)
			{
				journalItem.ObjectDeviceType = (short)(BaseDescriptor.GKBase as GKDevice).Driver.DriverTypeNo;
				journalItem.ObjectDeviceAddress = (short)(((BaseDescriptor.GKBase as GKDevice).ShleifNo - 1) * 256 + (BaseDescriptor.GKBase as GKDevice).IntAddress);
			}
			JournalHelper.ImitatorJournalItemCollection.ImitatorJournalItems.Add(journalItem);
			JournalHelper.Save();
		}
	}
}