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
		void InitializeAll()
		{
			StateBits = new ObservableCollection<StateBitViewModel>();
			StateBits.Add(new StateBitViewModel(this, GKStateBit.Norm, true));
			StateBits.Add(new StateBitViewModel(this, GKStateBit.Ignore));

			Failures = new ObservableCollection<FailureViewModel>();
			HasParameters = true;

			if (GKBase is GKDevice)
			{
				var device = GKBase as GKDevice;
				Failures.Add(new FailureViewModel(this, JournalEventDescriptionType.Потеря_связи, 255));
				HasParameters = device.Properties.Where(x => x.DriverProperty.IsAUParameter).Any();

				switch (device.DriverType)
				{
					case GKDriverType.GK:
						HasAutomaticRegime = false;
						HasManualRegime = false;
						HasIgnoreRegime = false;
						HasTechnoligicalCommands = true;
						HasUserCommands = true;
						break;

					case GKDriverType.RSR2_KAU:
						HasAutomaticRegime = false;
						HasManualRegime = false;
						HasIgnoreRegime = false;
						HasTechnoligicalCommands = true;
						break;

					case GKDriverType.GKIndicator:
						HasAutomaticRegime = false;
						HasManualRegime = false;
						HasIgnoreRegime = false;
						break;

					case GKDriverType.GKLine:
						HasAutomaticRegime = false;
						HasManualRegime = false;
						HasIgnoreRegime = false;
						break;

					case GKDriverType.GKRele:
						HasAutomaticRegime = true;
						HasManualRegime = true;
						HasIgnoreRegime = true;
						break;

					case GKDriverType.KAUIndicator:
						HasAutomaticRegime = false;
						HasManualRegime = false;
						HasIgnoreRegime = false;
						break;

					case GKDriverType.RSR2_HandDetector:
						HasAutomaticRegime = true;
						HasManualRegime = true;
						HasIgnoreRegime = true;
						StateBits.Add(new StateBitViewModel(this, GKStateBit.Fire2));
						HasStateFire2 = true;
						HasSetFireHeandDetector = true;
						HasResetFire = true;
						break;

					case GKDriverType.RSR2_SmokeDetector:
						HasAutomaticRegime = true;
						HasManualRegime = false;
						HasIgnoreRegime = true;
						HasTestButton = true;
						HasTestLaser = true;
						HasResetTest = true;
						StateBits.Add(new StateBitViewModel(this, GKStateBit.Test));
						HasStateTest = true;
						HasSetFireSmoke = true;
						HasResetFire = true;
						StateBits.Add(new StateBitViewModel(this, GKStateBit.Fire1));
						HasStateFire1 = true;
						HasDustiness = true;
						break;

					case GKDriverType.RSR2_CombinedDetector:
						HasAutomaticRegime = true;
						HasManualRegime = false;
						HasIgnoreRegime = true;
						HasTestButton = true;
						HasTestLaser = true;
						HasResetTest = true;
						StateBits.Add(new StateBitViewModel(this, GKStateBit.Test));
						HasStateTest = true;
						HasSetFireTemperatureGradient = true;
						HasResetFire = true;
						StateBits.Add(new StateBitViewModel(this, GKStateBit.Fire1));
						HasStateFire1 = true;
						HasDustiness = true;
						break;

					case GKDriverType.RSR2_HeatDetector:
						HasAutomaticRegime = true;
						HasManualRegime = false;
						HasIgnoreRegime = true;
						HasTestButton = true;
						HasTestLaser = true;
						HasResetTest = true;
						StateBits.Add(new StateBitViewModel(this, GKStateBit.Test));
						HasStateTest = true;
						HasSetFireTemperature = true;
						HasResetFire = true;
						StateBits.Add(new StateBitViewModel(this, GKStateBit.Fire1));
						HasStateFire1 = true;
						HasDustiness = true;
						break;

					case GKDriverType.RSR2_AM_1:
						HasAutomaticRegime = true;
						HasManualRegime = false;
						HasIgnoreRegime = true;
						break;

					case GKDriverType.RSR2_MAP4:
						HasAutomaticRegime = true;
						HasManualRegime = false;
						HasIgnoreRegime = true;
						HasReset = true;
						break;

					case GKDriverType.RSR2_RM_1:
						HasAutomaticRegime = true;
						HasManualRegime = true;
						HasIgnoreRegime = true;
						HasTurnOn = true;
						HasTurnOnNow = true;
						HasTurnOff = true;
						HasTurnOffNow = true;
						StateBits.Add(new StateBitViewModel(this, GKStateBit.On));
						StateBits.Add(new StateBitViewModel(this, GKStateBit.TurningOn));
						StateBits.Add(new StateBitViewModel(this, GKStateBit.Off));
						StateBits.Add(new StateBitViewModel(this, GKStateBit.TurningOff));
						HasStateOn = true;
						HasStateOff = true;
						HasStateTurningOn = true;
						HasStateTurningOff = true;
						HasOnDelay = true;
						HasHoldDelay = true;
						HasOffDelay = true;
						break;

					case GKDriverType.RSR2_MDU:
						HasAutomaticRegime = true;
						HasManualRegime = true;
						HasIgnoreRegime = true;
						HasTurnOn = true;
						HasTurnOnNow = true;
						HasTurnOff = true;
						HasTurnOffNow = true;
						StateBits.Add(new StateBitViewModel(this, GKStateBit.On));
						StateBits.Add(new StateBitViewModel(this, GKStateBit.TurningOn));
						StateBits.Add(new StateBitViewModel(this, GKStateBit.Off));
						StateBits.Add(new StateBitViewModel(this, GKStateBit.TurningOff));
						HasStateOn = true;
						HasStateOff = true;
						HasStateTurningOn = true;
						HasStateTurningOff = true;
						HasOnDelay = true;
						HasHoldDelay = true;
						HasOffDelay = true;
						break;

					case GKDriverType.RSR2_MDU24:
						HasAutomaticRegime = true;
						HasManualRegime = true;
						HasIgnoreRegime = true;
						HasTurnOn = true;
						HasTurnOnNow = true;
						HasTurnOff = true;
						HasTurnOffNow = true;
						StateBits.Add(new StateBitViewModel(this, GKStateBit.On));
						StateBits.Add(new StateBitViewModel(this, GKStateBit.TurningOn));
						StateBits.Add(new StateBitViewModel(this, GKStateBit.Off));
						StateBits.Add(new StateBitViewModel(this, GKStateBit.TurningOff));
						HasStateOn = true;
						HasStateOff = true;
						HasStateTurningOn = true;
						HasStateTurningOff = true;
						HasOnDelay = true;
						HasHoldDelay = true;
						HasOffDelay = true;
						break;

					case GKDriverType.RSR2_MVK8:
						HasAutomaticRegime = true;
						HasManualRegime = true;
						HasIgnoreRegime = true;
						HasTurnOn = true;
						HasTurnOnNow = true;
						HasTurnOff = true;
						HasTurnOffNow = true;
						StateBits.Add(new StateBitViewModel(this, GKStateBit.On));
						StateBits.Add(new StateBitViewModel(this, GKStateBit.TurningOn));
						StateBits.Add(new StateBitViewModel(this, GKStateBit.Off));
						StateBits.Add(new StateBitViewModel(this, GKStateBit.TurningOff));
						HasStateOn = true;
						HasStateOff = true;
						HasStateTurningOn = true;
						HasStateTurningOff = true;
						HasOnDelay = true;
						HasHoldDelay = true;
						HasOffDelay = true;
						break;

					case GKDriverType.RSR2_Bush_Drenazh:
						HasAutomaticRegime = true;
						HasManualRegime = true;
						HasIgnoreRegime = true;
						HasTurnOn = true;
						HasTurnOnNow = true;
						HasTurnOff = true;
						HasTurnOffNow = true;
						StateBits.Add(new StateBitViewModel(this, GKStateBit.On));
						StateBits.Add(new StateBitViewModel(this, GKStateBit.TurningOn));
						StateBits.Add(new StateBitViewModel(this, GKStateBit.Off));
						StateBits.Add(new StateBitViewModel(this, GKStateBit.TurningOff));
						HasStateOn = true;
						HasStateOff = true;
						HasStateTurningOn = true;
						HasStateTurningOff = true;
						HasOnDelay = true;
						HasHoldDelay = true;
						HasOffDelay = true;
						break;

					case GKDriverType.RSR2_Bush_Jokey:
						HasAutomaticRegime = true;
						HasManualRegime = true;
						HasIgnoreRegime = true;
						HasTurnOn = true;
						HasTurnOnNow = true;
						HasTurnOff = true;
						HasTurnOffNow = true;
						StateBits.Add(new StateBitViewModel(this, GKStateBit.On));
						StateBits.Add(new StateBitViewModel(this, GKStateBit.TurningOn));
						StateBits.Add(new StateBitViewModel(this, GKStateBit.Off));
						StateBits.Add(new StateBitViewModel(this, GKStateBit.TurningOff));
						HasStateOn = true;
						HasStateOff = true;
						HasStateTurningOn = true;
						HasStateTurningOff = true;
						HasOnDelay = true;
						HasHoldDelay = true;
						HasOffDelay = true;
						break;

					case GKDriverType.RSR2_Bush_Fire:
						HasAutomaticRegime = true;
						HasManualRegime = true;
						HasIgnoreRegime = true;
						HasTurnOn = true;
						HasTurnOnNow = true;
						HasTurnOff = true;
						HasTurnOffNow = true;
						StateBits.Add(new StateBitViewModel(this, GKStateBit.On));
						StateBits.Add(new StateBitViewModel(this, GKStateBit.TurningOn));
						StateBits.Add(new StateBitViewModel(this, GKStateBit.Off));
						StateBits.Add(new StateBitViewModel(this, GKStateBit.TurningOff));
						HasStateOn = true;
						HasStateOff = true;
						HasStateTurningOn = true;
						HasStateTurningOff = true;
						HasOnDelay = true;
						HasHoldDelay = true;
						HasOffDelay = true;
						break;

					case GKDriverType.RSR2_Bush_Shuv:
						HasAutomaticRegime = true;
						HasManualRegime = true;
						HasIgnoreRegime = true;
						HasTurnOn = true;
						HasTurnOnNow = true;
						HasTurnOff = true;
						HasTurnOffNow = true;
						StateBits.Add(new StateBitViewModel(this, GKStateBit.On));
						StateBits.Add(new StateBitViewModel(this, GKStateBit.TurningOn));
						StateBits.Add(new StateBitViewModel(this, GKStateBit.Off));
						StateBits.Add(new StateBitViewModel(this, GKStateBit.TurningOff));
						HasStateOn = true;
						HasStateOff = true;
						HasStateTurningOn = true;
						HasStateTurningOff = true;
						HasOnDelay = true;
						HasHoldDelay = true;
						HasOffDelay = true;
						break;

					case GKDriverType.RSR2_Valve_KV:
						HasAutomaticRegime = true;
						HasManualRegime = true;
						HasIgnoreRegime = true;
						HasTurnOn = true;
						HasTurnOnNow = true;
						HasTurnOff = true;
						HasTurnOffNow = true;
						StateBits.Add(new StateBitViewModel(this, GKStateBit.On));
						StateBits.Add(new StateBitViewModel(this, GKStateBit.TurningOn));
						StateBits.Add(new StateBitViewModel(this, GKStateBit.Off));
						StateBits.Add(new StateBitViewModel(this, GKStateBit.TurningOff));
						HasStateOn = true;
						HasStateOff = true;
						HasStateTurningOn = true;
						HasStateTurningOff = true;
						HasOnDelay = true;
						HasHoldDelay = true;
						HasOffDelay = true;
						break;

					case GKDriverType.RSR2_Valve_KVMV:
						HasAutomaticRegime = true;
						HasManualRegime = true;
						HasIgnoreRegime = true;
						HasTurnOn = true;
						HasTurnOnNow = true;
						HasTurnOff = true;
						HasTurnOffNow = true;
						StateBits.Add(new StateBitViewModel(this, GKStateBit.On));
						StateBits.Add(new StateBitViewModel(this, GKStateBit.TurningOn));
						StateBits.Add(new StateBitViewModel(this, GKStateBit.Off));
						StateBits.Add(new StateBitViewModel(this, GKStateBit.TurningOff));
						HasStateOn = true;
						HasStateOff = true;
						HasStateTurningOn = true;
						HasStateTurningOff = true;
						HasOnDelay = true;
						HasHoldDelay = true;
						HasOffDelay = true;
						break;

					case GKDriverType.RSR2_Valve_DU:
						HasAutomaticRegime = true;
						HasManualRegime = true;
						HasIgnoreRegime = true;
						HasTurnOn = true;
						HasTurnOnNow = true;
						HasTurnOff = true;
						HasTurnOffNow = true;
						StateBits.Add(new StateBitViewModel(this, GKStateBit.On));
						StateBits.Add(new StateBitViewModel(this, GKStateBit.TurningOn));
						StateBits.Add(new StateBitViewModel(this, GKStateBit.Off));
						StateBits.Add(new StateBitViewModel(this, GKStateBit.TurningOff));
						HasStateOn = true;
						HasStateOff = true;
						HasStateTurningOn = true;
						HasStateTurningOff = true;
						HasOnDelay = true;
						HasHoldDelay = true;
						HasOffDelay = true;
						break;

					case GKDriverType.RSR2_OPK:
						HasAutomaticRegime = true;
						HasManualRegime = true;
						HasIgnoreRegime = true;
						HasTurnOn = true;
						HasTurnOnNow = true;
						HasTurnOff = true;
						HasTurnOffNow = true;
						StateBits.Add(new StateBitViewModel(this, GKStateBit.On));
						StateBits.Add(new StateBitViewModel(this, GKStateBit.TurningOn));
						StateBits.Add(new StateBitViewModel(this, GKStateBit.Off));
						StateBits.Add(new StateBitViewModel(this, GKStateBit.TurningOff));
						HasStateOn = true;
						HasStateOff = true;
						HasStateTurningOn = true;
						HasStateTurningOff = true;
						HasOnDelay = true;
						HasHoldDelay = true;
						HasOffDelay = true;
						break;

					case GKDriverType.RSR2_OPS:
						HasAutomaticRegime = true;
						HasManualRegime = true;
						HasIgnoreRegime = true;
						HasTurnOn = true;
						HasTurnOnNow = true;
						HasTurnOff = true;
						HasTurnOffNow = true;
						StateBits.Add(new StateBitViewModel(this, GKStateBit.On));
						StateBits.Add(new StateBitViewModel(this, GKStateBit.TurningOn));
						StateBits.Add(new StateBitViewModel(this, GKStateBit.Off));
						StateBits.Add(new StateBitViewModel(this, GKStateBit.TurningOff));
						HasStateOn = true;
						HasStateOff = true;
						HasStateTurningOn = true;
						HasStateTurningOff = true;
						HasOnDelay = true;
						HasHoldDelay = true;
						HasOffDelay = true;
						break;

					case GKDriverType.RSR2_OPZ:
						HasAutomaticRegime = true;
						HasManualRegime = true;
						HasIgnoreRegime = true;
						HasTurnOn = true;
						HasTurnOnNow = true;
						HasTurnOff = true;
						HasTurnOffNow = true;
						StateBits.Add(new StateBitViewModel(this, GKStateBit.On));
						StateBits.Add(new StateBitViewModel(this, GKStateBit.TurningOn));
						StateBits.Add(new StateBitViewModel(this, GKStateBit.Off));
						StateBits.Add(new StateBitViewModel(this, GKStateBit.TurningOff));
						HasStateOn = true;
						HasStateOff = true;
						HasStateTurningOn = true;
						HasStateTurningOff = true;
						HasOnDelay = true;
						HasHoldDelay = true;
						HasOffDelay = true;
						break;

					case GKDriverType.RSR2_Buz_KV:
						HasAutomaticRegime = true;
						HasManualRegime = true;
						HasIgnoreRegime = true;
						HasTurnOn = true;
						HasTurnOnNow = true;
						HasTurnOff = true;
						HasTurnOffNow = true;
						StateBits.Add(new StateBitViewModel(this, GKStateBit.On));
						StateBits.Add(new StateBitViewModel(this, GKStateBit.TurningOn));
						StateBits.Add(new StateBitViewModel(this, GKStateBit.Off));
						StateBits.Add(new StateBitViewModel(this, GKStateBit.TurningOff));
						HasStateOn = true;
						HasStateOff = true;
						HasStateTurningOn = true;
						HasStateTurningOff = true;
						HasOnDelay = true;
						HasHoldDelay = true;
						HasOffDelay = true;
						break;

					case GKDriverType.RSR2_Buz_KVMV:
						HasAutomaticRegime = true;
						HasManualRegime = true;
						HasIgnoreRegime = true;
						HasTurnOn = true;
						HasTurnOnNow = true;
						HasTurnOff = true;
						HasTurnOffNow = true;
						StateBits.Add(new StateBitViewModel(this, GKStateBit.On));
						StateBits.Add(new StateBitViewModel(this, GKStateBit.TurningOn));
						StateBits.Add(new StateBitViewModel(this, GKStateBit.Off));
						StateBits.Add(new StateBitViewModel(this, GKStateBit.TurningOff));
						HasStateOn = true;
						HasStateOff = true;
						HasStateTurningOn = true;
						HasStateTurningOff = true;
						HasOnDelay = true;
						HasHoldDelay = true;
						HasOffDelay = true;
						break;

					case GKDriverType.RSR2_Buz_KVDU:
						HasAutomaticRegime = true;
						HasManualRegime = true;
						HasIgnoreRegime = true;
						HasTurnOn = true;
						HasTurnOnNow = true;
						HasTurnOff = true;
						HasTurnOffNow = true;
						StateBits.Add(new StateBitViewModel(this, GKStateBit.On));
						StateBits.Add(new StateBitViewModel(this, GKStateBit.TurningOn));
						StateBits.Add(new StateBitViewModel(this, GKStateBit.Off));
						StateBits.Add(new StateBitViewModel(this, GKStateBit.TurningOff));
						HasStateOn = true;
						HasStateOff = true;
						HasStateTurningOn = true;
						HasStateTurningOff = true;
						HasOnDelay = true;
						HasHoldDelay = true;
						HasOffDelay = true;
						break;

					case GKDriverType.RSR2_MVP:
						HasAutomaticRegime = true;
						HasManualRegime = true;
						HasIgnoreRegime = true;
						break;

					case GKDriverType.RSR2_CodeReader:
						HasAutomaticRegime = true;
						HasManualRegime = true;
						HasIgnoreRegime = true;
						break;

					case GKDriverType.RSR2_GuardDetector:
						HasAutomaticRegime = true;
						HasManualRegime = true;
						HasIgnoreRegime = true;
						break;

					case GKDriverType.RSR2_CardReader:
						HasAutomaticRegime = true;
						HasManualRegime = true;
						HasIgnoreRegime = true;
						break;

					case GKDriverType.RSR2_GuardDetectorSound:
						HasAutomaticRegime = true;
						HasManualRegime = true;
						HasIgnoreRegime = true;
						break;

				}
			}

			if (GKBase is GKZone)
			{
				HasAutomaticRegime = true;
				HasManualRegime = false;
				HasIgnoreRegime = true;
				HasReset = true;
				StateBits.Add(new StateBitViewModel(this, GKStateBit.Attention));
				StateBits.Add(new StateBitViewModel(this, GKStateBit.Fire1));
				StateBits.Add(new StateBitViewModel(this, GKStateBit.Fire2));
				HasStateAttention = true;
				HasStateFire1 = true;
				HasStateFire2 = true;
			}

			if (GKBase is GKDirection || GKBase is GKMPT || GKBase is GKPumpStation)
			{
				HasAutomaticRegime = true;
				HasManualRegime = true;
				HasIgnoreRegime = true;
				HasTurnOn = true;
				HasTurnOnNow = true;
				HasTurnOff = true;
				HasPauseTurnOn = true;
				StateBits.Add(new StateBitViewModel(this, GKStateBit.On));
				StateBits.Add(new StateBitViewModel(this, GKStateBit.Off));
				StateBits.Add(new StateBitViewModel(this, GKStateBit.TurningOn));
				StateBits.Add(new StateBitViewModel(this, GKStateBit.TurningOff));
				HasStateOn = true;
				HasStateOff = true;
				HasStateTurningOn = true;
				HasStateTurningOff = true;
				HasOnDelay = true;
				HasHoldDelay = true;
				HasOffDelay = false;
			}

			if (GKBase is GKDelay)
			{
				HasAutomaticRegime = true;
				HasManualRegime = true;
				HasIgnoreRegime = true;
				HasTurnOn = true;
				HasTurnOnNow = true;
				HasTurnOff = true;
				StateBits.Add(new StateBitViewModel(this, GKStateBit.On));
				StateBits.Add(new StateBitViewModel(this, GKStateBit.Off));
				StateBits.Add(new StateBitViewModel(this, GKStateBit.TurningOn));
				StateBits.Add(new StateBitViewModel(this, GKStateBit.TurningOff));
				HasStateOn = true;
				HasStateOff = true;
				HasStateTurningOn = true;
				HasStateTurningOff = true;
				HasOnDelay = true;
				HasHoldDelay = true;
				HasOffDelay = false;
			}

			if (GKBase is GKPim)
			{
				HasAutomaticRegime = true;
				HasManualRegime = true;
				HasIgnoreRegime = true;
				HasTurnOn = true;
				HasTurnOnNow = true;
				HasTurnOff = true;
				StateBits.Add(new StateBitViewModel(this, GKStateBit.On));
				StateBits.Add(new StateBitViewModel(this, GKStateBit.Off));
				StateBits.Add(new StateBitViewModel(this, GKStateBit.TurningOn));
				StateBits.Add(new StateBitViewModel(this, GKStateBit.TurningOff));
				HasStateOn = true;
				HasStateOff = true;
				HasStateTurningOn = true;
				HasStateTurningOff = true;
				HasOnDelay = true;
				HasHoldDelay = true;
				HasOffDelay = true;
			}

			if (GKBase is GKDoor)
			{
				HasAutomaticRegime = true;
				HasManualRegime = true;
				HasIgnoreRegime = true;
			}

			if (GKBase is GKGuardZone)
			{
				HasAutomaticRegime = true;
				HasManualRegime = true;
				HasIgnoreRegime = true;
			}
		}
	}
}