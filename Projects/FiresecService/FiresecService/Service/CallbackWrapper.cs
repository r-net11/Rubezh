using System;
using System.Collections.Generic;
using Common;
using FiresecAPI.Models;
using FiresecService.ViewModels;
using FiresecAPI;

namespace FiresecService.Service
{
	public class CallbackWrapper : IFiresecCallbackService
	{
		FiresecService FiresecService;

		public CallbackWrapper(FiresecService firesecService)
		{
			FiresecService = firesecService;
		}

		public void NewJournalRecord(JournalRecord journalRecord)
		{
			SafeCall((x) => { x.FiresecCallbackService.NewJournalRecord(journalRecord); }, "NewJournalRecord");
		}

		public void DeviceStateChanged(List<DeviceState> deviceStates)
		{
			SafeCall((x) => { x.FiresecCallbackService.DeviceStateChanged(deviceStates); }, "DeviceStatesChanged");
		}

		public void DeviceParametersChanged(List<DeviceState> deviceParameters)
		{
			SafeCall((x) => { x.FiresecCallbackService.DeviceParametersChanged(deviceParameters); }, "DeviceParametersChanged");
		}

		public void ZoneStateChanged(ZoneState zoneState)
		{
			SafeCall((x) => { x.FiresecCallbackService.ZoneStateChanged(zoneState); }, "ZoneStateChanged");
		}

		public void ConfigurationChanged()
		{
			SafeCall((x) => { x.FiresecCallbackService.ConfigurationChanged(); }, "ConfigurationChanged");
		}

		public Guid Ping()
		{
			try
			{
				return FiresecService.FiresecCallbackService.Ping();
			}
			catch
			{
				FiresecService.ReconnectToClient();
			}
			return Guid.Empty;
		}

		public void GetFilteredArchiveCompleted(IEnumerable<JournalRecord> journalRecords)
		{
			SafeCall((x) => { x.FiresecCallbackService.GetFilteredArchiveCompleted(journalRecords); }, "GetFilteredArchiveCompleted");
		}

		public bool Progress(int stage, string comment, int percentComplete, int bytesRW)
		{
			try
			{
				var result = FiresecService.FiresecCallbackService.Progress(stage, comment, percentComplete, bytesRW);
				if (FiresecService.ContinueProgress == false)
				{
					FiresecService.ContinueProgress = true;
					return false;
				}
				return result;
			}
			catch (Exception e)
			{
				Logger.Error(e, "Исключение при вызове CallbackWrapper.Progress");
				FiresecService.ReconnectToClient();
			}
			return true;
		}

		void SafeCall(Action<FiresecService> action, string actionName, bool reconnectOnException = true)
		{
			if (FiresecService.IsSubscribed)
			{
				MainViewModel.Current.BeginAddOperation(FiresecService.UID, OperationDirection.ServerToClient, actionName);
				try
				{
					action(FiresecService);
					MainViewModel.Current.EndAddOperation(FiresecService.UID, OperationDirection.ServerToClient);
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
				MainViewModel.Current.EndAddOperation(FiresecService.UID, OperationDirection.ServerToClient);
				if (reconnectOnException)
				{
					if (FiresecService.ReconnectToClient())
						SafeCall(action, actionName + "Повторный вызов", false);
				}
			}
		}
	}
}