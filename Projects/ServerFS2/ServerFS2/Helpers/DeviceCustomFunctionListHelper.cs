using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FiresecAPI.Models;

namespace ServerFS2
{
	public static class DeviceCustomFunctionListHelper
	{
		public static List<DeviceCustomFunction> GetDeviceCustomFunctionList(DriverType driverType)
		{
			switch (driverType)
			{
				case DriverType.Rubezh_2AM:
				case DriverType.USB_Rubezh_2AM:
					return new List<DeviceCustomFunction>()
					{
						new DeviceCustomFunction()
						{
							Code = "Set_BlindMode",
							Name = "Установить режим \"глухой панели\"",
							Description = "Установить режим \"глухой панели\"",
						},
						new DeviceCustomFunction()
						{
							Code = "Reset_BlindMode",
							Name = "Снять режим \"глухой панели\"",
							Description = "Снять режим \"глухой панели\"",
						}
					};
				case DriverType.IndicationBlock:
				case DriverType.PDU:
				case DriverType.PDU_PT:
					return new List<DeviceCustomFunction>()
					{
						new DeviceCustomFunction()
						{
							Code = "Touch_SetMaster",
							Name = "Записать мастер-ключ",
							Description = "Записать мастер-ключ TouchMemory",
						},
						new DeviceCustomFunction()
						{
							Code = "Touch_ClearMaster",
							Name = "Стереть пользовательские ключи",
							Description = "Стереть пользовательские ключи TouchMemory",
						},
						new DeviceCustomFunction()
						{
							Code = "Touch_ClearAll",
							Name = "Стереть все ключи",
							Description = "Стереть все ключи TouchMemory",
						}
					};
			}
			return new List<DeviceCustomFunction>();
		}
	}
}