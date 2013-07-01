using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using FiresecAPI.Models;
using FS2Api;
using ServerFS2.Journal;
using ServerFS2.Service;
using System.Diagnostics;
using ServerFS2.Operations;

namespace ServerFS2.Monitoring
{
	public partial class MonitoringPanel
	{
		int SequentUnAnswered = 0;
		int AnsweredCount;
		int UnAnsweredCount;

		void CheckConnectionLost()
		{
			var requestsToDelete = new List<Request>();
			lock (Locker)
			{
				foreach (var request in Requests)
				{
					if (request != null && (DateTime.Now - request.StartTime).TotalSeconds >= RequestExpiredTime)
					{
						requestsToDelete.Add(request);
						UnAnsweredCount++;
						SequentUnAnswered++;
					}
				}
				requestsToDelete.ForEach(x => Requests.Remove(x));
			}
			if (SequentUnAnswered > MaxSequentUnAnswered)
			{
				OnConnectionLost();
			}
		}

		public void OnConnectionLost()
		{
			if (!IsConnectionLost)
			{
				IsConnectionLost = true;
				PanelDevice.DeviceState.IsPanelConnectionLost = true;
				DeviceStatesManager.ForseUpdateDeviceStates(PanelDevice);
				OnConnectionChanged();
				OnNewJournalItem(JournalParser.CustomJournalItem(PanelDevice, "Потеря связи с прибором"));
			}
		}

		public void OnConnectionAppeared()
		{
			if (IsConnectionLost)
			{
				//var serialNo = PanelDevice.Properties.FirstOrDefault(x => x.Name == "SerialNo").Value;
				//GetInformationOperationHelper.GetDeviceInformation(PanelDevice);
				//if (PanelDevice.Properties.FirstOrDefault(x => x.Name == "SerialNo").Value == null)
				//    return;
				//if (PanelDevice.Properties.FirstOrDefault(x => x.Name == "serialNo").Value != serialNo)
				//{
				//    OnWrongPanel();
				//    return;
				//}

				if (!IsInitialized)
				{
					Initialize();
					return;
				}

				IsConnectionLost = false;
				PanelDevice.DeviceState.IsPanelConnectionLost = false;
				DeviceStatesManager.ForseUpdateDeviceStates(PanelDevice);
				OnConnectionChanged();
				OnNewJournalItem(JournalParser.CustomJournalItem(PanelDevice, "Связь с прибором восстановлена"));
			}
		}

		public event Action ConnectionChanged;
		void OnConnectionChanged()
		{
			if (ConnectionChanged != null)
				ConnectionChanged();
		}

		void OnWrongPanel()
		{
			var deviceStatesManager = new DeviceStatesManager();
			var deviceStates = new List<DeviceState>();
			var panelState = PanelDevice.Driver.States.FirstOrDefault(y => y.Name == "Несоответствие версий БД с панелью");
			PanelDevice.DeviceState.IsWrongPanel = true;
			deviceStatesManager.ForseUpdateDeviceStates(PanelDevice);
			foreach (var device in PanelDevice.GetRealChildren())
			{
				if (!device.DeviceState.ParentStates.Any(x => x.DriverState.Id == panelState.Id))
				{
					var parentDeviceState = new ParentDeviceState()
					{
						ParentDeviceUID = device.ParentPanel.UID,
						DriverState = panelState
					};
					device.DeviceState.ParentStates.Add(parentDeviceState);
				}

				device.DeviceState.IsWrongPanel = true;
				deviceStates.Add(device.DeviceState);
			}
			CallbackManager.DeviceStateChanged(deviceStates);
			OnNewJournalItem(JournalParser.CustomJournalItem(PanelDevice, "Несоответствие версий БД с панелью"));
		}
	}
}
