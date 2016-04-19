using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Threading;
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
		public int GKDescriptorNo { get; private set; }
		public int KauDescriptorNo { get; private set; }
		public ushort TypeNo { get; private set; }
		readonly List<ushort> AdditionalShortParameters;

		public DescriptorViewModel(BaseDescriptor descriptor)
		{
			GKBaseDescriptor = descriptor;
			GKDescriptorNo = descriptor.GKBase.GKDescriptorNo;
			KauDescriptorNo = descriptor.GKBase.KAUDescriptorNo;

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

		void OnStateBitChanged(GKStateBit stateBit, bool isActive, ImitatorJournalItem additionalJournalItem = null)
		{
			ImitatorJournalItem journalItem = null;
			if (stateBit == GKStateBit.Failure)
			{
				journalItem = new ImitatorJournalItem(2, 5, 255, (byte)(isActive ? 1 : 0));
				AddJournalItem(additionalJournalItem ?? journalItem);
			}
			else if (isActive)
			{
				if (stateBit != GKStateBit.Norm)
				{
					CurrentOnDelay = 0;
					CurrentOffDelay = 0;
					CurrentHoldDelay = 0;
					TurningState = TurningState.None;
				}
				if (stateBit == GKStateBit.On)
				{
					journalItem = new ImitatorJournalItem(2, 9, 2, 0);
					if (HoldDelay != 0 && !(GKBase is GKDoor && Regime == Regime.Manual))
					{
						CurrentHoldDelay = HoldDelay;
						TurningState = TurningState.Holding;
					}
				}

				if (stateBit == GKStateBit.TurningOn)
				{
					journalItem = new ImitatorJournalItem(2, 9, 4, 0);
					if (OnDelay != 0)
					{
						CurrentOnDelay = OnDelay;
						TurningState = TurningState.TurningOn;
					}
				}

				if (stateBit == GKStateBit.TurningOff)
				{
					journalItem = new ImitatorJournalItem(2, 9, 5, 0);
					if (OffDelay != 0)
					{
						CurrentOffDelay = OffDelay;
						TurningState = TurningState.TurningOff;
						SetStateBit(GKStateBit.Fire1, false);
						SetStateBit(GKStateBit.Fire2, false);
					}
				}

				if (stateBit == GKStateBit.Off)
				{
					AdditionalShortParameters[0] = 0;
					AdditionalShortParameters[1] = 0;
					AdditionalShortParameters[2] = 0;
					journalItem = new ImitatorJournalItem(2, 9, 3, 3);
					SetStateBit(GKStateBit.Attention, false);
					SetStateBit(GKStateBit.Fire1, false);
					SetStateBit(GKStateBit.Fire2, false);
				}

				if (stateBit == GKStateBit.Norm)
				{
					journalItem = new ImitatorJournalItem(2, 14, 0, 0);
				}

				if (stateBit == GKStateBit.Fire1)
				{
					journalItem = new ImitatorJournalItem(2, 2, 0, 0);
				}

				if (stateBit == GKStateBit.Fire2)
				{
					journalItem = new ImitatorJournalItem(2, 3, 0, 0);
				}

				if (stateBit == GKStateBit.Ignore)
				{
					journalItem = new ImitatorJournalItem(2, 10, 2, 0);
				}

				if (stateBit == GKStateBit.Attention)
				{
					journalItem = new ImitatorJournalItem(2, 4, 0, 0);
				}

				AddJournalItem(additionalJournalItem ?? journalItem);
			}

			if (additionalJournalItem != null || journalItem != null)
				RecalculateOutputLogic();
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
			var journalItem = new ImitatorJournalItem(2, 10, 0, 0);
			SetStateBit(GKStateBit.Ignore, false);
			SetStateBit(GKStateBit.Norm, true, journalItem);
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
			SetStateBit(GKStateBit.Attention, false);
			SetStateBit(GKStateBit.On, false);
			SetStateBit(GKStateBit.Fire1, false);
			SetStateBit(GKStateBit.Fire2, false);
			SetStateBit(GKStateBit.Ignore, true);
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

		public uint CurrentCardNo { get; set; }

		public bool HasCard { get; private set; }
		public List<byte> GetStateBytes(int no, DatabaseType databaseType)
		{
			lock (locker)
			{
				var result = new List<byte>();

				result.AddRange(ToBytes((short)TypeNo));

				if (databaseType == DatabaseType.Gk)
				{
					var controllerAddress = GKBaseDescriptor.ControllerAdress;
					result.AddRange(ToBytes((short)controllerAddress));

					var addressOnController = GKBaseDescriptor.AdressOnController;
					result.AddRange(ToBytes((short)addressOnController));
				}

				var physicalAddress = GKBaseDescriptor.PhysicalAdress;
				result.AddRange(ToBytes((short)physicalAddress));

				if (databaseType == DatabaseType.Gk)
				{
					result.AddRange(GKBaseDescriptor.Description);
				}

				var serialNo = 0;
				result.AddRange(IntToBytes(serialNo));

				result.AddRange(IntToBytes(StatesToInt()));

				foreach (var additionalShortParameter in AdditionalShortParameters)
				{
					result.AddRange(ShortToBytes(additionalShortParameter));
				}
				
				if (databaseType == DatabaseType.Gk && HasCard)
				{
					result.RemoveRange(52, 4);
					result.InsertRange(52, UIntToBytes(CurrentCardNo));
				}

				return result;
			}
		}

		List<byte> ToBytes(short shortValue)
		{
			return BitConverter.GetBytes(shortValue).ToList();
		}

		List<byte> IntToBytes(int intValue)
		{
			return BitConverter.GetBytes(intValue).ToList();
		}
		
		List<byte> UIntToBytes(uint uintValue)
		{
			return BitConverter.GetBytes(uintValue).ToList();
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
			using (var dbService = new DbService())
			{
				dbService.ImitatorJournalTranslator.Add(journalItem);
			}
			JournalCash.Add(journalItem);
		}

		public void EnterCard(uint cardNo, GKCodeReaderEnterType enterType)
		{
			CurrentCardNo = cardNo;
			var backgroundWorker = new BackgroundWorker();
			backgroundWorker.DoWork += backgroundWorker_DoWork;
			backgroundWorker.RunWorkerAsync();
			switch (enterType)
			{
				case GKCodeReaderEnterType.CodeOnly:
					SetStateBit(GKStateBit.Fire1, false);
					SetStateBit(GKStateBit.Fire2, false);
					SetStateBit(GKStateBit.Attention, true);
					break;

				case GKCodeReaderEnterType.CodeAndOne:
					SetStateBit(GKStateBit.Attention, false);
					SetStateBit(GKStateBit.Fire2, false);
					SetStateBit(GKStateBit.Fire1, true);
					break;

				case GKCodeReaderEnterType.CodeAndTwo:
					SetStateBit(GKStateBit.Attention, false);
					SetStateBit(GKStateBit.Fire1, false);
					SetStateBit(GKStateBit.Fire2, true);
					break;
			}
		}

		void backgroundWorker_DoWork(object sender, DoWorkEventArgs e)
		{
			Thread.Sleep(TimeSpan.FromSeconds(3));
			var journalItem = new ImitatorJournalItem(2, 14, 0, 0);
			SetStateBit(GKStateBit.Attention, false);
			SetStateBit(GKStateBit.Fire1, false);
			SetStateBit(GKStateBit.Fire2, false);
			AddJournalItem(journalItem);
			CurrentCardNo = 0;
		}
	}
}