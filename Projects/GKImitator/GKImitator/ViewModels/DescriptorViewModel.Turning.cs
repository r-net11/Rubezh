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
		public bool HasTurnOn { get; private set; }
		public bool HasTurnOnNow { get; private set; }
		public bool HasTurnOff { get; private set; }
		public bool HasTurnOffNow { get; private set; }
		public bool HasPauseTurnOn { get; private set; }
		bool IsSettingGuardAlarm { get; set; }
		ushort OnDelay { get; set; }
		ushort HoldDelay { get; set; }
		ushort HoldOffDelay { get; set; }
		ushort OffDelay { get; set; }
		ushort GuardZoneAlarmDelay { get; set; }
		DelayRegime? DelayRegime { get; set; }
		TurningState TurningState = TurningState.None;

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
					property = device.Properties.FirstOrDefault(x => x.Name == "Время включения, с");
					if (property != null)
						HoldDelay = property.Value;
					property = device.Properties.FirstOrDefault(x => x.Name == "Время выключения, с");
					if (property != null)
						OffDelay = property.Value;
					property = device.Properties.FirstOrDefault(x => x.Name == "Удержание открытия, мин");
					if (property != null)
						HoldDelay = (ushort) (property.Value * 60);
				}
				property = device.Properties.FirstOrDefault(x => x.Name == "Задержка на выключение, с");
				if (property != null)
				{
					OffDelay = property.Value;
				}
				property = device.Properties.FirstOrDefault(x => x.Name == "Режим после удержания включенного состояния");
				if (property != null)
					DelayRegime = property.Value == 0 ? RubezhAPI.GK.DelayRegime.Off : RubezhAPI.GK.DelayRegime.On;
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

		public object locker = new object();
		public void CheckDelays()
		{
			lock (locker)
			{
				if (TurningState == TurningState.TurningOn)
				{
					if (CurrentOnDelay == 0)
					{
						if (!IsMduOrBuz)
						{
							bool changed = SetStateBit(GKStateBit.TurningOn, false);
							changed = SetStateBit(GKStateBit.Off, false) || changed;
							changed = SetStateBit(GKStateBit.TurningOff, false) || changed;
							changed = SetStateBit(GKStateBit.On, true) || changed;
						}
						else
						{
							TurningState = TurningState.Holding;
							CurrentHoldDelay = HoldDelay;
						}
					}
					else
					{
						AdditionalShortParameters[0] = CurrentOnDelay;
						if (IsBush || IsMduOrBuz)
							AdditionalShortParameters[1] = 1;
						CurrentOnDelay--;
					}
				}
				if (TurningState == TurningState.Holding)
				{
					if (CurrentHoldDelay == 0)
					{
						if (IsMduOrBuz)
						{
							bool changed = SetStateBit(GKStateBit.TurningOn, false);
							changed = SetStateBit(GKStateBit.Off, false) || changed;
							changed = SetStateBit(GKStateBit.TurningOff, false) || changed;
							changed = SetStateBit(GKStateBit.On, true) || changed;
						}
						else
						{
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
					}
					else
					{
						if (IsMduOrBuz)
							AdditionalShortParameters[1] = 2;
						AdditionalShortParameters[IsMduOrBuz ? 0 : 1] = CurrentHoldDelay;
						CurrentHoldDelay--;
					}
				}
				if (TurningState == TurningState.TurningOff)
				{
					if (CurrentOffDelay == 0)
					{
						var changed = SetStateBit(GKStateBit.On, false);
						changed = SetStateBit(GKStateBit.TurningOn, false) || changed;
						changed = SetStateBit(GKStateBit.TurningOff, false) || changed;
						changed = SetStateBit(GKStateBit.Off, true) || changed;
					}
					else
					{
						if (IsBush || IsMduOrBuz)
							AdditionalShortParameters[1] = 2;
						AdditionalShortParameters[GKBase is GKGuardZone || GKBase is GKDoor || IsBush || IsMduOrBuz ? 0 : 2] = CurrentOffDelay;
						CurrentOffDelay--;
					}
				}
				if (IsSettingGuardAlarm)
				{
					if (CurrentAlarmDelay == 0)
					{
						IsSettingGuardAlarm = false;
						SetStateBit(GKStateBit.Attention, false);
						SetStateBit(GKStateBit.Fire1, true);
					}
					else
					{
						AdditionalShortParameters[1] = CurrentAlarmDelay;
						CurrentAlarmDelay--;
					}
				}
			}
		}

		bool IsBush
		{
			get
			{
				var device = GKBase as GKDevice;
				return (device != null && (device.DriverType == GKDriverType.RSR2_Bush_Drenazh || device.DriverType == GKDriverType.RSR2_Bush_Drenazh || device.DriverType == GKDriverType.RSR2_Bush_Jokey
					|| device.DriverType == GKDriverType.RSR2_Bush_Fire || device.DriverType == GKDriverType.RSR2_Bush_Shuv || device.DriverType == GKDriverType.RSR2_Valve_DU
					|| device.DriverType == GKDriverType.RSR2_Valve_KV || device.DriverType == GKDriverType.RSR2_Valve_KVMV));
			}
		}

		bool IsMduOrBuz
		{
			get
			{
				var device = GKBase as GKDevice;
				return (device != null && (device.DriverType == GKDriverType.RSR2_MDU || device.DriverType == GKDriverType.RSR2_MDU24 || device.DriverType == GKDriverType.RSR2_Buz_KV
					|| device.DriverType == GKDriverType.RSR2_Buz_KVMV || device.DriverType == GKDriverType.RSR2_Buz_KVDU));
			}
		}

		public RelayCommand TurnOnCommand { get; private set; }
		void OnTurnOn()
		{
			if (CanDo(GKStateBit.TurnOn_InManual))
			{
				TurnOn();
			}
		}

		public RelayCommand TurnOnNowCommand { get; private set; }
		void OnTurnOnNow()
		{
			if (CanDo(GKStateBit.TurnOnNow_InManual))
			{
				TurnOnNow();
			}
		}

		bool CanDo(GKStateBit stateBit)
		{
			var onState = StateBits.FirstOrDefault(x => x.StateBit == GKStateBit.On);
			var turningOnState = StateBits.FirstOrDefault(x => x.StateBit == GKStateBit.TurningOn);
			var offState = StateBits.FirstOrDefault(x => x.StateBit == GKStateBit.Off);
			var turningOffState = StateBits.FirstOrDefault(x => x.StateBit == GKStateBit.TurningOff);
			var fire1State = StateBits.FirstOrDefault(x => x.StateBit == GKStateBit.Fire1);
			var fire2State = StateBits.FirstOrDefault(x => x.StateBit == GKStateBit.Fire2);
			if (stateBit == GKStateBit.TurnOn_InManual)
				return onState != null && !onState.IsActive && (turningOnState == null || !turningOnState.IsActive);
			if (stateBit == GKStateBit.TurnOnNow_InManual)
				return onState != null && !onState.IsActive;
			if (stateBit == GKStateBit.TurnOff_InManual)
				return offState != null && !offState.IsActive && (turningOffState == null || !turningOffState.IsActive);
			if (stateBit == GKStateBit.TurnOffNow_InManual)
				return offState != null && !offState.IsActive;
			if (stateBit == GKStateBit.Fire1)
				return fire1State != null && !fire1State.IsActive;
			if (stateBit == GKStateBit.Fire2)
				return fire2State != null && !fire2State.IsActive;
			return false;
		}


		public RelayCommand TurnOffCommand { get; private set; }
		void OnTurnOff()
		{
			if (CanDo(GKStateBit.TurnOff_InManual))
			{
				TurnOff();
			}
		}

		public RelayCommand TurnOffNowCommand { get; private set; }
		void OnTurnOffNow()
		{
			if (CanDo(GKStateBit.TurnOffNow_InManual))
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
						SetStateBit(turningOnState.StateBit, false);
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
				var changed = SetStateBit(GKStateBit.On, false);
				changed = SetStateBit(GKStateBit.Off, false) || changed;
				changed = SetStateBit(GKStateBit.TurningOff, false) || changed;
				changed = SetStateBit(GKStateBit.TurningOn, true) || changed;
			}
		}

		void TurnOnNow()
		{
			var changed = SetStateBit(GKStateBit.TurningOn, false);
			changed = SetStateBit(GKStateBit.Off, false) || changed;
			changed = SetStateBit(GKStateBit.TurningOff, false) || changed;
			changed = SetStateBit(GKStateBit.On, true) || changed;
		}

		void TurnOff()
		{
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
			}
		}

		void TurnOffNow()
		{
			var changed = SetStateBit(GKStateBit.On, false);
			changed = SetStateBit(GKStateBit.TurningOn, false) || changed;
			changed = SetStateBit(GKStateBit.TurningOff, false) || changed;
			changed = SetStateBit(GKStateBit.Off, true) || changed;
		}

		void SetGuardAlarm()
		{
			if (GetStateBit(GKStateBit.Attention) || GetStateBit(GKStateBit.Fire1))
				return;
			if (GuardZoneAlarmDelay > 0)
			{
				SetStateBit(GKStateBit.Fire1, false);
				SetStateBit(GKStateBit.Attention, true);
				IsSettingGuardAlarm = true;
				CurrentAlarmDelay = GuardZoneAlarmDelay;
			}
			else
			{
				SetStateBit(GKStateBit.Attention, false);
				SetStateBit(GKStateBit.Fire1, true);
			}
		}
	}
}