using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using FiresecAPI;
using FiresecAPI.Models;
using ServerFS2.Helpers;
using Device = FiresecAPI.Models.Device;

namespace ServerFS2
{
	public static class ServerHelper
	{
		static readonly object Locker = new object();
		static readonly UsbRunner UsbRunner;
		public static event Action<int, int, string> Progress;
		public static List<Driver> Drivers;
		public static List<byte> DeviceRam;
		public static List<byte> DeviceRom;

		static ServerHelper()
		{
			var str = DateConverter.ConvertToBytes(DateTime.Now);
			MetadataHelper.Initialize();
			ConfigurationManager.Load();
			Drivers = ConfigurationManager.DriversConfiguration.Drivers;
			UsbRunner = new UsbRunner();
			UsbRunner.Open();
		}

		public static void Initialize()
		{
			;
		}

		private static int _usbRequestNo;

		public static void ParseJournal(List<byte> bytes, List<JournalItem> journalItems)
		{
			lock (Locker)
			{
				var journalParser = new JournalParser(bytes);
				var journalItem = journalParser.Parce();
				journalItems.Add(journalItem);
			}
		}

		public static OperationResult<List<Response>> SendCode(List<List<byte>> bytesList, int maxDelay = 1000, int maxTimeout = 1000)
		{
			return UsbRunner.AddRequest(bytesList, maxDelay, maxTimeout);
		}

		public static OperationResult<List<Response>> SendCode(List<byte> bytes, int maxDelay = 1000, int maxTimeout = 1000)
		{
			return UsbRunner.AddRequest(new List<List<byte>> { bytes }, maxDelay, maxTimeout);
		}

		public static List<JournalItem> GetSecJournalItems2Op(Device device)
		{
			int lastindex = GetLastSecJournalItemId2Op(device);
			var journlaItems = new List<JournalItem>();
			for (int i = 0; i <= lastindex; i++)
			{
				var bytes = new List<byte>();
				bytes.AddRange(BitConverter.GetBytes(++_usbRequestNo).Reverse());
				bytes.Add(Convert.ToByte(device.Parent.IntAddress + 2));
				bytes.Add(Convert.ToByte(device.IntAddress % 256));
				bytes.Add(0x01);
				bytes.Add(0x20);
				bytes.Add(0x02);
				bytes.AddRange(BitConverter.GetBytes(i).Reverse());
				ParseJournal(SendCode(bytes).Result.FirstOrDefault().Data, journlaItems);
			}
			journlaItems = journlaItems.OrderByDescending(x => x.IntDate).ToList();
			int no = 0;
			foreach (var item in journlaItems)
			{
				no++;
				item.No = no;
			}
			return journlaItems;
		}

		public static int GetLastSecJournalItemId2Op(Device device)
		{
			var bytes = new List<byte>();
			bytes.AddRange(BitConverter.GetBytes(++_usbRequestNo).Reverse());
			bytes.Add(Convert.ToByte(device.Parent.IntAddress + 2));
			bytes.Add(Convert.ToByte(device.IntAddress % 256));
			bytes.Add(0x01);
			bytes.Add(0x21);
			bytes.Add(0x02);
			try
			{
				var lastindex = SendCode(bytes);
				int li = 256 * lastindex.Result.FirstOrDefault().Data[9] + lastindex.Result.FirstOrDefault().Data[10];
				return li;
			}
			catch (NullReferenceException ex)
			{
				MessageBox.Show(ex.Message);
				throw;
			}
		}

		public static int GetJournalCount(Device device)
		{
			var bytes = new List<byte>();
			bytes.AddRange(BitConverter.GetBytes(++_usbRequestNo).Reverse());
			bytes.Add(Convert.ToByte(device.Parent.IntAddress + 2));
			bytes.Add(Convert.ToByte(device.IntAddress % 256));
			bytes.Add(0x01);
			bytes.Add(0x24);
			bytes.Add(0x01);
			try
			{
				var firecount = SendCode(bytes);
				int fc = 256 * firecount.Result.FirstOrDefault().Data[7] + firecount.Result.FirstOrDefault().Data[8];
				return fc;
			}
			catch (NullReferenceException ex)
			{
				MessageBox.Show(ex.Message);
				throw;
			}
		}

		public static int GetFirstJournalItemId(Device device)
		{
			var li = GetLastJournalItemId(device);
			var count = GetJournalCount(device);
			return li - count + 1;
		}

		public static int GetLastJournalItemId(Device device)
		{
			var bytes = new List<byte>();
			bytes.AddRange(BitConverter.GetBytes(++_usbRequestNo).Reverse());
			bytes.Add(Convert.ToByte(device.Parent.IntAddress + 2));
			bytes.Add(Convert.ToByte(device.IntAddress % 256));
			bytes.Add(0x01);
			bytes.Add(0x21);
			bytes.Add(0x00);
			try
			{
				var lastindex = SendCode(bytes);
				int li = 256 * lastindex.Result.FirstOrDefault().Data[9] + lastindex.Result.FirstOrDefault().Data[10];
				return li;
			}
			catch (NullReferenceException ex)
			{
				MessageBox.Show(ex.Message);
				throw;
			}
		}

		public static List<JournalItem> GetJournalItems(Device device)
		{
			int lastindex = GetLastJournalItemId(device);
			int firstindex = GetFirstJournalItemId(device);
			var journalItems = new List<JournalItem>();
			var secJournalItems = new List<JournalItem>();
			if (device.PresentationName == "Прибор РУБЕЖ-2ОП")
			{
				secJournalItems = GetSecJournalItems2Op(device);
			}
			for (int i = firstindex; i <= lastindex; i++)
			{
				var bytes = new List<byte>();
				bytes.AddRange(BitConverter.GetBytes(++_usbRequestNo).Reverse());
				bytes.Add(Convert.ToByte(device.Parent.IntAddress + 2));
				bytes.Add(Convert.ToByte(device.IntAddress % 256));
				bytes.Add(0x01);
				bytes.Add(0x20);
				bytes.Add(0x00);
				bytes.AddRange(BitConverter.GetBytes(i).Reverse());
				ParseJournal(SendCode(bytes).Result.FirstOrDefault().Data, journalItems);
			}
			int no = 0;
			foreach (var item in journalItems)
			{
				no++;
				item.No = no;
			}
			secJournalItems.ForEach(x => journalItems.Add(x)); // в случае, если устройство не Рубеж-2ОП, коллекция охранных событий будет пустая
			return journalItems;
		}

		public static Device AutoDetectDevice()
		{
			byte deviceCount;
			var bytes = new List<byte>();
			bytes.AddRange(BitConverter.GetBytes(++_usbRequestNo).Reverse());
			bytes.Add(0x01);
			bytes.Add(0x01);
			bytes.Add(0x04);
			var res = SendCode(bytes).Result;

			var computerDevice = new Device();
			var msDevice = new Device();
			var usbChannel1Device = new Device();
			var usbChannel2Device = new Device();

			// Добавляем компьютер
			computerDevice.DriverUID = new Guid("F8340ECE-C950-498D-88CD-DCBABBC604F3");
			computerDevice.Driver = Drivers.FirstOrDefault(x => x.UID == computerDevice.DriverUID);

			// МС-1
			byte ms = 0x03;
			msDevice.DriverUID = new Guid("FDECE1B6-A6C6-4F89-BFAE-51F2DDB8D2C6");
			msDevice.Driver = Drivers.FirstOrDefault(x => x.UID == msDevice.DriverUID);

			// Добавляем 1-й канал
			usbChannel1Device.DriverUID = new Guid("780DE2E6-8EDD-4CFA-8320-E832EB699544");
			usbChannel1Device.Driver = Drivers.FirstOrDefault(x => x.UID == usbChannel1Device.DriverUID);
			usbChannel1Device.IntAddress = 1;
			msDevice.Children.Add(usbChannel1Device);

			if (res.FirstOrDefault().Data[5] == 0x41) // запрашиваем второй шлейф
			{
				// МС-2
				ms = 0x04;
				msDevice.DriverUID = new Guid("CD0E9AA0-FD60-48B8-B8D7-F496448FADE6");
				msDevice.Driver = Drivers.FirstOrDefault(x => x.UID == msDevice.DriverUID);

				// Добавляем 2-й канал
				usbChannel2Device.DriverUID = new Guid("F36B2416-CAF3-4A9D-A7F1-F06EB7AAA76E");
				usbChannel2Device.Driver = Drivers.FirstOrDefault(x => x.UID == usbChannel2Device.DriverUID);
				usbChannel2Device.IntAddress = 2;
				msDevice.Children.Add(usbChannel2Device);
			}

			// Добавляем МС
			computerDevice.Children.Add(msDevice);
			for (byte sleif = 0x03; sleif <= ms; sleif++)
			{
				for (deviceCount = 1; deviceCount < 128; deviceCount++)
				{
					if (Progress != null)
						Progress(Convert.ToInt32(deviceCount), 127, (sleif - 2) + " - Канал. Поиск PNP-устройств Рубеж с адресом: " + deviceCount + ". Всего адресов: 127");
					bytes = new List<byte>();
					bytes.AddRange(BitConverter.GetBytes(++_usbRequestNo).Reverse());
					bytes.Add(sleif);
					bytes.Add(deviceCount);
					bytes.Add(0x3C);
					var inputBytes = SendCode(bytes, 5000, 500).Result.FirstOrDefault().Data;
					if (inputBytes[6] == 0x7C) // Если по данному адресу найдено устройство, узнаем тип устройства и его версию ПО
					{
						var device = new Device();
						device.Properties = new List<Property>();
						device.Driver = new Driver();
						device.IntAddress = inputBytes[5];
						device.Driver.HasAddress = true;
						bytes = new List<byte>();
						bytes.AddRange(BitConverter.GetBytes(++_usbRequestNo).Reverse());
						bytes.Add(sleif);
						bytes.Add(deviceCount);
						bytes.Add(0x01);
						bytes.Add(0x03);
						inputBytes = SendCode(bytes).Result.FirstOrDefault().Data;
						device.Driver = Drivers.FirstOrDefault(x => x.UID == DriversHelper.GetDriverUidByType(inputBytes[7]));

						bytes = new List<byte>();
						bytes.AddRange(BitConverter.GetBytes(++_usbRequestNo).Reverse());
						bytes.Add(sleif);
						bytes.Add(deviceCount);
						bytes.Add(0x01);
						bytes.Add(0x12);
						inputBytes = SendCode(bytes).Result.FirstOrDefault().Data;
						device.Properties.Add(new Property() { Name = "Version", Value = inputBytes[7].ToString("X2") + "." + inputBytes[8].ToString("X2") });

						bytes = new List<byte>();
						bytes.AddRange(BitConverter.GetBytes(++_usbRequestNo).Reverse());
						bytes.Add(sleif);
						bytes.Add(deviceCount);
						bytes.Add(0x01);
						bytes.Add(0x52);
						bytes.Add(0x00);
						bytes.Add(0x00);
						bytes.Add(0x00);
						bytes.Add(0xF4);
						bytes.Add(0x0B);
						inputBytes = SendCode(bytes).Result.FirstOrDefault().Data;
						if (inputBytes.Count >= 18)
						{
							var serilaNo = "";
							for (int i = 7; i <= 18; i++)
								serilaNo += inputBytes[i] - 0x30 + ".";
							serilaNo = serilaNo.Remove(serilaNo.Length - 1);
							device.Properties.Add(new Property() { Name = "SerialNo", Value = serilaNo });
						}
						if (sleif == 0x03)
							usbChannel1Device.Children.Add(device);
						else
							usbChannel2Device.Children.Add(device);
					}
				}
			}
			return computerDevice;
		}

		public static List<byte> SendRequest(List<byte> bytes)
		{
			return SendCode(bytes, 100000, 100000).Result.FirstOrDefault().Data;
		}

		public static void GetDeviceParameters(Device device)
		{
			var bytesList = new List<List<byte>>();
			var properties = device.Driver.Properties.FindAll(x => x.IsAUParameter);
			var properties1 = properties;
			var properties2 = properties;
			properties.RemoveAll(x => properties1.FirstOrDefault(z => (properties2.IndexOf(x) > properties1.IndexOf(z)) && (z.No == x.No)) != null); // Удаляем из списка все параметры, коды которых уже есть в этом списке (чтобы не дублировать запрос)
			foreach (var property in properties)
			{
				if (Progress != null)
				{
					var index = properties.IndexOf(property);
					var max = properties.Count - 1;
					Progress(index, max, "");
				}
				var bytes = new List<byte>();
				bytes.AddRange(BitConverter.GetBytes(++_usbRequestNo).Reverse());
				bytes.Add(Convert.ToByte(device.Parent.Parent.IntAddress + 2));
				bytes.Add((byte)device.Parent.IntAddress);
				bytes.Add(0x02);
				bytes.Add(0x53);
				bytes.Add(0x02);
				bytes.Add((byte)MetadataHelper.GetIdByUid(device.Driver.UID));
				bytes.Add(Convert.ToByte(device.IntAddress % 256));
				bytes.Add(0x00);
				bytes.Add(property.No);
				bytes.Add(0x00);
				bytes.Add(0x00);
				bytes.Add(Convert.ToByte(device.IntAddress / 256 - 1));
				bytesList.Add(bytes);
			}
			var results = SendCode(bytesList, 3000, 300);
			foreach (var result in results.Result)
			{
				properties = device.Driver.Properties.FindAll(x => x.No == result.Data[11]);
				foreach (var property in properties)
				{
					var value = ParametersHelper.CreateProperty(result.Data[12] * 256 + result.Data[13], property);
					var deviceProperty = device.Properties.FirstOrDefault(x => x.Name == value.Name);
					deviceProperty.Value = value.Value;
				}
			}
		}

		public static void SetDeviceParameters(Device device)
		{
			var bytesList = new List<List<byte>>();
			var properties = device.Driver.Properties.FindAll(x => x.IsAUParameter);
			var properties1 = properties;
			var properties2 = properties;
			properties.RemoveAll(x => properties1.FirstOrDefault(z => (properties2.IndexOf(x) > properties1.IndexOf(z)) && (z.No == x.No)) != null); // Удаляем из списка все параметры, коды которых уже есть в этом списке (чтобы не дублировать запрос)
			foreach (var property in properties)
			{
				if ((property == null) || (!property.IsAUParameter))
					continue;
				var bytes = new List<byte>();
				bytes.AddRange(BitConverter.GetBytes(++_usbRequestNo).Reverse());
				bytes.Add(Convert.ToByte(device.Parent.Parent.IntAddress + 2));
				bytes.Add((byte)device.Parent.IntAddress);
				bytes.Add(0x02);
				bytes.Add(0x53);
				bytes.Add(0x03);
				bytes.Add((byte)MetadataHelper.GetIdByUid(device.Driver.UID));
				bytes.Add(Convert.ToByte(device.IntAddress % 256));
				bytes.Add(0x00);
				bytes.Add(property.No);
				var value = Convert.ToInt32(ParametersHelper.SetConfigurationParameters(property, device));
				bytes.Add(Convert.ToByte(value % 256));
				bytes.Add(Convert.ToByte(value / 256));
				bytes.Add(Convert.ToByte(device.IntAddress / 256 - 1));
				bytesList.Add(bytes);
			}
			SendCode(bytesList, 3000, 300);
		}

		public static Device GetDeviceConfig(Device newdevice)
		{
			var device = (Device)newdevice.Clone();
			device.Children = new List<Device>();
			GetDeviceRam(device);
			GetDeviceRom(device);
			int pointer;
			Device child;
			if ((pointer = DeviceRom[18] * 256 * 256 + DeviceRom[19] * 256 + DeviceRom[20]) != 0) // МПТ
			{
				pointer -= 0x100; // Память Ram начинается с адресса 0x100, поэтому из индекса вычитаем величину смещения
				child = new Device();
				child.DriverUID = new Guid("33a85f87-e34c-45d6-b4ce-a4fb71a36c28");
				child.Driver = Drivers.FirstOrDefault(x => x.UID == child.DriverUID);
				child.IntAddress = DeviceRam[pointer] + 256 * (DeviceRam[pointer + 1] + 1);
				device.Children.Add(child);
			}

			if ((pointer = DeviceRom[24] * 256 * 256 + DeviceRom[25] * 256 + DeviceRom[26]) != 0) // ИП-64
			{
				pointer -= 0x100;
				child = new Device();
				child.DriverUID = new Guid("1e045ad6-66f9-4f0b-901c-68c46c89e8da");
				child.Driver = Drivers.FirstOrDefault(x => x.UID == child.DriverUID);
				child.IntAddress = DeviceRam[pointer] + 256 * (DeviceRam[pointer + 1] + 1);
				device.Children.Add(child);
			}
			if ((pointer = DeviceRom[30] * 256 * 256 + DeviceRom[31] * 256 + DeviceRom[32]) != 0)  // ИП-29
			{
				pointer -= 0x100;
				child = new Device();
				child.DriverUID = new Guid("799686b6-9cfa-4848-a0e7-b33149ab940c");
				child.Driver = Drivers.FirstOrDefault(x => x.UID == child.DriverUID);
				child.IntAddress = DeviceRam[pointer] + 256 * (DeviceRam[pointer + 1] + 1);
				device.Children.Add(child);
			}
			if ((pointer = DeviceRom[36] * 256 * 256 + DeviceRom[37] * 256 + DeviceRom[38]) != 0)  // ИП-64К
			{
				pointer -= 0x100;
				child = new Device();
				child.DriverUID = new Guid("37f13667-bc77-4742-829b-1c43fa404c1f");
				child.Driver = Drivers.FirstOrDefault(x => x.UID == child.DriverUID);
				child.IntAddress = DeviceRam[pointer] + 256 * (DeviceRam[pointer + 1] + 1);
				device.Children.Add(child);
			}
			if ((pointer = DeviceRom[42] * 256 * 256 + DeviceRom[43] * 256 + DeviceRom[44]) != 0)  // АМ-1
			{
				pointer -= 0x100;
				child = new Device();
				child.DriverUID = new Guid("dba24d99-b7e1-40f3-a7f7-8a47d4433392");
				child.Driver = Drivers.FirstOrDefault(x => x.UID == child.DriverUID);
				child.IntAddress = DeviceRam[pointer] + 256 * (DeviceRam[pointer + 1] + 1);
				device.Children.Add(child);
			}
			if ((pointer = DeviceRom[48] * 256 * 256 + DeviceRom[49] * 256 + DeviceRom[50]) != 0)  // РПИ
			{
				pointer -= 0x100;
				child = new Device();
				child.DriverUID = new Guid("641fa899-faa0-455b-b626-646e5fbe785a");
				child.Driver = Drivers.FirstOrDefault(x => x.UID == child.DriverUID);
				child.IntAddress = DeviceRam[pointer] + 256 * (DeviceRam[pointer + 1] + 1);
				device.Children.Add(child);
			}
			if ((pointer = DeviceRom[54] * 256 * 256 + DeviceRom[55] * 256 + DeviceRom[56]) != 0)
				MessageBox.Show("Пока не определено");
			if ((pointer = DeviceRom[60] * 256 * 256 + DeviceRom[61] * 256 + DeviceRom[62]) != 0)
				MessageBox.Show("Пока не определено");
			if ((pointer = DeviceRom[66] * 256 * 256 + DeviceRom[67] * 256 + DeviceRom[68]) != 0)
				MessageBox.Show("Пока не определено");
			if ((pointer = DeviceRom[72] * 256 * 256 + DeviceRom[73] * 256 + DeviceRom[74]) != 0)
				MessageBox.Show("Пока не определено");
			if ((pointer = DeviceRom[78] * 256 * 256 + DeviceRom[79] * 256 + DeviceRom[80]) != 0)
				MessageBox.Show("Пока не определено");
			if ((pointer = DeviceRom[84] * 256 * 256 + DeviceRom[85] * 256 + DeviceRom[86]) != 0)
				MessageBox.Show("Пока не определено");
			if ((pointer = DeviceRom[90] * 256 * 256 + DeviceRom[91] * 256 + DeviceRom[92]) != 0)
				MessageBox.Show("Пока не определено");
			if ((pointer = DeviceRom[96] * 256 * 256 + DeviceRom[97] * 256 + DeviceRom[98]) != 0)
				MessageBox.Show("Пока не определено");
			if ((pointer = DeviceRom[102] * 256 * 256 + DeviceRom[103] * 256 + DeviceRom[104]) != 0)
				MessageBox.Show("Пока не определено");
			if ((pointer = DeviceRom[114] * 256 * 256 + DeviceRom[115] * 256 + DeviceRom[116]) != 0)
				MessageBox.Show("Пока не определено");
			if ((pointer = DeviceRom[120] * 256 * 256 + DeviceRom[121] * 256 + DeviceRom[122]) != 0) // МДУ
			{
				pointer -= 0xff; // особенность смещения указателя на МДУ вычисленная империческим способом
				child = new Device();
				child.DriverUID = new Guid("043fbbe0-8733-4c8d-be0c-e5820dbf7039");
				child.Driver = Drivers.FirstOrDefault(x => x.UID == child.DriverUID);
				child.IntAddress = DeviceRam[pointer] + 256 * (DeviceRam[pointer + 1] + 1);
				device.Children.Add(child);
			}
			//if ((pointer = DeviceRom[126] * 256 * 256 + DeviceRom[127] * 256 + DeviceRom[128]) != 0) // Выход
			//{
			//}
			if ((pointer = DeviceRom[132] * 256 * 256 + DeviceRom[133] * 256 + DeviceRom[134]) != 0)
				MessageBox.Show("Пока не определено");
			if ((pointer = DeviceRom[138] * 256 * 256 + DeviceRom[139] * 256 + DeviceRom[140]) != 0)
				MessageBox.Show("Пока не определено");
			return device;
		}

		public static void GetDeviceRam(Device device)
		{
			DeviceRam = new List<byte>();
			var bytes = new List<byte>();
			bytes.AddRange(BitConverter.GetBytes(++_usbRequestNo).Reverse());
			bytes.Add(Convert.ToByte(device.Parent.IntAddress + 2));
			bytes.Add(Convert.ToByte(device.IntAddress % 256));
			bytes.Add(0x01);
			bytes.Add(0x52);
			bytes.AddRange(BitConverter.GetBytes(0x100).Reverse());
			bytes.Add(Convert.ToByte(0xFF));
			DeviceRam = SendCode(bytes).Result.FirstOrDefault().Data;
			DeviceRam.RemoveRange(0, 7); // удаляем служебные символы
			bytes = new List<byte>();
			bytes.AddRange(BitConverter.GetBytes(++_usbRequestNo).Reverse());
			bytes.Add(Convert.ToByte(device.Parent.IntAddress + 2));
			bytes.Add(Convert.ToByte(device.IntAddress % 256));
			bytes.Add(0x01);
			bytes.Add(0x52);
			bytes.AddRange(BitConverter.GetBytes(0x200).Reverse());
			bytes.Add(Convert.ToByte(0x99));
			DeviceRam.AddRange(SendCode(bytes).Result.FirstOrDefault().Data);
			DeviceRam.RemoveRange(0x100, 7); // удаляем служебные символы
		}

		public static void GetDeviceRom(Device device)
		{
			#region Находим адрес начального блока Rom

			var bytes = new List<byte>();
			bytes.AddRange(BitConverter.GetBytes(++_usbRequestNo).Reverse());
			bytes.Add(Convert.ToByte(device.Parent.IntAddress + 2));
			bytes.Add(Convert.ToByte(device.IntAddress % 256));
			bytes.Add(0x01);
			bytes.Add(0x57);
			var result = SendCode(bytes).Result.FirstOrDefault().Data;
			var begin = BitConverter.ToInt32((result.GetRange(7, 4)).ToArray(), 0);

			#endregion Находим адрес начального блока Rom

			#region Находим адрес конечного блока Rom и число байт в этом блоке

			bytes = new List<byte>();
			bytes.AddRange(BitConverter.GetBytes(++_usbRequestNo).Reverse());
			bytes.Add(Convert.ToByte(device.Parent.IntAddress + 2));
			bytes.Add(Convert.ToByte(device.IntAddress % 256));
			bytes.Add(0x38);
			bytes.AddRange(BitConverter.GetBytes(begin));
			bytes.Add(0xFF);
			result = SendCode(bytes).Result.FirstOrDefault().Data;
			result.RemoveRange(0, 7); // удаляем служебные байты (id - 4б, адрес приемника - 1б, адрес получателя - 1б, код функции - 1б)
			var offset = 256 * result[16] + result[17];
			var count = result[18];

			#endregion Находим адрес конечного блока Rom и число байт в этом блоке

			#region Читаем все кроме последнего блока

			for (int i = begin + 1; i < offset; i++)
			{
				bytes = new List<byte>();
				bytes.AddRange(BitConverter.GetBytes(++_usbRequestNo).Reverse());
				bytes.Add(Convert.ToByte(device.Parent.IntAddress + 2));
				bytes.Add(Convert.ToByte(device.IntAddress % 256));
				bytes.Add(0x38);
				bytes.AddRange(BitConverter.GetBytes(i));
				bytes.Add(0xFF);
				result.AddRange(SendCode(bytes).Result.FirstOrDefault().Data);
			}

			#endregion Читаем все кроме последнего блока

			#region Читаем последний блок

			bytes = new List<byte>();
			bytes.AddRange(BitConverter.GetBytes(++_usbRequestNo).Reverse());
			bytes.Add(Convert.ToByte(device.Parent.IntAddress + 2));
			bytes.Add(Convert.ToByte(device.IntAddress % 256));
			bytes.Add(0x38);
			bytes.AddRange(BitConverter.GetBytes(offset));
			bytes.Add(count);
			result.AddRange(SendCode(bytes).Result.FirstOrDefault().Data);

			#endregion Читаем последний блок

			DeviceRom = new List<byte>(result);
		}

		public static void SynchronizeTime(Device device)
		{
			var bytes = new List<byte>();
			bytes.AddRange(BitConverter.GetBytes(++_usbRequestNo).Reverse());
			bytes.Add(Convert.ToByte(device.Parent.IntAddress + 2));
			bytes.Add((byte)device.IntAddress);
			bytes.Add(0x02);
			bytes.Add(0x11);
			bytes.AddRange(DateConverter.ConvertToBytes(DateTime.Now));
			SendCode(bytes);
		}

		public static class DateConverter
		{
			public static List<byte> ConvertToBytes(DateTime date)
			{
				var arr = Convert.ToString(date.Day, 2).PadLeft(5, '0').ToCharArray();
				Array.Reverse(arr);
				var day = new string(arr);
				arr = Convert.ToString(date.Month, 2).PadLeft(4, '0').ToCharArray();
				Array.Reverse(arr);
				var month = new string(arr);
				arr = Convert.ToString(date.Year - 2000, 2).PadLeft(6, '0').ToCharArray();
				Array.Reverse(arr);
				var year = new string(arr);
				arr = Convert.ToString(date.Hour, 2).PadLeft(5, '0').ToCharArray();
				Array.Reverse(arr);
				var hour = new string(arr);
				arr = Convert.ToString(date.Minute, 2).PadLeft(6, '0').ToCharArray();
				Array.Reverse(arr);
				var minute = new string(arr);
				arr = Convert.ToString(date.Second, 2).PadLeft(6, '0').ToCharArray();
				Array.Reverse(arr);
				var second = new string(arr);
				var binstring = day + month + year + hour + minute + second;
				var bytes = new List<byte>();
				for (int i = 0; i < 4; ++i)
				{
					arr = binstring.Substring(8 * i, 8).ToCharArray();
					Array.Reverse(arr);
					bytes.Add(Convert.ToByte(new string(arr), 2));
				}
				return bytes;
			}

			public static DateTime ConvertFromBytes(List<byte> timeBytes)
			{
				var bitsExtracter = new BitsExtracter(timeBytes);
				var day = bitsExtracter.Get(0, 4);
				var month = bitsExtracter.Get(5, 8);
				var year = 2000 + bitsExtracter.Get(9, 14);
				var hour = bitsExtracter.Get(15, 19);
				var minute = bitsExtracter.Get(20, 25);
				var second = bitsExtracter.Get(26, 31);
				return new DateTime(year, month, day, hour, minute, second);
			}
		}
	}
}