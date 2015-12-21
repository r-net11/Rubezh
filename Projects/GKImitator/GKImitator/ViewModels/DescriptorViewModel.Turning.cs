using System.Linq;
using RubezhAPI.GK;
using GKImitator.Processor;
using Infrastructure.Common;
using Infrastructure.Common.Windows.ViewModels;
using RubezhDAL.DataClasses;

namespace GKImitator.ViewModels
{
	public partial class DescriptorViewModel : BaseViewModel
	{
		public bool HasOnDelay { get; private set; }
		public bool HasHoldDelay { get; private set; }
		public bool HasOffDelay { get; private set; }
		bool IsSettingGuardAlarm { get; set; }

		ushort OnDelay { get; set; }
		ushort HoldDelay { get; set; }
		ushort OffDelay { get; set; }
		ushort GuardZoneAlarmDelay { get; set; }
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

			var door = GKBase as GKDoor;
			if (door != null)
			{
				OnDelay = (ushort)door.Delay;
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
					var changed = false;
					changed = SetStateBit(GKStateBit.On, true) || changed;
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
			if (IsSettingGuardAlarm)
			{
				if (CurrentAlarmDelay == 0)
				{
					IsSettingGuardAlarm = false;
					SetStateBit(GKStateBit.Attention, false);
					SetStateBit(GKStateBit.Fire1, true);
					var journalItem = new ImitatorJournalItem(2, 2, 0, 0);
					AddJournalItem(journalItem);
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
			if (HasTurnOn && TurningState != TurningState.TurningOn)
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
			if (HasTurnOff && TurningState != TurningState.TurningOff)
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
				if (TurningState == TurningState.TurningOn)
				{
					TurningState = TurningState.Paused;
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
				var changed = false;
				changed = SetStateBit(GKStateBit.On, false) || changed;
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
			CurrentOnDelay = 0;
			CurrentHoldDelay = 0;
			CurrentOffDelay = 0;
			var changed = false;

			changed = SetStateBit(GKStateBit.On, true) || changed;
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
				var changed = false;
				changed = SetStateBit(GKStateBit.On, false) || changed;
				changed = SetStateBit(GKStateBit.TurningOn, false) || changed;
				changed = SetStateBit(GKStateBit.Off, false) || changed;
				changed = SetStateBit(GKStateBit.TurningOff, true) || changed;
				if (changed)
				{
					var journalItem = new ImitatorJournalItem(2, 9, 5, 0);
					AddJournalItem(journalItem);
					RecalculateOutputLogic();
					// Сброс пожара
					SetStateBit(GKStateBit.Fire1, false);
					SetStateBit(GKStateBit.Fire2, false);
				}
			}
		}

		void TurnOffNow()
		{
			CurrentOnDelay = 0;
			CurrentHoldDelay = 0;
			CurrentOffDelay = 0;
			var changed = false;
			changed = SetStateBit(GKStateBit.On, false) || changed;
			changed = SetStateBit(GKStateBit.TurningOn, false) || changed;
			changed = SetStateBit(GKStateBit.Off, true) || changed;
			changed = SetStateBit(GKStateBit.TurningOff, false) || changed;
			if (changed)
			{
				var journalItem = new ImitatorJournalItem(2, 9, 3, 3);
				AddJournalItem(journalItem);
				RecalculateOutputLogic();

				// Сброс пожара
				SetStateBit(GKStateBit.Fire1, false);
				SetStateBit(GKStateBit.Fire2, false);
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