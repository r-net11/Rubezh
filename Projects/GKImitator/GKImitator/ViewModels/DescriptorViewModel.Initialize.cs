using System.Collections.ObjectModel;
using System.Linq;
using RubezhAPI.GK;
using Infrastructure.Common.Windows.Windows.ViewModels;
using RubezhAPI.Journal;

namespace GKImitator.ViewModels
{
	public partial class DescriptorViewModel
	{
		void InitializeAll()
		{
			StateBits = new ObservableCollection<StateBitViewModel>();
			AddStateBit(GKStateBit.Norm, true);
			AddStateBit(GKStateBit.Ignore);

			Failures = new ObservableCollection<FailureViewModel>();
			HasParameters = true;
			HasMeasure = false;

			if (GKBase is GKDevice)
			{
				var device = GKBase as GKDevice;
				Failures.Add(new FailureViewModel(this, JournalEventDescriptionType.Потеря_связи_Неисправность, 255));
				HasParameters = device.Properties.Any(x => x.DriverProperty.IsAUParameter);
				HasMeasure = device.Driver.MeasureParameters.Any();
				var driverType = device.DriverType;

				HasAutomaticRegime = device.Driver.IsControlDevice || (device.Driver.IsDeviceOnShleif && !device.Driver.IsControlDevice);
				HasManualRegime = device.Driver.IsControlDevice;
				HasIgnoreRegime = device.Driver.IsControlDevice || (device.Driver.IsDeviceOnShleif && !device.Driver.IsControlDevice);
				HasReset = device.Driver.IsDeviceOnShleif && !device.Driver.IsControlDevice;
				HasTechnoligicalCommands = driverType == GKDriverType.GK || driverType == GKDriverType.RSR2_KAU;
				HasUserCommands = driverType == GKDriverType.GK;
				HasSetFireHandDetector = driverType == GKDriverType.RSR2_HandDetector || driverType == GKDriverType.RSR2_HandDetectorEridan;
				HasSetFireSmoke = driverType == GKDriverType.RSR2_SmokeDetector;
				HasSetFireTemperature = driverType == GKDriverType.RSR2_HeatDetector || driverType == GKDriverType.RSR2_HeatDetectorEridan || driverType == GKDriverType.RSR2_ABTK;
				HasSetFireTemperatureGradient = driverType == GKDriverType.RSR2_CombinedDetector || driverType == GKDriverType.RSR2_IOLIT;
				HasDustiness = driverType == GKDriverType.RSR2_SmokeDetector || driverType == GKDriverType.RSR2_HandDetector || driverType == GKDriverType.RSR2_CombinedDetector;
				HasFire1 = driverType == GKDriverType.RSR2_Button;
				HasFire12 = driverType == GKDriverType.RSR2_AM_1 || driverType == GKDriverType.RSR2_MAP4 || driverType == GKDriverType.RSR2_ABShS;
				HasCard = driverType == GKDriverType.RSR2_CodeReader || driverType == GKDriverType.RSR2_CardReader || driverType == GKDriverType.RSR2_CodeCardReader;
				HasResetFire = device.Driver.AvailableStateBits.Any(x => x == GKStateBit.Fire1 || x == GKStateBit.Fire2);
				HasTest = device.Driver.AvailableStateBits.Any(x => x == GKStateBit.Test);
				HasTurnOn = device.Driver.AvailableStateBits.Any(x => x == GKStateBit.On) && device.Driver.IsControlDevice;
				HasTurnOnNow = device.Driver.AvailableStateBits.Any(x => x == GKStateBit.TurningOn) && device.Driver.IsControlDevice;
				HasTurnOff = device.Driver.AvailableStateBits.Any(x => x == GKStateBit.Off) && device.Driver.IsControlDevice;
				HasTurnOffNow = device.Driver.AvailableStateBits.Any(x => x == GKStateBit.TurningOff) && device.Driver.IsControlDevice;
				HasOnDelay = HasTurnOnNow;
				HasOffDelay = HasTurnOffNow;
				HasHoldDelay = HasTurnOn;
				device.Driver.AvailableStateBits.ForEach(stateBit =>  AddStateBit(stateBit, stateBit == GKStateBit.Norm || stateBit == GKStateBit.Off));
				HasAlarm = driverType == GKDriverType.RSR2_GuardDetectorSound || driverType == GKDriverType.RSR2_GuardDetector || driverType == GKDriverType.RSR2_HandGuardDetector;
				if (device.Driver.IsCardReaderOrCodeReader)
				{
					AddStateBit(GKStateBit.Fire1);
					AddStateBit(GKStateBit.Fire2);
				}
			}

			if (GKBase is GKZone)
			{
				HasAutomaticRegime = true;
				HasManualRegime = false;
				HasIgnoreRegime = true;
				HasReset = true;
				AddStateBit(GKStateBit.Attention);
				AddStateBit(GKStateBit.Fire1);
				AddStateBit(GKStateBit.Fire2);
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
				AddStateBit(GKStateBit.On);
				AddStateBit(GKStateBit.Off, true);
				AddStateBit(GKStateBit.TurningOn);
				AddStateBit(GKStateBit.TurningOff);
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
				AddStateBit(GKStateBit.On);
				AddStateBit(GKStateBit.Off, true);
				AddStateBit(GKStateBit.TurningOn);
				AddStateBit(GKStateBit.TurningOff);
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
				HasTurnOffNow = true;
				AddStateBit(GKStateBit.On);
				AddStateBit(GKStateBit.Off, true);
				AddStateBit(GKStateBit.TurningOn);
				AddStateBit(GKStateBit.TurningOff);
				AddStateBit(GKStateBit.Fire1);
				AddStateBit(GKStateBit.Fire2);
				AddStateBit(GKStateBit.Test);
				HasFire12 = true;
				OnDelay = 1;
				HoldDelay = 1;
				OffDelay = 1;
			}

			if (GKBase is GKDoor)
			{
				HasAutomaticRegime = true;
				HasManualRegime = true;
				HasIgnoreRegime = true;
				HasTurnOn = true;
				HasTurnOffNow = true;
				HasTurnOff = true;
				AddStateBit(GKStateBit.On);
				AddStateBit(GKStateBit.Off, true);
				AddStateBit(GKStateBit.TurningOn);
				AddStateBit(GKStateBit.TurningOff);
				AddStateBit(GKStateBit.Fire1);
				HasHoldDelay = true;
				HasOffDelay = true;
				HasReset = true;
				HasAlarm = true;
			}

			if (GKBase is GKGuardZone)
			{
				HasAutomaticRegime = true;
				HasManualRegime = true;
				HasIgnoreRegime = true;
				HasTurnOn = true;
				HasTurnOnNow = true;
				HasTurnOff = true;
				HasTurnOffNow = true;
				AddStateBit(GKStateBit.On);
				AddStateBit(GKStateBit.Off, true);
				AddStateBit(GKStateBit.TurningOn);
				AddStateBit(GKStateBit.TurningOff);
				AddStateBit(GKStateBit.Attention);
				AddStateBit(GKStateBit.Fire1);
				HasOnDelay = true;
				HasHoldDelay = true;
				HasOffDelay = true;
				HasReset = true;
			}
		}
	}
}