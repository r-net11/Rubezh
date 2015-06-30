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
		public bool HasOnDelay { get; private set; }
		public bool HasHoldDelay { get; private set; }
		public bool HasOffDelay { get; private set; }

		ushort OnDelay { get; set; }
		ushort HoldDelay { get; set; }
		ushort OffDelay { get; set; }
		DelayRegime? DelayRegime { get; set; }

		void InitializeDelays()
		{
			var device = GKBase as GKDevice;
			if (device != null)
			{
				var property = device.Properties.FirstOrDefault(x => x.Name == "Задержка на включение, с");
				if (property != null)
				{
					OnDelay = property.Value;
				}
				property = device.Properties.FirstOrDefault(x => x.Name == "Время удержания, с");
				if (property != null)
				{
					HoldDelay = property.Value;
				}
				property = device.Properties.FirstOrDefault(x => x.Name == "Задержка на выключение, с");
				if (property != null)
				{
					OffDelay = property.Value;
				}
			}
			var direction = GKBase as GKDirection;
			if (direction != null)
			{
				OnDelay = direction.Delay;
				HoldDelay = direction.Hold;
				DelayRegime = direction.DelayRegime;
			}
			var mpt = GKBase as GKMPT;
			if (mpt != null)
			{
				OnDelay = (ushort)mpt.Delay;
				HoldDelay = (ushort)mpt.Hold;
				DelayRegime = mpt.DelayRegime;
			}
			var pumpStation = GKBase as GKPumpStation;
			if (pumpStation != null)
			{
				OnDelay = (ushort)pumpStation.Delay;
				HoldDelay = (ushort)pumpStation.Hold;
				DelayRegime = pumpStation.DelayRegime;
			}
			var delay = GKBase as GKDelay;
			if (delay != null)
			{
				OnDelay = delay.DelayTime;
				HoldDelay = delay.Hold;
				DelayRegime = delay.DelayRegime;
			}
		}

		TurningState TurningState = TurningState.None;

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
			PauseTurnOnCommand = new RelayCommand(OnPauseTurnOn);
		}

		public bool HasTurnOn { get; private set; }
		public bool HasTurnOnNow { get; private set; }
		public bool HasTurnOff { get; private set; }
		public bool HasTurnOffNow { get; private set; }
		public bool HasPauseTurnOn { get; private set; }

		public void CheckDelays()
		{
			if (TurningState == TurningState.TurningOn)
			{
				if (CurrentOnDelay == 0)
				{
					TurningState = TurningState.None;
					StateBits.FirstOrDefault(x => x.StateBit == GKStateBit.On).IsActive = true;
					StateBits.FirstOrDefault(x => x.StateBit == GKStateBit.TurningOn).IsActive = false;
					StateBits.FirstOrDefault(x => x.StateBit == GKStateBit.Off).IsActive = false;
					StateBits.FirstOrDefault(x => x.StateBit == GKStateBit.TurningOff).IsActive = false;
					var journalItem = new ImitatorJournalItem(2, 9, 2, 0);
					AddJournalItem(journalItem);

					if (HoldDelay > 0)
					{
						CurrentHoldDelay = HoldDelay;
						TurningState = TurningState.Holding;
					}
				}
				else
				{
					CurrentOnDelay--;
					AdditionalShortParameters[0] = CurrentOnDelay;
				}
			}
			if (TurningState == TurningState.Holding)
			{
				if (CurrentHoldDelay == 0)
				{
					TurningState = TurningState.None;
					if (DelayRegime != null)
					{
						if (DelayRegime.Value == FiresecAPI.GK.DelayRegime.Off)
						{
							TurnOffNow();
						}
					}
					else
					{
						if (OffDelay > 0)
						{
							CurrentOffDelay = OffDelay;
							TurningState = TurningState.TurningOff;
							TurnOff();
						}
					}
				}
				else
				{
					CurrentHoldDelay--;
					AdditionalShortParameters[1] = CurrentHoldDelay;
				}
			}
			if (TurningState == TurningState.TurningOff)
			{
				if (CurrentOffDelay == 0)
				{
					TurningState = TurningState.None;
					TurnOffNow();
				}
				else
				{
					CurrentOffDelay--;
					AdditionalShortParameters[2] = CurrentOffDelay;
				}
			}
		}

		public RelayCommand TurnOnCommand { get; private set; }
		void OnTurnOn()
		{
			if (HasTurnOn)
			{
				if (OnDelay > 0)
				{
					if (TurningState != TurningState.Paused)
					{
						CurrentOnDelay = OnDelay;
					}
					TurningState = TurningState.TurningOn;
				}
				TurnOn();
			}
		}

		public RelayCommand TurnOnNowCommand { get; private set; }
		void OnTurnOnNow()
		{
			if (HasTurnOnNow)
			{
				TurnOnNow();
			}
		}

		public RelayCommand TurnOffCommand { get; private set; }
		void OnTurnOff()
		{
			if (HasTurnOff)
			{
				if (OffDelay > 0)
				{
					CurrentOffDelay = OffDelay;
				}
				TurningState = TurningState.None;
				TurnOff();
			}
		}

		public RelayCommand TurnOffNowCommand { get; private set; }
		void OnTurnOffNow()
		{
			if (HasTurnOffNow)
			{
				TurnOffNow();
			}
		}

		public RelayCommand PauseTurnOnCommand { get; private set; }
		void OnPauseTurnOn()
		{
			if (HasPauseTurnOn)
			{
				if(TurningState == TurningState.TurningOn)
				{
					TurningState = TurningState.Paused;
				}
			}
		}

		void TurnOn()
		{
			if (OnDelay == 0)
			{
				TurnOnNow();
			}
			else
			{
				StateBits.FirstOrDefault(x => x.StateBit == GKStateBit.On).IsActive = false;
				StateBits.FirstOrDefault(x => x.StateBit == GKStateBit.TurningOn).IsActive = true;
				StateBits.FirstOrDefault(x => x.StateBit == GKStateBit.Off).IsActive = false;
				StateBits.FirstOrDefault(x => x.StateBit == GKStateBit.TurningOff).IsActive = false;
				var journalItem = new ImitatorJournalItem(2, 9, 4, 0);
				AddJournalItem(journalItem);
			}
		}

		void TurnOnNow()
		{
			CurrentOnDelay = 0;
			CurrentHoldDelay = 0;
			CurrentOffDelay = 0;
			StateBits.FirstOrDefault(x => x.StateBit == GKStateBit.On).IsActive = true;
			StateBits.FirstOrDefault(x => x.StateBit == GKStateBit.TurningOn).IsActive = false;
			StateBits.FirstOrDefault(x => x.StateBit == GKStateBit.Off).IsActive = false;
			StateBits.FirstOrDefault(x => x.StateBit == GKStateBit.TurningOff).IsActive = false;
			var journalItem = new ImitatorJournalItem(2, 9, 3, 3);
			AddJournalItem(journalItem);
		}

		void TurnOff()
		{
			if (OffDelay == 0)
			{
				TurnOffNow();
			}
			else
			{
				StateBits.FirstOrDefault(x => x.StateBit == GKStateBit.On).IsActive = false;
				StateBits.FirstOrDefault(x => x.StateBit == GKStateBit.TurningOn).IsActive = false;
				StateBits.FirstOrDefault(x => x.StateBit == GKStateBit.Off).IsActive = false;
				StateBits.FirstOrDefault(x => x.StateBit == GKStateBit.TurningOff).IsActive = true;
				var journalItem = new ImitatorJournalItem(2, 9, 5, 3);
				AddJournalItem(journalItem);
			}
		}

		void TurnOffNow()
		{
			CurrentOnDelay = 0;
			CurrentHoldDelay = 0;
			CurrentOffDelay = 0;
			StateBits.FirstOrDefault(x => x.StateBit == GKStateBit.On).IsActive = false;
			StateBits.FirstOrDefault(x => x.StateBit == GKStateBit.TurningOn).IsActive = false;
			StateBits.FirstOrDefault(x => x.StateBit == GKStateBit.Off).IsActive = true;
			StateBits.FirstOrDefault(x => x.StateBit == GKStateBit.TurningOff).IsActive = false;
			var journalItem = new ImitatorJournalItem(2, 9, 3, 3);
			AddJournalItem(journalItem);
		}
	}
}