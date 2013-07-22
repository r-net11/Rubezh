using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FiresecAPI.Models;
using System.Diagnostics;
using System.Collections;

namespace ServerFS2.Monitoring
{
	public static class NonPanelStatesManager
	{
		static Device Panel;
        static BitArray BitArray;
        static DeviceStatesManager DeviceStatesManager;
		
		public static void UpdatePDUPanelState(Device panel, bool isSilent = false)
		{
			Panel = panel;
            Panel.DeviceState.States = new List<DeviceDriverState>();
			var bytes = ServerHelper.GetDeviceStatus(Panel);
            BitArray = new BitArray(new byte[] {bytes[3]});
            DeviceStatesManager = new DeviceStatesManager();
			switch(Panel.Driver.DriverType)
			{
				case DriverType.IndicationBlock:
				case DriverType.PDU_PT:
                    UpdatePDU_PT();	
                    break;

				case DriverType.PDU:
                    UpdatePDU();
                    break;

				case DriverType.UOO_TL:
				case DriverType.MS_3:
				case DriverType.MS_4:
                    UpdateUOOTL();
                    break;
			}
        }

		static void UpdatePDU_PT()
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

        static void UpdatePDU()
        {
            if (BitArray.Count < 2)
                return;
            if (BitArray[0])
                AddStateByName("Потеря связи с устройством");
            if (BitArray[1])
                AddStateByName("Несоответствие версий БД с панелью");
        }

        static void UpdateUOOTL()
        {
            if (BitArray.Count < 6)
                return;
            if (BitArray[0])
                AddStateByName("Неисправность телефонной линии");
            if (BitArray[1])
                AddStateByName("Невозможно доставить сообщение");
            if (BitArray[2])
                AddStateByName("Переполнение журнала событий");
            if (BitArray[3])
                AddStateByName("неисправность линии устранена");
            if (BitArray[4])
                AddStateByName("доставка сообщений восстановлена");
            if (BitArray[5])
                AddStateByName("Потеря связи с прибором");
        }

        static void AddStateByName(string stateName)
        {
            var driverState = ConfigurationManager.DriversConfiguration.Drivers.FirstOrDefault(x => x.DriverType == Panel.Driver.DriverType).States.FirstOrDefault(x => x != null && x.Name == stateName);
            if (driverState == null)
                return;
            var deviceDriverState = new DeviceDriverState()
			{
				DriverState = driverState,
				Time = DateTime.Now
			};
			Panel.DeviceState.States.Add(deviceDriverState);
			DeviceStatesManager.ForseUpdateDeviceStates(Panel);
        }
	}
}