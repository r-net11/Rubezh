using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using XFiresecAPI;

namespace FiresecAPI
{
	public interface IGKService
	{
		void Start();
		void Stop();
		void SetNewConfiguration(XDeviceConfiguration deviceConfiguration);
		void ExecuteDeviceCommand(XDevice device, XStateBit stateType);
		void Reset(XBase xBase);
		void ResetFire1(XZone zone);
		void ResetFire2(XZone zone);
		void SetAutomaticRegime(XBase xBase);
		void SetManualRegime(XBase xBase);
		void SetIgnoreRegime(XBase xBase);
		void TurnOn(XBase xBase);
		void TurnOnNow(XBase xBase);
		void TurnOff(XBase xBase);
		void TurnOffNow(XBase xBase);
		void Stop(XBase xBase);
	}
}