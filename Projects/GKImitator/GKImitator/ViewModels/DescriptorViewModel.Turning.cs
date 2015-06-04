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
		bool IsTurningOn;
		bool IsHolding;
		bool IsTurningOff;

		ushort _currentOnDelay;
		public ushort CurrentOnDelay
		{
			get { return _currentOnDelay; }
			set
			{
				_currentOnDelay = value;
				OnPropertyChanged(() => CurrentOnDelay);
			}
		}

		ushort _currentHoldDelay;
		public ushort CurrentHoldDelay
		{
			get { return _currentHoldDelay; }
			set
			{
				_currentHoldDelay = value;
				OnPropertyChanged(() => CurrentHoldDelay);
			}
		}

		ushort _currentOffDelay;
		public ushort CurrentOffDelay
		{
			get { return _currentOffDelay; }
			set
			{
				_currentOffDelay = value;
				OnPropertyChanged(() => CurrentOffDelay);
			}
		}

		public void InitializeTurning()
		{
			TurnOnCommand = new RelayCommand(OnTurnOn);
			TurnOnNowCommand = new RelayCommand(OnTurnOnNow);
			TurnOffCommand = new RelayCommand(OnTurnOff);
			TurnOffNowCommand = new RelayCommand(OnTurnOffNow);

			if (GKBase is GKDevice)
			{
				var device = GKBase as GKDevice;
				switch(device.DriverType)
				{
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
						HasTurnOn = true;
						HasTurnOnNow = true;
						HasTurnOff = true;
						HasTurnOffNow = true;
						StateBits.Add(new StateBitViewModel(this, GKStateBit.On));
						StateBits.Add(new StateBitViewModel(this, GKStateBit.TurningOn));
						StateBits.Add(new StateBitViewModel(this, GKStateBit.Off));
						StateBits.Add(new StateBitViewModel(this, GKStateBit.TurningOff));
						break;
				}
			}
		}

		public bool HasTurnOn { get; private set; }
		public bool HasTurnOnNow { get; private set; }
		public bool HasTurnOff { get; private set; }
		public bool HasTurnOffNow { get; private set; }

		public void CheckDelays()
		{
			if (IsTurningOn)
			{
				CurrentOnDelay--;
				AdditionalShortParameters[0] = CurrentOnDelay;
				if (CurrentOnDelay == 0)
				{
					IsTurningOn = false;
					StateBits.FirstOrDefault(x => x.StateBit == GKStateBit.On).IsActive = true;
					StateBits.FirstOrDefault(x => x.StateBit == GKStateBit.TurningOn).IsActive = false;
					StateBits.FirstOrDefault(x => x.StateBit == GKStateBit.Off).IsActive = false;
					StateBits.FirstOrDefault(x => x.StateBit == GKStateBit.TurningOff).IsActive = false;
					var journalItem = new ImitatorJournalItem(2, 9, 2, 0);
					AddJournalItem(journalItem);
				}
			}
			if (IsTurningOn)
			{
			}
			if (IsTurningOff)
			{
			}
		}

		public RelayCommand TurnOnCommand { get; private set; }
		void OnTurnOn()
		{
			if (GKBase is GKDevice)
			{
				var device = GKBase as GKDevice;
				var property = device.Properties.FirstOrDefault(x => x.Name == "Задержка на включение, с");
				if (property != null)
				{
					CurrentOnDelay = property.Value;
					IsTurningOn = true;
				}
			}
			StateBits.FirstOrDefault(x => x.StateBit == GKStateBit.On).IsActive = false;
			StateBits.FirstOrDefault(x => x.StateBit == GKStateBit.TurningOn).IsActive = true;
			StateBits.FirstOrDefault(x => x.StateBit == GKStateBit.Off).IsActive = false;
			StateBits.FirstOrDefault(x => x.StateBit == GKStateBit.TurningOff).IsActive = false;
			var journalItem = new ImitatorJournalItem(2, 9, 4, 0);
			AddJournalItem(journalItem);
		}

		public RelayCommand TurnOnNowCommand { get; private set; }
		void OnTurnOnNow()
		{
			StateBits.FirstOrDefault(x => x.StateBit == GKStateBit.On).IsActive = true;
			StateBits.FirstOrDefault(x => x.StateBit == GKStateBit.TurningOn).IsActive = false;
			StateBits.FirstOrDefault(x => x.StateBit == GKStateBit.Off).IsActive = false;
			StateBits.FirstOrDefault(x => x.StateBit == GKStateBit.TurningOff).IsActive = false;
			var journalItem = new ImitatorJournalItem(2, 9, 2, 0);
			AddJournalItem(journalItem);
		}

		public RelayCommand TurnOffCommand { get; private set; }
		void OnTurnOff()
		{
			if (GKBase is GKDevice)
			{
				var device = GKBase as GKDevice;
				var property = device.Properties.FirstOrDefault(x => x.Name == "Задержка на выключение, с");
				if (property != null)
				{
					CurrentOffDelay = property.Value;
					IsTurningOff = true;
				}
			}
			StateBits.FirstOrDefault(x => x.StateBit == GKStateBit.On).IsActive = false;
			StateBits.FirstOrDefault(x => x.StateBit == GKStateBit.TurningOn).IsActive = false;
			StateBits.FirstOrDefault(x => x.StateBit == GKStateBit.Off).IsActive = false;
			StateBits.FirstOrDefault(x => x.StateBit == GKStateBit.TurningOff).IsActive = true;
			var journalItem = new ImitatorJournalItem(2, 9, 5, 3);
			AddJournalItem(journalItem);
		}

		public RelayCommand TurnOffNowCommand { get; private set; }
		void OnTurnOffNow()
		{
			StateBits.FirstOrDefault(x => x.StateBit == GKStateBit.On).IsActive = false;
			StateBits.FirstOrDefault(x => x.StateBit == GKStateBit.TurningOn).IsActive = false;
			StateBits.FirstOrDefault(x => x.StateBit == GKStateBit.Off).IsActive = true;
			StateBits.FirstOrDefault(x => x.StateBit == GKStateBit.TurningOff).IsActive = false;
			var journalItem = new ImitatorJournalItem(2, 9, 3, 3);
			AddJournalItem(journalItem);
		}
	}
}