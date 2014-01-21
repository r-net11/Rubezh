using System;
using System.Collections.Generic;
using System.Text;
using ServerFS2.Journal;
using FiresecAPI;
using System.Diagnostics;

namespace ServerFS2.Monitoring
{
	public partial class MonitoringPanel
	{
		const int MaxSequentUnAnswered = 10;
		int SequentUnAnswered = 0;
		int AnsweredCount;
		int UnAnsweredCount;
		bool IsConnectionLost;
		string SerialNo;

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
				CustomMessageJournalHelper.Add("Потеря связи с прибором", null, PanelDevice, null, null, StateType.Failure);
				Trace.WriteLine("OnConnectionLost " + PanelDevice.PresentationAddressAndName);
			}
		}

		public void OnConnectionAppeared()
		{
			if (IsConnectionLost)
			{
				CheckWrongPanel();

				if (!IsInitialized)
				{
					Initialize();
					return;
				}

				IsConnectionLost = false;
				PanelDevice.DeviceState.IsPanelConnectionLost = false;
				DeviceStatesManager.ForseUpdateDeviceStates(PanelDevice);
				CustomMessageJournalHelper.Add("Соединение с прибором восстановленно", null, PanelDevice);
			}
		}

		bool CheckWrongPanel()
		{
			var serialNo = GetSerialNo();
			if (serialNo != null)
			{
				if (SerialNo == null)
				{
					SerialNo = serialNo;
				}
				else
				{
					if (serialNo != SerialNo)
					{
						if (!PanelDevice.DeviceState.IsWrongPanel)
						{
							PanelDevice.DeviceState.IsWrongPanel = true;
							DeviceStatesManager.ForseUpdateDeviceStates(PanelDevice);
							CustomMessageJournalHelper.Add("Несоответствие версий БД с панелью", null, PanelDevice);
							return false;
						}
					}
					else
					{
						if (PanelDevice.DeviceState.IsWrongPanel)
						{
							PanelDevice.DeviceState.IsWrongPanel = false;
							DeviceStatesManager.ForseUpdateDeviceStates(PanelDevice);
							CustomMessageJournalHelper.Add("Несоответствие версий БД с панелью устранено", null, PanelDevice);
							return true;
						}
					}
				}
			}
			return false;
		}

		string GetSerialNo()
		{
			var response = USBManager.Send(PanelDevice, "Запрос серийного номера прибора", 0x01, 0x52, 0x00, 0x00, 0x00, 0xF4, 0x0B);
			if (!response.HasError)
			{
				var result = new string(Encoding.Default.GetChars(response.Bytes.ToArray()));
				return result;
			}
			return null;
		}
	}
}