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
			var bytes = ServerHelper.GetDeviceStatus(Panel);
            BitArray = new BitArray(bytes.ToArray());
            DeviceStatesManager = new DeviceStatesManager();
			switch(Panel.Driver.DriverType)
			{
				case DriverType.IndicationBlock:
				case DriverType.PDU_PT:
					break;

				case DriverType.PDU:
					break;

				case DriverType.UOO_TL:
				case DriverType.MS_3:
				case DriverType.MS_4:
					break;
			}
			//Trace.WriteLine(panel.PresentationAddressAndName + " " + BytesHelper.BytesToString(bytes));
		}

		static void UpdatePDU_PT()
		{
            if (BitArray.Count < 6)
                return;
            if (BitArray[0])
                AddStateByName("потеря связи с прибором");
            else if (BitArray[1])
                AddStateByName("БД устарела");
            else if (BitArray[2])
                AddStateByName("клавиатура заблокирована");
            else if (BitArray[3])
                AddStateByName("питание 1 в порядке");
            else if (BitArray[4])
                AddStateByName("питание 2 в порядке");
            else if (BitArray[5])
                AddStateByName("вскрытие корпуса");
		}

        static void UpdatePDU()
        {
            if (BitArray.Count < 2)
                return;
            if (BitArray[0])
                AddStateByName("потеря связи с прибором");
            else if (BitArray[1])
                AddStateByName("БД устарела");
        }

        static void UpdateUOOTL()
        {
            if (BitArray.Count < 6)
                return;
            if (BitArray[0])
                AddStateByName("неисправность линии");
            else if (BitArray[1])
                AddStateByName("невозможность доставить сообщение");
            else if (BitArray[2])
                AddStateByName("переполнение журнала сообщений");
            else if (BitArray[3])
                AddStateByName("неисправность линии устранена");
            else if (BitArray[4])
                AddStateByName("доставка сообщений восстановлена");
            else if (BitArray[5])
                AddStateByName("потеря связи с прибором");
        }

        static void AddStateByName(string stateName)
        {
            var driverState = new DriverState { Name = stateName };
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