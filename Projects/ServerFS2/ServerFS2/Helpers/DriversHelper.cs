using System;
using System.Collections.Generic;
using System.Linq;
using FiresecAPI.Models;

namespace ServerFS2
{
	public static class DriversHelper
	{
		public class DriverData
		{
			public DriverData(string driverUid, string name, byte driverType)
			{
				Name = name;
				DriverUid = driverUid;
				DriverType = driverType;
			}

			public string Name { get; private set; }
			public string DriverUid { get; private set; }
			public byte DriverType { get; private set; }
		}

		public static string GetDriverNameByType(byte driverType)
		{
			return DriverDataList.FirstOrDefault(x => x.DriverType == driverType).Name;
		}

		public static Guid GetDriverUidByType(byte driverType)
		{
			return new Guid(DriverDataList.FirstOrDefault(x => x.DriverType == driverType).DriverUid);
		}

		public static List<DriverData> DriverDataList { get; private set; }
		static DriversHelper()
		{
			DriverDataList = new List<DriverData>();
			DriverDataList.Add(new DriverData("B476541B-5298-4B3E-A9BA-605B839B1011", "Прибор Рубеж-2AM", 1));
			DriverDataList.Add(new DriverData("02CE2CC4-D71F-4EAA-ACCC-4F2E870F548C", "БУНС", 2));
			DriverDataList.Add(new DriverData("F966D47B-468D-40A5-ACA7-9BE30D0A3847", "Модуль сопряжения МС-3", 100));
			DriverDataList.Add(new DriverData("F966D47B-468D-40A5-ACA7-9BE30D0A3847", "Модуль сопряжения МС-4", 101));
			DriverDataList.Add(new DriverData("584BC59A-28D5-430B-90BF-592E40E843A6", "Модуль сопряжения МС-ТЛ", 102));
			DriverDataList.Add(new DriverData("28A7487A-BA32-486C-9955-E251AF2E9DD4", "Блок индикации БИ", 3));
			DriverDataList.Add(new DriverData("E750EF8F-54C3-4B00-8C72-C7BEC9E59BFC", "Прибор Рубеж-10AM", 4));
			DriverDataList.Add(new DriverData("F3485243-2F60-493B-8A4E-338C61EF6581", "Рубеж-4А", 5));
			DriverDataList.Add(new DriverData("96CDBD7E-29F6-45D4-9028-CF10332FAB1A", "Прибор РУБЕЖ-2ОП", 6));
			DriverDataList.Add(new DriverData("B1DF571E-8786-4987-94B2-EC91F7578D20", "Пульт дистанционного управления ПДУ", 7));
			DriverDataList.Add(new DriverData("A7BB2FD0-0088-49AE-8C04-7D6FA22C79D6", "БУНС-2", 8));
			DriverDataList.Add(new DriverData("07BEB3DD-7D14-41F8-B8BC-FE8EDD215762", "Пульт дистанционного управления ПДУ-ПТ", 9));
		}

		public static DriverType GetUsbDriverTypeByTypeNo(int driverTypeNo)
		{
			switch (driverTypeNo)
			{
				case 1:
					return DriverType.USB_Rubezh_2AM;
				case 6:
					return DriverType.USB_Rubezh_2OP;
				case 5:
					return DriverType.USB_Rubezh_4A;
				case 2:
					return DriverType.USB_BUNS;
				case 8:
					return DriverType.USB_BUNS_2;
				case 3:
					return DriverType.IndicationBlock;
				case 7:
					return DriverType.PDU;
				case 9:
					return DriverType.PDU_PT;
				case 10:
					return DriverType.USB_Rubezh_P;
				case 4:
					return DriverType.Rubezh_10AM;
				case 98:
					return DriverType.MS_1;
				case 99:
					return DriverType.MS_2;
				case 100:
					return DriverType.MS_3;
				case 101:
					return DriverType.MS_4;
				case 102:
					return DriverType.UOO_TL;
			}
			return DriverType.Computer;
		}

		public static int GetTypeNoByDriverType(DriverType driverType)
		{
			switch (driverType)
			{
				case DriverType.USB_Rubezh_2AM:
					return 1;
				case DriverType.USB_Rubezh_2OP:
					return 6;
				case DriverType.USB_Rubezh_4A:
					return 5;
				case DriverType.USB_BUNS:
					return 2;
				case DriverType.USB_BUNS_2:
					return 8;
				case DriverType.IndicationBlock:
					return 3;
				case DriverType.PDU:
					return 7;
				case DriverType.PDU_PT:
					return 9;
				case DriverType.USB_Rubezh_P:
					return 10;
				case DriverType.Rubezh_10AM:
					return 4;
				case DriverType.MS_1:
					return 98;
				case DriverType.MS_2:
					return 99;
				case DriverType.MS_3:
					return 100;
				case DriverType.MS_4:
					return 101;
				case DriverType.UOO_TL:
					return 102;
			}
			return -1;
		}
	}
}