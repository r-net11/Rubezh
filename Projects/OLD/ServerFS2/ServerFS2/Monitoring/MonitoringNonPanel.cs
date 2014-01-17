using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FiresecAPI.Models;
using System.Collections;
using ServerFS2.Journal;
using System.Diagnostics;
using FiresecAPI;

namespace ServerFS2.Monitoring
{
	public class MonitoringNonPanel
	{
		public Device PanelDevice { get; private set; }
		BitArray BitArray;
		DeviceStatesManager DeviceStatesManager;
		public bool IsInitialized { get; private set; }
		bool IsConnectionLost;

		public MonitoringNonPanel(Device device)
		{
			PanelDevice = device;
		}

		public bool Initialize()
		{
			DeviceStatesManager = new DeviceStatesManager();
			IsInitialized = IsHashEqual();
			if (!IsInitialized)
			{
				PanelDevice.DeviceState.IsDBMissmatch = true;
				DeviceStatesManager.ForseUpdateDeviceStates(PanelDevice);
				return false;
			}
			else
			{
				PanelDevice.DeviceState.IsDBMissmatch = false;
				DeviceStatesManager.ForseUpdateDeviceStates(PanelDevice);
			}

			return true;
		}

		bool IsHashEqual()
		{
			return false;
		}

		public void CheckState()
		{
			DeviceStatesManager = new DeviceStatesManager();
			PanelDevice.DeviceState.States = new List<DeviceDriverState>();
			var statusBytes = ServerHelper.GetPanelStatus(PanelDevice, false);
			if (statusBytes != null && statusBytes.Count == 4)
			{
				BitArray = new BitArray(new byte[] { statusBytes[3] });
				switch (PanelDevice.Driver.DriverType)
				{
					case DriverType.IndicationBlock:
					case DriverType.PDU_PT:
						UpdatePDU_PT();
						break;

					case DriverType.PDU:
						UpdatePDU_OrBI();
						break;

					case DriverType.UOO_TL:
					case DriverType.MS_3:
					case DriverType.MS_4:
						UpdateUOOTL();
						break;
				}
				OnConnectionAppeared();
			}
			else
			{
				OnConnectionLost();
			}
		}

		void UpdatePDU_OrBI()
		{
			if (BitArray.Count < 2)
				return;
			if (BitArray[0])
				AddStateByName("Потеря связи с устройством");
			if (BitArray[1])
				AddStateByName("Несоответствие версий БД с панелью");
		}

		void UpdatePDU_PT()
		{
			if (BitArray.Count < 6)
				return;
			if (BitArray[0])
				AddStateByName("Потеря связи с устройством");
			if (BitArray[1])
				AddStateByName("Несоответствие версий БД с панелью");
			if (BitArray[2])
				AddStateByName("Клавиатура заблокирована");
			if (!BitArray[3])
				AddStateByName("Авария питания 1");
			if (!BitArray[4])
				AddStateByName("Авария питания 2");
		}

		void UpdateUOOTL()
		{
			if (BitArray.Count < 6)
				return;
			if (BitArray[0])
				AddStateByName("Неисправность телефонной линии");
			if (BitArray[1])
				AddStateByName("Невозможно доставить сообщение");
			if (BitArray[2])
				AddStateByName("Переполнение журнала событий");
			//if (BitArray[3])
			//    AddStateByName("неисправность линии устранена");
			//if (BitArray[4])
			//    AddStateByName("доставка сообщений восстановлена");
			if (BitArray[5])
				AddStateByName("Потеря связи с прибором");
		}

		void AddStateByName(string stateName)
		{
			var driverState = ConfigurationManager.DriversConfiguration.Drivers.FirstOrDefault(x => x.DriverType == PanelDevice.Driver.DriverType).States.FirstOrDefault(x => x != null && x.Name == stateName);
			if (driverState == null)
				return;
			var deviceDriverState = new DeviceDriverState()
			{
				DriverState = driverState,
				Time = DateTime.Now
			};
			PanelDevice.DeviceState.States.Add(deviceDriverState);
			DeviceStatesManager.ForseUpdateDeviceStates(PanelDevice);
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
	}
}