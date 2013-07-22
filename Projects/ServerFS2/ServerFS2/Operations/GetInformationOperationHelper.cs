using System.Linq;
using System.Text;
using FiresecAPI.Models;

namespace ServerFS2.Operations
{
	public static class GetInformationOperationHelper
	{
		public static string GetDeviceInformation(Device device)
		{
			string serialNo;
			var result = "";
			var driverType = device.Driver.DriverType;

			if (driverType == DriverType.MS_1 || driverType == DriverType.MS_2)
			{
				var driverTypeBytes = USBManager.Send(device, 0x01, 0x04).MsFlag;
				result += "Тип устройства: " + (driverTypeBytes == 0x41 ? FiresecAPI.Models.DriversHelper.DriverDataList.FirstOrDefault(x => x.DriverType == DriverType.MS_2).Name : FiresecAPI.Models.DriversHelper.DriverDataList.FirstOrDefault(x => x.DriverType == DriverType.MS_1).Name) + "\n";

				var serialNoBytes = USBManager.Send(device, 0x01, 0x32).Bytes;
				serialNo = new string(Encoding.Default.GetChars(serialNoBytes.ToArray()));
				result += "Заводской номер: " + serialNo + "\n";
			}

			else if (driverType == DriverType.IndicationBlock || driverType == DriverType.PDU || driverType == DriverType.PDU_PT)
			{
				var driverTypeBytes = USBManager.Send(device, 0x01, 0x03).Bytes;
				result += "Тип устройства: " + DriversHelper.DriverDataList.FirstOrDefault(x => x.DriverCode == driverTypeBytes[0]).Name + "\n";

				var serialNoBytes = USBManager.Send(device, 0x01, 0x32).Bytes;
				serialNo = new string(Encoding.Default.GetChars(serialNoBytes.ToArray()));
				result += "Заводской номер: " + serialNo + "\n";

				var addressBytes = USBManager.Send(device, 0x01, 0x31).Bytes;
				result += "Адрес: " + addressBytes[2] + "\n";
				result += "Скорость: ";
				switch (addressBytes[1])
				{
					case 0: result += "9600\n"; break;
					case 1: result += "19200\n"; break;
					case 2: result += "38400\n"; break;
					case 3: result += "57600\n"; break;
					case 4: result += "115200\n"; break;
				}

			}
			else
			{
				var driverTypeBytes = USBManager.Send(device, 0x01, 0x03).Bytes;
				result += "Тип устройства: " + DriversHelper.DriverDataList.FirstOrDefault(x => x.DriverCode == driverTypeBytes[0]).Name + "\n";

				var serialNoBytes = USBManager.Send(device, 0x01, 0x52, 0x00, 0x00, 0x00, 0xF4, 0x0B).Bytes;
				serialNo = new string(Encoding.Default.GetChars(serialNoBytes.ToArray()));
				result += "Заводской номер: " + serialNo + "\n";

				var panelDatabaseReader = new ReadPanelDatabaseOperationHelper(device, false);
				var bdVersionBytes = ServerHelper.GetBytesFromRomDB(device, panelDatabaseReader.GetRomFirstIndex(device) + 4, 2);
				string bdVersion = bdVersionBytes[0].ToString("X") + "." + bdVersionBytes[1].ToString("X");

				result += "Версия базы: " + bdVersion + "\n";
			}
			var softVersionBytes = USBManager.Send(device, 0x01, 0x12).Bytes;
			string softVersion = softVersionBytes[0].ToString("X") + "." + softVersionBytes[1].ToString("X");
			result += "Версия микропрограммы: " + softVersion;

			return result;
		}
	}
}