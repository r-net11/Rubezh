using System.Linq;
using FiresecAPI.Models;
using Infrastructure.Common.Windows;
using Infrastructure.Common.Windows.Windows.ViewModels;

namespace DevicesModule.ViewModels
{
	public class SimulationZoneViewModel : BaseViewModel
	{
		public SimulationZoneViewModel(Zone zone)
		{
			Zone = zone;
			SetFireCommand = new RelayCommand(OnSetFire, CanSetFire);
			SetNormCommand = new RelayCommand(OnSetNorm, CanSetNorm);
			SetAttentionCommand = new RelayCommand(OnSetAttention);
			SetMPTAutomaticOnCommand = new RelayCommand(OnSetMPTAutomaticOn, CanSetMPTAutomaticOn);
			SetMPTOnCommand = new RelayCommand(OnSetMPTOn, CanSetMPTOn);
			SetFirefightingCommand = new RelayCommand(OnFirefighting, CanSetFirefighting);
			SetGuardSetCommand = new RelayCommand(OnSetGuardSet, CanSetGuardSet);
			SetGuardUnSetCommand = new RelayCommand(OnSetGuardUnSet, CanSetGuardUnSet);
			SetLampCommand = new RelayCommand(OnSetLamp, CanSetLamp);
			SetPCNCommand = new RelayCommand(OnSetPCN, CanSetPCN);
			SetAlarmCommand = new RelayCommand(OnSetAlarm, CanSetAlarm);
		}

		public void Initialize()
		{
		}

		public Zone Zone { get; private set; }

		ZoneLogicState? _zoneState;
		public ZoneLogicState? ZoneState
		{
			get { return _zoneState; }
			set
			{
				_zoneState = value;
				OnPropertyChanged(() => ZoneState);
			}
		}

		void UpdateDevices()
		{
			SimulationViewModel.Current.UpdateDevices();
		}

		public RelayCommand SetFireCommand { get; private set; }
		void OnSetFire()
		{
			ZoneState = ZoneLogicState.Fire;
			UpdateDevices();
		}
		bool CanSetFire()
		{
			return Zone.ZoneType == ZoneType.Fire;
		}

		public RelayCommand SetAttentionCommand { get; private set; }
		void OnSetAttention()
		{
			ZoneState = ZoneLogicState.Attention;
			UpdateDevices();
		}
		bool CanSetAttention()
		{
			return Zone.ZoneType == ZoneType.Fire;
		}

		public RelayCommand SetMPTAutomaticOnCommand { get; private set; }
		void OnSetMPTAutomaticOn()
		{
			ZoneState = ZoneLogicState.MPTAutomaticOn;
			UpdateDevices();
		}
		bool CanSetMPTAutomaticOn()
		{
			return Zone.ZoneType == ZoneType.Fire && Zone.DevicesInZone.Any(x => x.Driver.DriverType == DriverType.MPT);
		}

		public RelayCommand SetMPTOnCommand { get; private set; }
		void OnSetMPTOn()
		{
			ZoneState = ZoneLogicState.MPTOn;
			UpdateDevices();
		}
		bool CanSetMPTOn()
		{
			return Zone.ZoneType == ZoneType.Fire && Zone.DevicesInZone.Any(x=>x.Driver.DriverType == DriverType.MPT);
		}

		public RelayCommand SetFirefightingCommand { get; private set; }
		void OnFirefighting()
		{
			ZoneState = ZoneLogicState.Firefighting;
			UpdateDevices();
		}
		bool CanSetFirefighting()
		{
			return Zone.ZoneType == ZoneType.Fire && Zone.DevicesInZone.Any(x => x.Driver.DriverType == DriverType.MPT);
		}

		public RelayCommand SetAlarmCommand { get; private set; }
		void OnSetAlarm()
		{
			ZoneState = ZoneLogicState.Alarm;
			UpdateDevices();
		}
		bool CanSetAlarm()
		{
			return Zone.ZoneType == ZoneType.Guard;
		}

		public RelayCommand SetGuardSetCommand { get; private set; }
		void OnSetGuardSet()
		{
			ZoneState = ZoneLogicState.GuardSet;
			UpdateDevices();
		}
		bool CanSetGuardSet()
		{
			return Zone.ZoneType == ZoneType.Guard;
		}

		public RelayCommand SetGuardUnSetCommand { get; private set; }
		void OnSetGuardUnSet()
		{
			ZoneState = ZoneLogicState.GuardUnSet;
			UpdateDevices();
		}
		bool CanSetGuardUnSet()
		{
			return Zone.ZoneType == ZoneType.Guard;
		}

		public RelayCommand SetLampCommand { get; private set; }
		void OnSetLamp()
		{
			ZoneState = ZoneLogicState.Lamp;
			UpdateDevices();
		}
		bool CanSetLamp()
		{
			return Zone.ZoneType == ZoneType.Guard;
		}

		public RelayCommand SetPCNCommand { get; private set; }
		void OnSetPCN()
		{
			ZoneState = ZoneLogicState.PCN;
			UpdateDevices();
		}
		bool CanSetPCN()
		{
			return Zone.ZoneType == ZoneType.Guard;
		}

		public RelayCommand SetNormCommand { get; private set; }
		void OnSetNorm()
		{
			ZoneState = null;
			UpdateDevices();
		}
		bool CanSetNorm()
		{
			return ZoneState != null;
		}
	}
}