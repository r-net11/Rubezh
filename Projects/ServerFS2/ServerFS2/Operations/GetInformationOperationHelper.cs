using System.Linq;
using System.Text;
using FiresecAPI.Models;
using FS2Api;

namespace ServerFS2.Operations
{
	public static class GetInformationOperationHelper
	{
		public static string GetDeviceInformation(Device device)
		{
			string serialNo;
			var result = "";
			var driverType = device.Driver.DriverType;
			Response response = null;

			if (driverType == DriverType.MS_1 || driverType == DriverType.MS_2)
			{
				response = USBManager.Send(device, "Запрос типа устройства", 0x01, 0x04);
				if (response.HasError)
					throw new FS2Exception("USB устройство отсутствует");
				result += "Тип устройства: " + (response.MsFlag == 0x41 ? FiresecAPI.Models.DriversHelper.DriverDataList.FirstOrDefault(x => x.DriverType == DriverType.MS_2).Name : FiresecAPI.Models.DriversHelper.DriverDataList.FirstOrDefault(x => x.DriverType == DriverType.MS_1).Name) + "\n";

				response = USBManager.Send(device, "Запрос серийного номера", 0x01, 0x32);
				if (response.HasError)
					throw new FS2Exception("USB устройство отсутствует");
				serialNo = new string(Encoding.Default.GetChars(response.Bytes.ToArray()));
				result += "Заводской номер: " + serialNo + "\n";
			}

			else if (driverType == DriverType.IndicationBlock || driverType == DriverType.PDU || driverType == DriverType.PDU_PT)
			{
				response = USBManager.Send(device, "Запрос типа устройства", 0x01, 0x03);
				if (response.HasError)
					throw new FS2Exception("USB устройство отсутствует");
				result += "Тип устройства: " + DriversHelper.DriverDataList.FirstOrDefault(x => x.DriverCode == response.Bytes[0]).Name + "\n";

				response = USBManager.Send(device, "Запрос серийного номера", 0x01, 0x32);
				if (response.HasError)
					throw new FS2Exception("USB устройство отсутствует");
				serialNo = new string(Encoding.Default.GetChars(response.Bytes.ToArray()));
				result += "Заводской номер: " + serialNo + "\n";

				response = USBManager.Send(device, "Запрос адресного листа", 0x01, 0x31);
				if (response.HasError)
					throw new FS2Exception("USB устройство отсутствует");
				result += "Адрес: " + response.Bytes[2] + "\n";
				result += "Скорость: ";
				switch (response.Bytes[1])
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
				response = USBManager.Send(device, "Запрос типа устройства", 0x01, 0x03);
				if (response.HasError)
					throw new FS2Exception("USB устройство отсутствует");
				result += "Тип устройства: " + DriversHelper.DriverDataList.FirstOrDefault(x => x.DriverCode == response.Bytes[0]).Name + "\n";

				response = USBManager.Send(device, "Запрос серийного номера", 0x01, 0x52, 0x00, 0x00, 0x00, 0xF4, 0x0B);
				if (response.HasError)
					throw new FS2Exception("USB устройство отсутствует");
				serialNo = new string(Encoding.Default.GetChars(response.Bytes.ToArray()));
				result += "Заводской номер: " + serialNo + "\n";

				var panelDatabaseReader = new ReadPanelDatabaseOperationHelper(device, false);
				var bdVersionBytes = ServerHelper.GetBytesFromRomDB(device, panelDatabaseReader.GetRomFirstIndex(device) + 4, 2);
				if (bdVersionBytes == null)
					throw new FS2Exception("USB устройство отсутствует");
				string bdVersion = bdVersionBytes[0].ToString("X") + "." + bdVersionBytes[1].ToString("X");

				result += "Версия базы: " + bdVersion + "\n";
			}
			response = USBManager.Send(device, "Запрос версии ПО", 0x01, 0x12);
			if (response.HasError)
				throw new FS2Exception("USB устройство отсутствует");
			string softVersion = response.Bytes[0].ToString("X") + "." + response.Bytes[1].ToString("X");
			result += "Версия микропрограммы: " + softVersion;

			return result;
		}
	}
}