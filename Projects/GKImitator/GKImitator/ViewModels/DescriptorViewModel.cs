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
using System.Collections.Generic;
using System.Windows.Input;

namespace GKImitator.ViewModels
{
	public partial class DescriptorViewModel : BaseViewModel
	{
		public BaseDescriptor BaseDescriptor { get; private set; }
		public GKBase GKBase { get { return BaseDescriptor.GKBase; } }
		public int DescriptorNo { get; private set; }
		public ushort TypeNo { get; private set; }
		List<ushort> AdditionalShortParameters;

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
			InitializeTypeNo();
			InitializeFire();
			InitializeTurning();
			InitializeLogic();
			InitializeDelays();

			AdditionalShortParameters = new List<ushort>();
			for (int i = 0; i < 10; i++)
			{
				AdditionalShortParameters.Add(0);
			}
		}

		void InitializeTypeNo()
		{
			TypeNo = 0;
			if (GKBase is GKDevice)
				TypeNo = (GKBase as GKDevice).Driver.DriverTypeNo;
			if (GKBase is GKZone)
				TypeNo = 0x100;
			if (GKBase is GKDirection)
				TypeNo = 0x106;
			if (GKBase is GKPumpStation)
				TypeNo = 0x106;
			if (GKBase is GKMPT)
				TypeNo = 0x106;
			if (GKBase is GKDelay)
				TypeNo = 0x101;
			if (GKBase is GKPim)
				TypeNo = 0x107;
			if (GKBase is GKGuardZone)
				TypeNo = 0x108;
			if (GKBase is GKCode)
				TypeNo = 0x109;
			if (GKBase is GKDoor)
				TypeNo = 0x104;
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
						break;

					case GKDriverType.RSR2_AM_1:
					case GKDriverType.RSR2_MAP4:
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
						//StateBits.Add(new StateBitViewModel(this, GKStateBit.On));
						//StateBits.Add(new StateBitViewModel(this, GKStateBit.Off));
						//StateBits.Add(new StateBitViewModel(this, GKStateBit.TurningOn));
						//StateBits.Add(new StateBitViewModel(this, GKStateBit.TurningOff));
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
			Regime = Regime.Automatic;
			StateBits.FirstOrDefault(x => x.StateBit == GKStateBit.Norm).IsActive = true;
			StateBits.FirstOrDefault(x => x.StateBit == GKStateBit.Ignore).IsActive = false;
			var journalItem = new ImitatorJournalItem(2, 10, 0, 0);
			AddJournalItem(journalItem);
		}

		public bool CanSetAutomaticRegime
		{
			get { return Regime != Regime.Automatic; }
		}

		public RelayCommand SetManualRegimeCommand { get; private set; }
		void OnSetManualRegime()
		{
			Regime = Regime.Manual;
			StateBits.FirstOrDefault(x => x.StateBit == GKStateBit.Norm).IsActive = false;
			StateBits.FirstOrDefault(x => x.StateBit == GKStateBit.Ignore).IsActive = false;
			var journalItem = new ImitatorJournalItem(2, 10, 1, 0);
			AddJournalItem(journalItem);
		}

		public bool CanSetManualRegime
		{
			get { return Regime != Regime.Manual; }
		}

		public RelayCommand SetIgnoreRegimeCommand { get; private set; }
		void OnSetIgnoreRegime()
		{
			Regime = Regime.Ignore;
			StateBits.FirstOrDefault(x => x.StateBit == GKStateBit.Norm).IsActive = false;
			StateBits.FirstOrDefault(x => x.StateBit == GKStateBit.Ignore).IsActive = true;
			var journalItem = new ImitatorJournalItem(2, 10, 2, 0);
			AddJournalItem(journalItem);
		}

		public bool CanSetIgnoreRegime
		{
			get { return Regime != Regime.Ignore; }
		}

		Regime _regime;
		public Regime Regime
		{
			get { return _regime; }
			set
			{
				_regime = value;
				OnPropertyChanged(() => Regime);
				OnPropertyChanged(() => CanSetAutomaticRegime);
				OnPropertyChanged(() => CanSetManualRegime);
				OnPropertyChanged(() => CanSetIgnoreRegime);
				CanControl = value == Regime.Manual;
			}
		}

		bool _canControl;
		public bool CanControl
		{
			get { return _canControl; }
			set
			{
				_canControl = value;
				OnPropertyChanged(() => CanControl);
			}
		}

		public void SetParameters()
		{
			var journalItem = new ImitatorJournalItem(2, 13, 0, 0);
			AddJournalItem(journalItem);
		}

		public void GetParameters()
		{
		}

		public List<byte> GetStateBytes(int no)
		{
			var result = new List<byte>();

			result.AddRange(ToBytes((short)TypeNo));

			var controllerAddress = BaseDescriptor.ControllerAdress;
			result.AddRange(ToBytes((short)controllerAddress));

			var addressOnController = BaseDescriptor.AdressOnController;
			result.AddRange(ToBytes((short)addressOnController));

			var physicalAddress = BaseDescriptor.PhysicalAdress;
			result.AddRange(ToBytes((short)physicalAddress));

			result.AddRange(BaseDescriptor.Description);

			var serialNo = 0;
			result.AddRange(IntToBytes((int)serialNo));

			var state = 0;
			foreach (var stateBitViewModel in StateBits)
			{
				if (stateBitViewModel.IsActive)
				{
					state += (1 << (int)stateBitViewModel.StateBit);
				}
			}
			result.AddRange(IntToBytes((int)state));

			foreach (var additionalShortParameter in AdditionalShortParameters)
			{
				result.AddRange(ShortToBytes(additionalShortParameter));
			}

			return result;
		}

		List<byte> ToBytes(short shortValue)
		{
			return BitConverter.GetBytes(shortValue).ToList();
		}

		List<byte> IntToBytes(int intValue)
		{
			return BitConverter.GetBytes(intValue).ToList();
		}

		List<byte> ShortToBytes(ushort shortValue)
		{
			return BitConverter.GetBytes(shortValue).ToList();
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
			journalItem.GkNo = DBHelper.ImitatorJournalItemCollection.ImitatorJournalItems.Count + 1;
			journalItem.GkObjectNo = BaseDescriptor.GetDescriptorNo();
			journalItem.ObjectFactoryNo = 0;
			journalItem.ObjectState = state;
			if (BaseDescriptor.GKBase is GKDevice)
			{
				journalItem.ObjectDeviceType = (short)(BaseDescriptor.GKBase as GKDevice).Driver.DriverTypeNo;
				journalItem.ObjectDeviceAddress = (short)(((BaseDescriptor.GKBase as GKDevice).ShleifNo - 1) * 256 + (BaseDescriptor.GKBase as GKDevice).IntAddress);
			}
			DBHelper.ImitatorJournalItemCollection.ImitatorJournalItems.Add(journalItem);
			DBHelper.Save();
		}
	}
}