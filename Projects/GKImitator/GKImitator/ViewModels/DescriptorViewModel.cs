using System;
using System.Collections.ObjectModel;
using System.Linq;
using Infrastructure.Common.Windows.ViewModels;
using RubezhAPI.GK;
using GKImitator.Processor;
using GKProcessor;
using Infrastructure.Common;
using System.Collections.Generic;
using Infrastructure.Common.Windows;
using RubezhDAL.DataClasses;

namespace GKImitator.ViewModels
{
    public partial class DescriptorViewModel : BaseViewModel
	{
		public BaseDescriptor GKBaseDescriptor { get; private set; }
		public BaseDescriptor KauBaseDescriptor { get; private set; }
		public GKBase GKBase { get { return GKBaseDescriptor.GKBase; } }
		public int DescriptorNo { get; private set; }
		public ushort TypeNo { get; private set; }
		readonly List<ushort> AdditionalShortParameters;

		public DescriptorViewModel(BaseDescriptor descriptor)
		{
			GKBaseDescriptor = descriptor;
			DescriptorNo = descriptor.GetDescriptorNo();

			SetAutomaticRegimeCommand = new RelayCommand(OnSetAutomaticRegime);
			SetManualRegimeCommand = new RelayCommand(OnSetManualRegime);
			SetIgnoreRegimeCommand = new RelayCommand(OnSetIgnoreRegime);
			ShowParametersCommand = new RelayCommand(OnShowParameters);
			ShowMeasureCommand = new RelayCommand(OnShowMeasure);
			ShowCardReaderCommand = new RelayCommand(OnShowCardReader);

			InitializeTest();
			InitializeDustiness();
			InitializeController();
			InitializeTypeNo();
			InitializeFire();
			InitializeTurning();
			InitializeLogic();
			InitializeDelays();
			InitializeAll();

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

		public bool HasAutomaticRegime { get; private set; }
		public bool HasManualRegime { get; private set; }
		public bool HasIgnoreRegime { get; private set; }

		public RelayCommand SetAutomaticRegimeCommand { get; private set; }
		void OnSetAutomaticRegime()
		{
			Regime = Regime.Automatic;
			SetStateBit(GKStateBit.Norm, true);
			SetStateBit(GKStateBit.Ignore, false);
			var journalItem = new ImitatorJournalItem(2, 10, 0, 0);
			AddJournalItem(journalItem);
			RecalculateOutputLogic();
			RecalculateCurrentLogic();
		}

		public bool CanSetAutomaticRegime
		{
			get { return Regime != Regime.Automatic; }
		}

		public RelayCommand SetManualRegimeCommand { get; private set; }
		void OnSetManualRegime()
		{
			Regime = Regime.Manual;
			SetStateBit(GKStateBit.Norm, false);
			SetStateBit(GKStateBit.Ignore, false);
			var journalItem = new ImitatorJournalItem(2, 10, 1, 0);
			AddJournalItem(journalItem);
			RecalculateOutputLogic();
		}

		public bool CanSetManualRegime
		{
			get { return Regime != Regime.Manual; }
		}

		public RelayCommand SetIgnoreRegimeCommand { get; private set; }
		void OnSetIgnoreRegime()
		{
			Regime = Regime.Ignore;
			SetStateBit(GKStateBit.Norm, false);
			SetStateBit(GKStateBit.Ignore, true);
			SetStateBit(GKStateBit.Attention, false);
			SetStateBit(GKStateBit.Fire1, false);
			SetStateBit(GKStateBit.Fire2, false);
			var journalItem = new ImitatorJournalItem(2, 10, 2, 0);
			AddJournalItem(journalItem);
			RecalculateOutputLogic();
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

		public RelayCommand ShowCardReaderCommand { get; private set; }
		void OnShowCardReader()
		{
			var cardReaderViewModel = new CardReaderViewModel(this);
			DialogService.ShowModalWindow(cardReaderViewModel);
		}

		public int CurrentCardNo { get; set; }

		public bool HasCard { get; private set; }

		public List<byte> GetStateBytes(int no)
		{
			var result = new List<byte>();

			result.AddRange(ToBytes((short)TypeNo));

			var controllerAddress = GKBaseDescriptor.ControllerAdress;
			result.AddRange(ToBytes((short)controllerAddress));

			var addressOnController = GKBaseDescriptor.AdressOnController;
			result.AddRange(ToBytes((short)addressOnController));

			var physicalAddress = GKBaseDescriptor.PhysicalAdress;
			result.AddRange(ToBytes((short)physicalAddress));

			result.AddRange(GKBaseDescriptor.Description);

			var serialNo = 0;
			result.AddRange(IntToBytes(serialNo));

			result.AddRange(IntToBytes(StatesToInt()));

			foreach (var additionalShortParameter in AdditionalShortParameters)
			{
				result.AddRange(ShortToBytes(additionalShortParameter));
			}

			if(HasCard)
			{
				result.RemoveRange(52, 4);
				result.InsertRange(52, IntToBytes(CurrentCardNo));
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
			journalItem.UNUSED_KauNo = 0;
			journalItem.UNUSED_KauAddress = 0;
			journalItem.GkObjectNo = GKBaseDescriptor.GetDescriptorNo();
			journalItem.ObjectState = StatesToInt();
			if (GKBaseDescriptor.GKBase is GKDevice)
			{
				journalItem.ObjectDeviceType = (short)(GKBaseDescriptor.GKBase as GKDevice).Driver.DriverTypeNo;
				journalItem.ObjectDeviceAddress = (short)(((GKBaseDescriptor.GKBase as GKDevice).ShleifNo - 1) * 256 + (GKBaseDescriptor.GKBase as GKDevice).IntAddress);
			}
			using(var dbService = new DbService())
			{
				dbService.ImitatorJournalTranslator.Add(journalItem);
			}
			JournalCash.Add(journalItem);
		}
	}
}