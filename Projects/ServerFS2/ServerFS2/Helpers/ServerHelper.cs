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
        public static event Action<int,int,string> Progress;
        public static List<Driver> Drivers;
        static ServerHelper()
        {
            MetadataHelper.Initialize();
            ConfigurationManager.Load();
            Drivers = ConfigurationManager.DriversConfiguration.Drivers;
            UsbRunner = new UsbRunner();
            UsbRunner.Open();
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
        static OperationResult<List<Response>> SendCode(List<List<byte>> bytesList, int delay = 1000, int timeout = 1000)
        {
            return UsbRunner.AddRequest(bytesList, delay, timeout);
        }
        static OperationResult<List<Response>> SendCode(List<byte> bytes, int delay = 1000, int timeout = 1000)
        {
            return UsbRunner.AddRequest(new List<List<byte>> { bytes }, delay, timeout);
        }
		public static List<JournalItem> GetSecJournalItems2Op(Device device)
		{
			int lastindex = GetLastSecJournalItemId2Op(device);
			var journlaItems = new List<JournalItem>();
			for (int i = 0; i <= lastindex; i++)
			{
				var bytes = new List<byte>();
				bytes.AddRange(BitConverter.GetBytes(++_usbRequestNo).Reverse());
				bytes.Add(Convert.ToByte(Convert.ToInt32(device.Properties.FirstOrDefault(x => x.Name == "UsbChannel").Value) + 2));
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
			bytes.Add(Convert.ToByte(Convert.ToInt32(device.Properties.FirstOrDefault(x => x.Name == "UsbChannel").Value) + 2));
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
			bytes.Add(Convert.ToByte(Convert.ToInt32(device.Properties.FirstOrDefault(x => x.Name == "UsbChannel").Value) + 2));
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
			bytes.Add(Convert.ToByte(Convert.ToInt32(device.Properties.FirstOrDefault(x => x.Name == "UsbChannel").Value) + 2));
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
				bytes.Add(Convert.ToByte(Convert.ToInt32(device.Properties.FirstOrDefault(x => x.Name == "UsbChannel").Value) + 2));
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
                        Progress(Convert.ToInt32(deviceCount), 127, (sleif - 2) + " - Канал. Поиск PNP-устройств Рубеж с адресом: "+ deviceCount + ". Всего адресов: 127");
					bytes = new List<byte>();
					bytes.AddRange(BitConverter.GetBytes(++_usbRequestNo).Reverse());
					bytes.Add(sleif);
					bytes.Add(deviceCount);
					bytes.Add(0x3C);
					var inputBytes = SendCode(bytes,5000,500).Result.FirstOrDefault().Data;
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
                        if(sleif == 0x03)
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
			return SendCode(bytes).Result.FirstOrDefault().Data;
		}
        public static List<Property> GetDeviceParameters(Device device)
        {
            var values = new List<Property>();
            var bytesList = new List<List<byte>>();
            var properties = device.Driver.Properties.FindAll(x => x.IsAUParameter);
            var properties1 = properties;
            var properties2 = properties;
            properties.RemoveAll(x => properties1.FirstOrDefault(z => (properties2.IndexOf(x) > properties1.IndexOf(z))&&(z.No == x.No)) != null); // Удаляем из списка все параметры, коды которых уже есть в этом списке (чтобы не дублировать запрос)
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
                bytes.Add((byte) device.Parent.IntAddress);
                bytes.Add(0x02);
                bytes.Add(0x53);
                bytes.Add(0x02);
                bytes.Add((byte) MetadataHelper.GetIdByUid(device.Driver.UID));
                bytes.Add(Convert.ToByte(device.IntAddress%256));
                bytes.Add(0x00);
                bytes.Add(property.No);
                bytes.Add(0x00);
                bytes.Add(0x00);
                bytes.Add(Convert.ToByte(device.IntAddress/256 - 1));
                bytesList.Add(bytes);
            }
            var results = SendCode(bytesList, 1000000);
            foreach (var result in results.Result)
            {
                properties = device.Driver.Properties.FindAll(x => x.No == result.Data[11]);
                foreach (var property in properties)
                {
                    var value = ParametersHelper.CreateProperty(result.Data[12] * 256 + result.Data[13], property);
                    var deviceProperty = device.Properties.FirstOrDefault(x => x.Name == value.Name);
                    deviceProperty.Value = value.Value;
                    value.Name = property.Caption;
                    values.Add(value);
                }
            }
            return values;
        }
        public static List<Property> SetDeviceParameters(Device device)
        {
            var values = new List<Property>();
            var bytesList = new List<List<byte>>();
            foreach (var property in device.Driver.Properties)
            {
                if ((!property.IsAUParameter) || (bytesList.FirstOrDefault(x => x[12] == property.No)) != null)
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
                bytes.Add(0x00);
                bytes.Add(0x00);
                bytes.Add(Convert.ToByte(device.IntAddress / 256 - 1));
                bytesList.Add(bytes);
            }
            var results = SendCode(bytesList, 1000000);
            foreach (var result in results.Result)
            {
                var properties = device.Driver.Properties.FindAll(x => x.No == result.Data[11]);
                foreach (var property in properties)
                {
                    var value = ParametersHelper.CreateProperty(result.Data[12] * 256 + result.Data[13], property);
                    value.Name = property.Caption;
                    values.Add(value);
                }
            }
            return values;
        }
    }
}