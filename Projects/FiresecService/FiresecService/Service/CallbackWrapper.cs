using System;
using System.Collections.Generic;
using Common;
using FiresecAPI.Models;

namespace FiresecService.Service
{
	public class CallbackWrapper
	{
		FiresecService FiresecService;

		public CallbackWrapper(FiresecService firesecService)
		{
			FiresecService = firesecService;
		}

		public void OnNewJournalRecord(JournalRecord journalRecord)
		{
			SafeCall((x) => { x.FiresecCallbackService.NewJournalRecord(journalRecord); });
		}

		public void OnDeviceStatesChanged(List<DeviceState> deviceStates)
		{
			SafeCall((x) => { x.FiresecCallbackService.DeviceStateChanged(deviceStates); });
		}

		public void OnDeviceParametersChanged(List<DeviceState> deviceParameters)
		{
			SafeCall((x) => { x.FiresecCallbackService.DeviceParametersChanged(deviceParameters); });
		}

		public void OnZoneStateChanged(ZoneState zoneState)
		{
			SafeCall((x) => { x.FiresecCallbackService.ZoneStateChanged(zoneState); });
		}

		public void OnConfigurationChanged()
		{
			SafeCall((x) => { x.FiresecCallbackService.ConfigurationChanged(); });
		}

		public void OnPing()
		{
			try
			{
				FiresecService.FiresecCallbackService.Ping();
			}
			catch
			{
				FiresecService.ReconnectToClient();
			}
		}
			

		public bool OnProgress(int stage, string comment, int percentComplete, int bytesRW)
		{
			try
			{
				var result = FiresecService.FiresecCallbackService.Progress(stage, comment, percentComplete, bytesRW);
				return result;
			}
			catch (Exception e)
			{
				Logger.Error(e, "Исключение при вызове CallbackWrapper.OnProgress");
				FiresecService.ReconnectToClient();
			}
			return true;
		}

		void SafeCall(Action<FiresecService> action, bool reconnectOnException = true)
		{
			if (FiresecService.IsSubscribed)
				try
				{
					action(FiresecService);
				}
				catch (Exception e)
				{
					Logger.Error(e, "Исключение при вызове CallbackWrapper.SafeCall");
					if (reconnectOnException)
					{
						if (FiresecService.ReconnectToClient())
							SafeCall(action, false);
					}
				}
		}
	}
}