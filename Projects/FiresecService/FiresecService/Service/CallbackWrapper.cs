using System;
using System.Collections.Generic;
using Common;
using FiresecAPI.Models;
using FiresecService.ViewModels;

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
			SafeCall((x) => { x.FiresecCallbackService.NewJournalRecord(journalRecord); }, "OnNewJournalRecord");
		}

		public void OnDeviceStatesChanged(List<DeviceState> deviceStates)
		{
			SafeCall((x) => { x.FiresecCallbackService.DeviceStateChanged(deviceStates); }, "OnDeviceStatesChanged");
		}

		public void OnDeviceParametersChanged(List<DeviceState> deviceParameters)
		{
			SafeCall((x) => { x.FiresecCallbackService.DeviceParametersChanged(deviceParameters); }, "OnDeviceParametersChanged");
		}

		public void OnZoneStateChanged(ZoneState zoneState)
		{
			SafeCall((x) => { x.FiresecCallbackService.ZoneStateChanged(zoneState); }, "OnZoneStateChanged");
		}

		public void OnConfigurationChanged()
		{
			SafeCall((x) => { x.FiresecCallbackService.ConfigurationChanged(); }, "OnConfigurationChanged");
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

		void SafeCall(Action<FiresecService> action, string actionName, bool reconnectOnException = true)
		{
			if (FiresecService.IsSubscribed)
			{
				MainViewModel.Current.UpdateCallbackOperation(FiresecService.UID, actionName);
				try
				{
					action(FiresecService);
					return;
				}
				catch (System.ServiceModel.CommunicationObjectFaultedException)
				{
					Logger.Error("Исключение CommunicationObjectFaultedException при вызове CallbackWrapper.SafeCall." + actionName);
				}
				catch (System.ServiceModel.CommunicationException)
				{
					Logger.Error("Исключение CommunicationException при вызове CallbackWrapper.SafeCall." + actionName);
				}
				catch (Exception e)
				{
					Logger.Error(e, "Исключение при вызове CallbackWrapper.SafeCall");
				}
				if (reconnectOnException)
				{
					if (FiresecService.ReconnectToClient())
						SafeCall(action, actionName + "Повторный вызов", false);
				}
			}
		}
	}
}