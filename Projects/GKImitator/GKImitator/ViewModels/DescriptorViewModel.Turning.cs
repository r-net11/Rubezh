using System.Linq;
using RubezhAPI.GK;
using GKImitator.Processor;
using Infrastructure.Common;
using RubezhDAL.DataClasses;

namespace GKImitator.ViewModels
{
	public partial class DescriptorViewModel
	{
		public bool HasOnDelay { get; private set; }
		public bool HasHoldDelay { get; private set; }
		public bool HasOffDelay { get; private set; }
		bool IsSettingGuardAlarm { get; set; }

		ushort OnDelay { get; set; }
		ushort HoldDelay { get; set; }
		ushort HoldOffDelay { get; set; }
		ushort OffDelay { get; set; }
		ushort GuardZoneAlarmDelay { get; set; }
		DelayRegime? DelayRegime { get; set; }

		public void InitializeDelays()
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
				else
				{
					property = device.Properties.FirstOrDefault(x => x.Name == "Время удержания на включение, с");
					if (property != null)
						HoldDelay = property.Value;
					property = device.Properties.FirstOrDefault(x => x.Name == "Время удержания на выключение, с");
					if (property != null)
						HoldOffDelay = property.Value;
				}
				property = device.Properties.FirstOrDefault(x => x.Name == "Задержка на выключение, с");
				if (property != null)
				{
					OffDelay = property.Value;
				}
				property = device.Properties.FirstOrDefault(x => x.Name == "Режим после удержания включенного состояния");
				if (property != null)
					DelayRegime = property.Value == 0 ? RubezhAPI.GK.DelayRegime.Off: RubezhAPI.GK.DelayRegime.On;

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
			}
			var pumpStation = GKBase as GKPumpStation;
			if (pumpStation != null)
			{
				OnDelay = pumpStation.Delay;
				HoldDelay = pumpStation.Hold;
				DelayRegime = pumpStation.DelayRegime;
			}
			var delay = GKBase as GKDelay;
			if (delay != null)
			{
				OnDelay = delay.DelayTime;
				HoldDelay = delay.Hold;
				DelayRegime = delay.DelayRegime;
			}

			var door = GKBase as GKDoor;
			if (door != null)
			{
				OffDelay = (ushort)door.Delay;
				HoldDelay = (ushort)door.Hold;
				DelayRegime = RubezhAPI.GK.DelayRegime.Off;
			}


			var guardZone = GKBase as GKGuardZone;
			if (guardZone != null)
			{
				OnDelay = (ushort)guardZone.SetDelay;
				OffDelay = (ushort)guardZone.ResetDelay;
				GuardZoneAlarmDelay = (ushort)guardZone.AlarmDelay;
				DelayRegime = RubezhAPI.GK.DelayRegime.On;
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

		ushort _currentAlarmDelay;
		public ushort CurrentAlarmDelay
		{
			get { return _currentAlarmDelay; }
			set
			{
				_currentAlarmDelay = value;
				OnPropertyChanged(() => CurrentAlarmDelay);
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
				    bool changed = SetStateBit(GKStateBit.On, true);
					changed = SetStateBit(GKStateBit.TurningOn, false) || changed;
					changed = SetStateBit(GKStateBit.Off, false) || changed;
					changed = SetStateBit(GKStateBit.TurningOff, false) || changed;
					if (changed)
					{
						var journalItem = new ImitatorJournalItem(2, 9, 2, 0);
						AddJournalItem(journalItem);
						RecalculateOutputLogic();
					}

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
						if (DelayRegime.Value == RubezhAPI.GK.DelayRegime.Off)
						{
							TurnOffNow();
						}
						if (DelayRegime.Value == RubezhAPI.GK.DelayRegime.On)
						{
							TurnOnNow();
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
				    var changed = SetStateBit(GKStateBit.On, false);
					changed = SetStateBit(GKStateBit.TurningOn, false) || changed;
					changed = SetStateBit(GKStateBit.Off, true) || changed;
					changed = SetStateBit(GKStateBit.TurningOff, false) || changed;
					if (changed)
					{
						var journalItem = new ImitatorJournalItem(2, 9, 3, 3);
						AddJournalItem(journalItem);
						RecalculateOutputLogic();
					}

					if (HoldOffDelay > 0)
					{
						CurrentHoldDelay = HoldOffDelay;
						TurningState = TurningState.Holding;
					}
				}
				else
				{
					CurrentOffDelay--;
					AdditionalShortParameters[2] = CurrentOffDelay;
				}
			}
			if (IsSettingGuardAlarm)
			{
				if (CurrentAlarmDelay == 0)
				{
					IsSettingGuardAlarm = false;
					SetStateBit(GKStateBit.Attention, false);
					SetStateBit(GKStateBit.Fire1, true);
					var journalItem = new ImitatorJournalItem(2, 2, 0, 0);
					AddJournalItem(journalItem);
					RecalculateOutputLogic();
				}
				else
				{
					CurrentAlarmDelay--;
				}
			}
		}

		public RelayCommand TurnOnCommand { get; private set; }
		void OnTurnOn()
		{
			var onState = StateBits.FirstOrDefault(x => x.StateBit == GKStateBit.On);
			var turningOnState = StateBits.FirstOrDefault(x => x.StateBit == GKStateBit.TurningOn);
			if (HasTurnOn && onState != null && !onState.IsActive && (turningOnState == null || !turningOnState.IsActive))
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
			var onState = StateBits.FirstOrDefault(x => x.StateBit == GKStateBit.On);
			var turningOnState = StateBits.FirstOrDefault(x => x.StateBit == GKStateBit.TurningOn);
			if (HasTurnOnNow && onState != null && !onState.IsActive && (turningOnState == null || !turningOnState.IsActive))
			{
				TurnOnNow();
			}
            if (HoldDelay > 0)
            {
                TurningState = TurningState.Holding;
                CurrentHoldDelay = HoldDelay;
            }
		}

		public RelayCommand TurnOffCommand { get; private set; }
		void OnTurnOff()
		{
			var offState = StateBits.FirstOrDefault(x => x.StateBit == GKStateBit.Off);
			var turningOffState = StateBits.FirstOrDefault(x => x.StateBit == GKStateBit.TurningOff);
			if (HasTurnOn && offState != null && !offState.IsActive && (turningOffState == null || !turningOffState.IsActive))
			{
				if (OffDelay > 0)
				{
					CurrentOffDelay = OffDelay;
				}
				TurningState = TurningState.TurningOff;
				TurnOff();
			}
		}

		public RelayCommand TurnOffNowCommand { get; private set; }
		void OnTurnOffNow()
		{
			var offState = StateBits.FirstOrDefault(x => x.StateBit == GKStateBit.Off);
			var turningOffState = StateBits.FirstOrDefault(x => x.StateBit == GKStateBit.TurningOff);
			if (HasTurnOn && offState != null && !offState.IsActive && (turningOffState == null || !turningOffState.IsActive))
			{
				TurnOffNow();
			}
		}

		public RelayCommand PauseTurnOnCommand { get; private set; }
		void OnPauseTurnOn()
		{
			if (HasPauseTurnOn)
			{
				if (TurningState == TurningState.TurningOn)
				{
					TurningState = TurningState.Paused;
					var turningOnState = StateBits.FirstOrDefault(x => x.StateBit == GKStateBit.TurningOn);
					if (turningOnState != null)
						turningOnState.IsActive = false;
				}
			}
		}

		void TurnOn()
		{
			CurrentOffDelay = 0;
			CurrentHoldDelay = 0;
			if (OnDelay == 0)
			{
				TurnOnNow();
			}
			else
			{
			    var changed = SetStateBit(GKStateBit.On, false);
				changed = SetStateBit(GKStateBit.TurningOn, true) || changed;
				changed = SetStateBit(GKStateBit.Off, false) || changed;
				changed = SetStateBit(GKStateBit.TurningOff, false) || changed;
				if (changed)
				{
					var journalItem = new ImitatorJournalItem(2, 9, 4, 0);
					AddJournalItem(journalItem);
					RecalculateOutputLogic();
				}
			}
		}

		void TurnOnNow()
		{
			TurningState = TurningState.None;
			CurrentOnDelay = 0;
			CurrentHoldDelay = 0;
			CurrentOffDelay = 0;
			var changed = SetStateBit(GKStateBit.On, true);
			changed = SetStateBit(GKStateBit.TurningOn, false) || changed;
			changed = SetStateBit(GKStateBit.Off, false) || changed;
			changed = SetStateBit(GKStateBit.TurningOff, false) || changed;
			var journalItem = new ImitatorJournalItem(2, 9, 2, 0);
			if (changed)
			{
				AddJournalItem(journalItem);
				RecalculateOutputLogic();
			}
		}

		void TurnOff()
		{
			CurrentOnDelay = 0;
			CurrentHoldDelay = 0;
			if (OffDelay == 0)
			{
				TurnOffNow();
			}
			else
			{
			    var changed = SetStateBit(GKStateBit.On, false);
				changed = SetStateBit(GKStateBit.TurningOn, false) || changed;
				changed = SetStateBit(GKStateBit.Off, false) || changed;
				changed = SetStateBit(GKStateBit.TurningOff, true) || changed;
				if (changed)
				{
					// Сброс пожара
					OnResetFire();

					var journalItem = new ImitatorJournalItem(2, 9, 5, 0);
					AddJournalItem(journalItem);
					RecalculateOutputLogic();
				}
			}
		}

		void TurnOffNow()
		{
			TurningState = TurningState.None;
			CurrentOnDelay = 0;
			CurrentHoldDelay = 0;
			CurrentOffDelay = 0;
			var changed = SetStateBit(GKStateBit.On, false);
			changed = SetStateBit(GKStateBit.TurningOn, false) || changed;
			changed = SetStateBit(GKStateBit.Off, true) || changed;
			changed = SetStateBit(GKStateBit.TurningOff, false) || changed;
			if (changed)
			{
				// Сброс пожара
				OnResetFire();

				var journalItem = new ImitatorJournalItem(2, 9, 3, 3);
				AddJournalItem(journalItem);
				RecalculateOutputLogic();
			}
		}

		void SetGuardAlarm()
		{
			if (GetStateBit(GKStateBit.Attention) || GetStateBit(GKStateBit.Fire1))
				return;

			if (GuardZoneAlarmDelay > 0)
			{
				SetStateBit(GKStateBit.Attention, true);
				SetStateBit(GKStateBit.Fire1, false);
				var journalItem = new ImitatorJournalItem(2, 4, 0, 0);
				AddJournalItem(journalItem);
				IsSettingGuardAlarm = true;
				CurrentAlarmDelay = GuardZoneAlarmDelay;
			}
			else
			{
				SetStateBit(GKStateBit.Attention, false);
				SetStateBit(GKStateBit.Fire1, true);
				var journalItem = new ImitatorJournalItem(2, 2, 0, 0);
				AddJournalItem(journalItem);
			}
		}
	}
}