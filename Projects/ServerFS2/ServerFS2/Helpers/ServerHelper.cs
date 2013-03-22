using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using ClientFS2.ConfigurationWriter;
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
		public static List<byte> DeviceRom;
        public static List<byte> DeviceRam;
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
				bytes.Add(Convert.ToByte(device.AddressOnShleif));
				bytes.Add(0x00);
				bytes.Add(property.No);
				var value = Convert.ToInt32(ParametersHelper.SetConfigurationParameters(property, device));
				bytes.Add(Convert.ToByte(value % 256));
				bytes.Add(Convert.ToByte(value / 256));
				bytes.Add(Convert.ToByte(device.AddressOnShleif - 1));
				bytesList.Add(bytes);
			}
			SendCode(bytesList, 3000, 300);
		}
        public static DeviceConfiguration GetDeviceConfig(Device selectedDevice)
		{
            var remoteDeviceConfiguration = new DeviceConfiguration();
            var device = (Device)selectedDevice.Clone();
            remoteDeviceConfiguration.RootDevice = device;
            remoteDeviceConfiguration.Devices.Add(device);
			device.Children = new List<Device>();
            device.Children.AddRange(selectedDevice.Children.FindAll((x => x.DriverUID == new Guid("05323d14-9070-44b8-b91c-be024f10e267"))));
            var zones = new List<Zone>();
            GetDeviceRamFirstAndRomLastIndex(device);
			GetDeviceRom(device);
            GetDeviceRam(device);
			int pointer;
            int pPointer;
			Device child;
            int sleifCount = device.Driver.ShleifCount;
            var zonePanelRelationsInfo = new ZonePanelRelationsInfo();

            #region Хидеры таблицы указателей на указатели на зоны
            if ((pPointer = DeviceRam[1542] * 256 * 256 + DeviceRam[1543] * 256 + DeviceRam[1544]) != 0)
            {
                // [1546] - длина записи
                int count = DeviceRam[1546] * 256 + DeviceRam[1547];
                pointer = DeviceRam[pPointer - _deviceRamFirstIndex] * 256 * 256 + DeviceRam[pPointer - _deviceRamFirstIndex + 1] * 256 + DeviceRam[pPointer - _deviceRamFirstIndex + 2] - 0x100;
                for (int i = 0; i < count; i++)
                {
                    var zone = new Zone();
                    // 0,1,2,3 - Внутренние параметры (снят с охраны/ на охране, неисправность, пожар, ...)
                    zone.No = DeviceRom[pointer + 33] * 256 + DeviceRom[pointer + 34]; // Глобальный номер зоны
                    if (zones.FirstOrDefault(x => x.No == zone.No) != null) // Если зона с таким номером уже добавлена, то пропускаем её
                    {
                        pointer = pointer + DeviceRom[pointer + 26] * 256 + DeviceRom[pointer + 27]; // Длина записи (2) pointer + 26
                        continue;
                    }
                    zone.Name = new string(Encoding.Default.GetChars(DeviceRom.GetRange(pointer + 6, 20).ToArray()));
                    zone.Name.Replace(" ", "");
                    // Длина нижеследующих параметров (1) pointer + 28
                    // Конфин (1) (0-пожараная, 1-охранная, 2-комбирированная, 3-технологическая) pointer + 29
                    // Битовые атрибуты(1) (0x01 - автопостановка, 0x02 - обойденная/тихая тревога, 0x04 - к зоне привязан МПТ(для фильтра при управлении СПТ через зону)
                    // 0x08 - охранная зона с задержкой, 0x10 - охранная зона проходная  pointer + 30
                    // Количество датчиков для формирования "пожар" (иначе "внимание") (1)  pointer + 31
                    // Количество потерянных ИП в зоне (1) pointer + 32
                    // Глобальный номер зоны (2) pointer + 33
                    // Время входной/выходной задержки (для охранных с задержкой входа/выхода), 10 x сек(2) pointer + 35
                    // Номер направления куда входит зона (1) pointer + 37
                    // Время автоперевзятия, сек(1) pointer + 38
                    // Общее количество связанных с зоной ИУ (2) pointer + 39
                    // Указатель на ведущее МПТ в зоне из таблицы МПТ или 0 (3) pointer + 41

                    int tableDynamicSize = 0; // размер динамической части таблицы
                    for (int sleifNo = 0; sleifNo < sleifCount; sleifNo++)
                    {
                        var inExecDeviceCount = DeviceRom[pointer + 44 + sleifNo * 4];
                        tableDynamicSize += inExecDeviceCount * 3;
                        // Количество связанных ИУ sleifNo шлейфа (1) pointer + 44 + sleifNo*4(т.к. для каждого шлейфа эта информация занимает 4 байта - кол-во связанных ИУ - 1 байт и абс адрес - 4 байта)
                        //pPointer = DeviceRom[pointer + 45 + sleifNo * 4] * 256 * 256 + DeviceRom[pointer + 46 + sleifNo * 4] * 256 + DeviceRom[pointer + 47 + sleifNo * 4]; // Указатель на размещение абсолютного адреса первого в списке связанного ИУ sleifNo шлейфа или 0 при отсутсвие ИУ (3) pointer + 45 + sleifNo*4
                        //for (int inExecDeviceNo = 0; inExecDeviceNo < inExecDeviceCount; inExecDeviceNo++)
                        //{
                        //    int localPointer = DeviceRom[pPointer + inExecDeviceNo * 3 - 0x100] * 256 * 256 +
                        //                       DeviceRom[pPointer + inExecDeviceNo * 3 + 1 - 0x100] * 256 +
                        //                       DeviceRom[pPointer + inExecDeviceNo * 3 + 2 - 0x100];
                        //    int intAddress = DeviceRom[localPointer - 0x100 + 1] +
                        //                     (DeviceRom[localPointer - 0x100 + 2] + 1) * 256;
                        //    child = device.Children.FirstOrDefault(x => x.IntAddress == intAddress);
                        //    zone.DevicesInZoneLogic.Add(child);
                        //    child.ZoneLogic = new ZoneLogic();
                        //    child.ZoneLogic.Clauses = new List<Clause>();
                        //    var clause = new Clause();
                        //    clause.ZoneUIDs.Add(zone.UID);
                        //    clause.Zones.Add(zone);
                        //    child.ZoneLogic.Clauses.Add(clause);
                        //}
                    }

                    var outExecDeviceCount = DeviceRom[pointer + 44 + sleifCount * 4]; // количество связанных внешних ИУ, кроме тех у которых в логике "межприборное И"
                    tableDynamicSize += outExecDeviceCount * 3;
                    pPointer = DeviceRom[pointer + 45 + sleifCount * 4] * 256 * 256 + DeviceRom[pointer + 46 + sleifCount * 4] * 256 + DeviceRom[pointer + 47 + sleifCount * 4]; // Указатель на размещение абсолютного адреса первого в списке связанного внешнего ИУ или 0 при отсутсвие ИУ (3)
                    for (int outExecDeviceNo = 0; outExecDeviceNo < outExecDeviceCount; outExecDeviceNo++)
                    {
                        int localPointer = DeviceRom[pPointer + outExecDeviceNo * 3 - 0x100] * 256 * 256 +
                                           DeviceRom[pPointer + outExecDeviceNo * 3 + 1 - 0x100] * 256 +
                                           DeviceRom[pPointer + outExecDeviceNo * 3 + 2 - 0x100];
                        int intAddress = DeviceRom[localPointer - 0x100 + 1] +
                                         (DeviceRom[localPointer - 0x100 + 2] + 1) * 256;
                        // ... //
                    }

                    var outPanelCount = DeviceRom[pointer + 48 + sleifCount * 4]; // Количество внешних приборов, ИУ которого могут управляться нашими ИП по логике "межприборное И" или 0 (1)
                    tableDynamicSize += outPanelCount; // не умнажаем на 3, т.к. адрес прибора записывается в 1 байт
                    var zonePanelItem = new ZonePanelItem();
                    zonePanelItem.IsRemote = true;
                    zonePanelItem.No = DeviceRom[pointer + 4] * 256 + DeviceRom[pointer + 5]; // локальный номер зоны
                    zonePanelItem.PanelDevice = device;
                    zonePanelItem.Zone = zone;
                    zonePanelRelationsInfo.ZonePanelItems.Add(zonePanelItem);
                    zones.Add(zone);
                    remoteDeviceConfiguration.Zones.Add(zone);
                    pointer = pointer + 48 + sleifCount * 4 + tableDynamicSize + 1;
                }
            }
            #endregion

            #region Хидеры таблиц на исполнительные устройства
            if ((pointer = DeviceRam[12] * 256 * 256 + DeviceRam[13] * 256 + DeviceRam[14]) != 0) //РМ-1
            {
                pointer -= 0x100;
                child = new Device();
                child.ZoneLogic = new ZoneLogic();
                child.ZoneLogic.Clauses = new List<Clause>();
                child.DriverUID = new Guid("4a60242a-572e-41a8-8b87-2fe6b6dc4ace");
                child.Driver = Drivers.FirstOrDefault(x => x.UID == child.DriverUID);
                // адрес прибора привязки в сети (0 для локальной) (1) pointer + 0
                child.IntAddress = DeviceRom[pointer + 1] + 256 * (DeviceRom[pointer + 2] + 1);
                // внутренние параметры (2) pointer + 3
                // динамические параметры для базы (1) pointer + 5
                var description = new string(Encoding.Default.GetChars(DeviceRom.GetRange(pointer + 6, 20).ToArray()));
                var configAndParamSize = DeviceRom[pointer + 26];// длина переменной части блока с конфигурацией и сырыми параметрами (1) pointer + 26
                // общая длина записи (2) pointer + 27
                pointer = pointer + configAndParamSize; // конфиг и сырые параметры
                /* Настройка логики */
                byte outAndOr = 1;
                int tableDynamicSize = 0; // размер динамической части таблицы + 1
                while (outAndOr != 0)
                {
                    pointer = pointer + tableDynamicSize;
                    byte inAndOr = DeviceRom[pointer + 29]; // логика внутри группы зон с одинаковым типом события 0x01 - "и", 0x02 - "или"
                    byte eventType = DeviceRom[pointer + 30]; // Тип события по которому срабатывать в этой группе зон (1)
                    outAndOr = DeviceRom[pointer + 31];
                    if (outAndOr == 0x01)
                        child.ZoneLogic.JoinOperator = ZoneLogicJoinOperator.And;
                    else
                        child.ZoneLogic.JoinOperator = ZoneLogicJoinOperator.Or;
                    int zonesCount = DeviceRom[pointer + 32] * 256 + DeviceRom[pointer + 33];
                    tableDynamicSize += 5;
                    var clause = new Clause();
                    switch (eventType)
                    {
                        case 0x01:
                            clause.State = ZoneLogicState.MPTAutomaticOn;
                            break;
                        case 0x04:
                            clause.State = ZoneLogicState.Fire;
                            break;
                        case 0x20:
                            clause.State = ZoneLogicState.Attention;
                            break;
                        case 0x40:
                            clause.State = ZoneLogicState.MPTOn;
                            break;
                        default:
                            clause.State = new ZoneLogicState();
                            break;
                    }
                    if (inAndOr == 0x01)
                        clause.Operation = ZoneLogicOperation.All;
                    else
                        clause.Operation = ZoneLogicOperation.Any;
                    for (int zoneNo = 0; zoneNo < zonesCount; zoneNo++)
                    {
                        var localPointer = DeviceRom[pointer + 34 + zoneNo*3]*256*256 +
                                           DeviceRom[pointer + 35 + zoneNo * 3] * 256 + DeviceRom[pointer + 36 + zoneNo * 3] - 0x100;
                        // ... здесь инициализируются все зоны учавствующие в логике ... //
                        var zone = new Zone();
                        zone.No = DeviceRom[localPointer + 33] * 256 + DeviceRom[localPointer + 34]; // Глобальный номер зоны
                        zone.Name = new string(Encoding.Default.GetChars(DeviceRom.GetRange(localPointer + 6, 20).ToArray()));
                        zone.Name.Replace(" ", "");
                        zone.DevicesInZoneLogic.Add(child);
                        tableDynamicSize += 3;
                        if (zones.FirstOrDefault(x => x.No == zone.No) != null) // Если зона с таким номером уже добавлена, то добавляем её в clauses и продолжаем цикл
                        {
                            clause.ZoneUIDs.Add(zones.FirstOrDefault(x => x.No == zone.No).UID);
                            continue;
                        }

                        clause.ZoneUIDs.Add(zone.UID);
                        zones.Add(zone);
                        var zonePanelItem = new ZonePanelItem();
                        zonePanelItem.IsRemote = true;
                        zonePanelItem.No = DeviceRom[localPointer + 4] * 256 + DeviceRom[localPointer + 5]; // локальный номер зоны
                        zonePanelItem.PanelDevice = device;
                        zonePanelItem.Zone = zone;
                        zonePanelRelationsInfo.ZonePanelItems.Add(zonePanelItem);
                        remoteDeviceConfiguration.Zones.Add(zone);
                    }
                    child.ZoneLogic.Clauses.Add(clause);
                }
                device.Children.Add(child);
            }
            if ((pointer = DeviceRam[18] * 256 * 256 + DeviceRam[19] * 256 + DeviceRam[20]) != 0) // МПТ-1
			{
                pointer -= 0x100;
                child = new Device();
                child.ZoneLogic = new ZoneLogic();
                child.ZoneLogic.Clauses = new List<Clause>();
                child.DriverUID = new Guid("33a85f87-e34c-45d6-b4ce-a4fb71a36c28");
                child.Driver = Drivers.FirstOrDefault(x => x.UID == child.DriverUID);
                // адрес прибора привязки в сети (0 для локальной) (1) pointer + 0
                child.IntAddress = DeviceRom[pointer + 1] + 256 * (DeviceRom[pointer + 2] + 1);
                // внутренние параметры (2) pointer + 3
                // динамические параметры для базы (1) pointer + 5
                var description = new string(Encoding.Default.GetChars(DeviceRom.GetRange(pointer + 6, 20).ToArray()));
                // длина переменной части блока с конфигурацией и сырыми параметрами (1) pointer + 26
                // общая длина записи (2) pointer + 27
                // сырые параметры устройств МПТ (5) pointer + 29
                // параметры конфигурации заливаемые с компа мпт-7 байт pointer + 34 следующие:
                // конфиг (1) pointer + 34
                // адрес родителя (1) pointer + 35
                // шлейф родителя (1) pointer + 36
                // задержка запуска (2) pointer + 37
			    var zoneNo = DeviceRom[pointer + 39]*256 + DeviceRom[pointer + 40];
                child.Zone = zonePanelRelationsInfo.ZonePanelItems.FirstOrDefault(x => x.No == zoneNo).Zone;// номер привязанной зоны (2) pointer + 40
			    child.ZoneUID = child.Zone.UID;
                device.Children.Add(child);
			}
            if ((pointer = DeviceRam[120] * 256 * 256 + DeviceRam[121] * 256 + DeviceRam[122]) != 0) // МДУ (в документе это МУК-1Э, а не МДУ)
            {
                pointer -= 0x100;
                child = new Device();
                child.ZoneLogic = new ZoneLogic();
                child.ZoneLogic.Clauses = new List<Clause>();
                child.DriverUID = new Guid("043fbbe0-8733-4c8d-be0c-e5820dbf7039");
                child.Driver = Drivers.FirstOrDefault(x => x.UID == child.DriverUID);
                // адрес прибора привязки в сети (0 для локальной) (1) pointer + 0
                child.IntAddress = DeviceRom[pointer + 1] + 256 * (DeviceRom[pointer + 2] + 1);
                // внутренние параметры (2) pointer + 3
                // динамические параметры для базы (1) pointer + 5
                var description = new string(Encoding.Default.GetChars(DeviceRom.GetRange(pointer + 6, 20).ToArray()));
                // длина переменной части блока с конфигурацией и сырыми параметрами (1) pointer + 26
                // общая длина записи (2) pointer + 27
                // сырые параметры устройств МУК-1Э (4) pointer + 29
                // конфиг (1) pointer + 33
                
                /* Настройка логики */
                byte outAndOr = 0x01;
                int tableDynamicSize = 0; // размер динамической части таблицы + 1
                while (outAndOr != 0)
                {
                    pointer = pointer + tableDynamicSize;
                    byte inAndOr = DeviceRom[pointer + 34]; // логика внутри группы зон с одинаковым типом события 0x01 - "и", 0x02 - "или"
                    byte eventType = DeviceRom[pointer + 35]; // Тип события по которому срабатывать в этой группе зон (1)
                    // $01  -  включение автоматики
                    // $02  -  тревога
                    // $03  -  поставлен на охрану
                    // $05  -  снят с охраны
                    // $06  -  ПЦН
                    // $07  -  меандр
                    // $04  -  пожар
                    // $08  -  неисправность
                    // $09  -  включение НС
                    // $0A  -  выключение автоматики НС
                    // $10  -  выходная задержка
                    // $20  -  внимание
                    // $40  -  срабатывание модуля пожаротушения
                    // $80  -  тушение
                    // $0B  -  активация устройства АМ-1Т или МДУ
                    outAndOr = DeviceRom[pointer + 36];
                    if (outAndOr == 0x01)
                        child.ZoneLogic.JoinOperator = ZoneLogicJoinOperator.And;
                    else
                        child.ZoneLogic.JoinOperator = ZoneLogicJoinOperator.Or;
                    int zonesCount = DeviceRom[pointer + 37] * 256 + DeviceRom[pointer + 38];
                    tableDynamicSize += 5;
                    var clause = new Clause();
                    switch (eventType)
                    {
                        case 0x01:
                            clause.State = ZoneLogicState.MPTAutomaticOn;
                            break;
                        case 0x04:
                            clause.State = ZoneLogicState.Fire;
                            break;
                        case 0x20:
                            clause.State = ZoneLogicState.Attention;
                            break;
                        case 0x40:
                            clause.State = ZoneLogicState.MPTOn;
                            break;
                        default:
                            clause.State = new ZoneLogicState();
                            break;
                    }
                    if (inAndOr == 0x01)
                        clause.Operation = ZoneLogicOperation.All;
                    else
                        clause.Operation = ZoneLogicOperation.Any;
                    for (int zoneNo = 0; zoneNo < zonesCount; zoneNo++)
                    {
                        var localPointer = DeviceRom[pointer + 39 + zoneNo*3]*256*256 +
                                           DeviceRom[pointer + 40 + zoneNo * 3] * 256 + DeviceRom[pointer + 41 + zoneNo * 3] - 0x100;
                        // ... здесь инициализируются все зоны учавствующие в логике ... //
                        var zone = new Zone();
                        zone.No = DeviceRom[localPointer + 33] * 256 + DeviceRom[localPointer + 34]; // Глобальный номер зоны
                        zone.Name = new string(Encoding.Default.GetChars(DeviceRom.GetRange(localPointer + 6, 20).ToArray()));
                        zone.Name.Replace(" ", "");
                        zone.DevicesInZoneLogic.Add(child);
                        tableDynamicSize += 3;
                        if (zones.FirstOrDefault(x => x.No == zone.No) != null) // Если зона с таким номером уже добавлена, то добавляем её в clauses и продолжаем цикл
                        {
                            clause.ZoneUIDs.Add(zones.FirstOrDefault(x => x.No == zone.No).UID);
                            continue;
                        }

                        clause.ZoneUIDs.Add(zone.UID);
                        zones.Add(zone);
                        var zonePanelItem = new ZonePanelItem();
                        zonePanelItem.IsRemote = true;
                        zonePanelItem.No = DeviceRom[localPointer + 4] * 256 + DeviceRom[localPointer + 5]; // локальный номер зоны
                        zonePanelItem.PanelDevice = device;
                        zonePanelItem.Zone = zone;
                        zonePanelRelationsInfo.ZonePanelItems.Add(zonePanelItem);
                        remoteDeviceConfiguration.Zones.Add(zone);
                    }
                    child.ZoneLogic.Clauses.Add(clause);
                }
                device.Children.Add(child);
            }

            if ((pointer = DeviceRam[84] * 256 * 256 + DeviceRam[85] * 256 + DeviceRam[86]) != 0) // МРО-2
            {
                pointer -= 0x100;
                child = new Device();
                child.ZoneLogic = new ZoneLogic();
                child.ZoneLogic.Clauses = new List<Clause>();
                child.DriverUID = new Guid("2d078d43-4d3b-497c-9956-990363d9b19b");
                child.Driver = Drivers.FirstOrDefault(x => x.UID == child.DriverUID);
                // адрес прибора привязки в сети (0 для локальной) (1) pointer + 0
                child.IntAddress = DeviceRom[pointer + 1] + 256 * (DeviceRom[pointer + 2] + 1);
                // внутренние параметры (2) pointer + 3
                // динамические параметры для базы (1) pointer + 5
                var description = new string(Encoding.Default.GetChars(DeviceRom.GetRange(pointer + 6, 20).ToArray()));
                // длина переменной части блока с конфигурацией и сырыми параметрами (1) pointer + 26
                // общая длина записи (2) pointer + 27
                // сырые параметры устройства МРО-2 (2) pointer + 28
                // конфиг (1) pointer + 31

                /* Настройка логики */
                byte outAndOr = 0x01;
                int tableDynamicSize = 0; // размер динамической части таблицы + 1
                while (outAndOr != 0)
                {
                    pointer = pointer + tableDynamicSize;
                    byte inAndOr = DeviceRom[pointer + 32]; // логика внутри группы зон с одинаковым типом события 0x01 - "и", 0x02 - "или"
                    byte eventType = DeviceRom[pointer + 33]; // Тип события по которому срабатывать в этой группе зон (1)
                    outAndOr = DeviceRom[pointer + 34];
                    if (outAndOr == 0x01)
                        child.ZoneLogic.JoinOperator = ZoneLogicJoinOperator.And;
                    else
                        child.ZoneLogic.JoinOperator = ZoneLogicJoinOperator.Or;
                    int zonesCount = DeviceRom[pointer + 35] * 256 + DeviceRom[pointer + 36];
                    tableDynamicSize += 5;
                    var clause = new Clause();
                    switch (eventType)
                    {
                        case 0x01:
                            clause.State = ZoneLogicState.MPTAutomaticOn;
                            break;
                        case 0x04:
                            clause.State = ZoneLogicState.Fire;
                            break;
                        case 0x20:
                            clause.State = ZoneLogicState.Attention;
                            break;
                        case 0x40:
                            clause.State = ZoneLogicState.MPTOn;
                            break;
                        default:
                            clause.State = new ZoneLogicState();
                            break;
                    }
                    if (inAndOr == 0x01)
                        clause.Operation = ZoneLogicOperation.All;
                    else
                        clause.Operation = ZoneLogicOperation.Any;
                    for (int zoneNo = 0; zoneNo < zonesCount; zoneNo++)
                    {
                        var localPointer = DeviceRom[pointer + 37 + zoneNo * 3] * 256 * 256 +
                                           DeviceRom[pointer + 38 + zoneNo * 3] * 256 + DeviceRom[pointer + 39 + zoneNo * 3] - 0x100;
                        // ... здесь инициализируются все зоны учавствующие в логике ... //
                        var zone = new Zone();
                        zone.No = DeviceRom[localPointer + 33] * 256 + DeviceRom[localPointer + 34]; // Глобальный номер зоны
                        zone.Name = new string(Encoding.Default.GetChars(DeviceRom.GetRange(localPointer + 6, 20).ToArray()));
                        zone.Name.Replace(" ", "");
                        zone.DevicesInZoneLogic.Add(child);
                        tableDynamicSize += 3;
                        if (zones.FirstOrDefault(x => x.No == zone.No) != null) // Если зона с таким номером уже добавлена, то добавляем её в clauses и продолжаем цикл
                        {
                            clause.ZoneUIDs.Add(zones.FirstOrDefault(x => x.No == zone.No).UID);
                            continue;
                        }

                        clause.ZoneUIDs.Add(zone.UID);
                        zones.Add(zone);
                        var zonePanelItem = new ZonePanelItem();
                        zonePanelItem.IsRemote = true;
                        zonePanelItem.No = DeviceRom[localPointer + 4] * 256 + DeviceRom[localPointer + 5]; // локальный номер зоны
                        zonePanelItem.PanelDevice = device;
                        zonePanelItem.Zone = zone;
                        zonePanelRelationsInfo.ZonePanelItems.Add(zonePanelItem);
                        remoteDeviceConfiguration.Zones.Add(zone);
                    }
                    child.ZoneLogic.Clauses.Add(clause);
                }
                device.Children.Add(child);
            }

            #endregion

            #region Хидеры таблиц на не исполнительные устройства по типам

            if ((pointer = DeviceRam[24] * 256 * 256 + DeviceRam[25] * 256 + DeviceRam[26]) != 0) // ИП-64
			{
				pointer -= 0x100;
				child = new Device();
				child.DriverUID = new Guid("1e045ad6-66f9-4f0b-901c-68c46c89e8da");
				child.Driver = Drivers.FirstOrDefault(x => x.UID == child.DriverUID);
                child.IntAddress = DeviceRom[pointer] + 256 * (DeviceRom[pointer + 1] + 1);
                int zoneNo = DeviceRom[pointer + 5]*256 + DeviceRom[pointer + 6];
                if (zoneNo != 0)
                {
                    child.Zone =
                        zonePanelRelationsInfo.ZonePanelItems.FirstOrDefault(
                            x => (x.No == zoneNo) && x.PanelDevice.IntAddress == device.IntAddress).Zone;
                    child.ZoneUID = child.Zone.UID;
                }
                device.Children.Add(child);
			}
            if ((pointer = DeviceRam[30] * 256 * 256 + DeviceRam[31] * 256 + DeviceRam[32]) != 0)  // ИП-29
			{
				pointer -= 0x100;
				child = new Device();
				child.DriverUID = new Guid("799686b6-9cfa-4848-a0e7-b33149ab940c");
				child.Driver = Drivers.FirstOrDefault(x => x.UID == child.DriverUID);
                child.IntAddress = DeviceRom[pointer] + 256 * (DeviceRom[pointer + 1] + 1);
                int zoneNo = DeviceRom[pointer + 5] * 256 + DeviceRom[pointer + 6];
                if (zoneNo != 0)
                {
                    child.Zone =
                        zonePanelRelationsInfo.ZonePanelItems.FirstOrDefault(
                            x => (x.No == zoneNo) && x.PanelDevice.IntAddress == device.IntAddress).Zone;
                    child.ZoneUID = child.Zone.UID;
                }
				device.Children.Add(child);
			}
            if ((pointer = DeviceRam[36] * 256 * 256 + DeviceRam[37] * 256 + DeviceRam[38]) != 0)  // ИП-64К
			{
				pointer -= 0x100;
				child = new Device();
				child.DriverUID = new Guid("37f13667-bc77-4742-829b-1c43fa404c1f");
				child.Driver = Drivers.FirstOrDefault(x => x.UID == child.DriverUID);
                child.IntAddress = DeviceRom[pointer] + 256 * (DeviceRom[pointer + 1] + 1);
                int zoneNo = DeviceRom[pointer + 5] * 256 + DeviceRom[pointer + 6];
                if (zoneNo != 0)
                {
                    child.Zone =
                        zonePanelRelationsInfo.ZonePanelItems.FirstOrDefault(
                            x => (x.No == zoneNo) && x.PanelDevice.IntAddress == device.IntAddress).Zone;
                    child.ZoneUID = child.Zone.UID;
                }
				device.Children.Add(child);
			}
            if ((pointer = DeviceRam[42] * 256 * 256 + DeviceRam[43] * 256 + DeviceRam[44]) != 0)  // АМ-1П, КО, КЗ, КУА, КнВклШУЗ, КнРазблАвт, КнВыклШУЗ 
            {
                var count = DeviceRam[46]*256 + DeviceRam[47]; // текущее число записей в таблице
				pointer -= 0x100;
                var ppuDevice = new Device();
                for (int i = 0; i < count; i++)
                {
                    child = new Device();
                    child.IntAddress = DeviceRom[pointer] + 256*(DeviceRom[pointer + 1] + 1);
                    int zoneNo = DeviceRom[pointer + 5]*256 + DeviceRom[pointer + 6];
                    if (zoneNo != 0)
                    {
                        child.Zone =
                            zonePanelRelationsInfo.ZonePanelItems.FirstOrDefault(
                                x => (x.No == zoneNo) && x.PanelDevice.IntAddress == device.IntAddress).Zone;
                        child.ZoneUID = child.Zone.UID;
                    }
                    var tableDynamicSize = DeviceRom[pointer + 7];
                    var deviceType = DeviceRom[pointer + 10];
                    child.DriverUID = MetadataHelper.GetUidById(deviceType);
                    child.Driver = Drivers.FirstOrDefault(x => x.UID == child.DriverUID);
                    var config = new BitArray(new byte[] {DeviceRom[pointer + 9]});
                    pointer = pointer + 8 + tableDynamicSize; // указатель на следующую запись в таблице
                    if (config[4])
                    {
                        var localNoInPPU = Convert.ToInt32(config[3])*4 + Convert.ToInt32(config[2])*2 +  Convert.ToInt32(config[1]);
                        if (localNoInPPU == 0) // если это первое устройство в составе ППУ, то  создаем новое ППУ
                        {
                            ppuDevice = new Device();
                            ppuDevice.DriverUID = new Guid("E495C37A-A414-4B47-AF24-FEC1F9E43D86"); // АМ-4
                            ppuDevice.Driver = Drivers.FirstOrDefault(x => x.UID == ppuDevice.DriverUID);
                            device.Children.Add(ppuDevice);
                            ppuDevice.IntAddress = child.IntAddress;
                        }
                        ppuDevice.Children.Add(child);
                        //device.Children.FirstOrDefault(x => x.UID == ppuDevice.UID).Children.Add(child););
                        continue;
                    }
                    device.Children.Add(child);
                }
            }
            if ((pointer = DeviceRam[48] * 256 * 256 + DeviceRam[49] * 256 + DeviceRam[50]) != 0)  // РПИ
			{
				pointer -= 0x100;
				child = new Device();
				child.DriverUID = new Guid("641fa899-faa0-455b-b626-646e5fbe785a");
				child.Driver = Drivers.FirstOrDefault(x => x.UID == child.DriverUID);
                child.IntAddress = DeviceRom[pointer] + 256 * (DeviceRom[pointer + 1] + 1);
                int zoneNo = DeviceRom[pointer + 5] * 256 + DeviceRom[pointer + 6];
                if (zoneNo != 0)
                {
                    child.Zone =
                        zonePanelRelationsInfo.ZonePanelItems.FirstOrDefault(
                            x => (x.No == zoneNo) && x.PanelDevice.IntAddress == device.IntAddress).Zone;
                    child.ZoneUID = child.Zone.UID;
                }
				device.Children.Add(child);
			}
            if ((pointer = DeviceRam[54] * 256 * 256 + DeviceRam[55] * 256 + DeviceRam[56]) != 0) // АМ-1О
            {
                var count = DeviceRam[58] * 256 + DeviceRam[59]; // текущее число записей в таблице
                pointer -= 0x100;
                for (int i = 0; i < count; i++)
                {
                    child = new Device();
                    child.DriverUID = new Guid("efca74b2-ad85-4c30-8de8-8115cc6dfdd2");
                    child.Driver = Drivers.FirstOrDefault(x => x.UID == child.DriverUID);
                    child.IntAddress = DeviceRom[pointer] + 256 * (DeviceRom[pointer + 1] + 1);
                    int zoneNo = DeviceRom[pointer + 5] * 256 + DeviceRom[pointer + 6];
                    if (zoneNo != 0)
                    {
                        child.Zone =
                            zonePanelRelationsInfo.ZonePanelItems.FirstOrDefault(
                                x => (x.No == zoneNo) && x.PanelDevice.IntAddress == device.IntAddress).Zone;
                        child.ZoneUID = child.Zone.UID;
                    }
                    var tableDynamicSize = DeviceRom[pointer + 7];
                    pointer = pointer + 8 + tableDynamicSize; // указатель на следующую запись в таблице
                    device.Children.Add(child);
                }
            }

            if ((pointer = DeviceRam[96] * 256 * 256 + DeviceRam[97] * 256 + DeviceRam[98]) != 0) // АМ-1Т
            {
                var count = DeviceRam[100] * 256 + DeviceRam[101]; // текущее число записей в таблице
                pointer -= 0x100;
                for (int i = 0; i < count; i++)
                {
                    child = new Device();
                    child.DriverUID = new Guid("f5a34ce2-322e-4ed9-a75f-fc8660ae33d8");
                    child.Driver = Drivers.FirstOrDefault(x => x.UID == child.DriverUID);
                    child.IntAddress = DeviceRom[pointer] + 256 * (DeviceRom[pointer + 1] + 1);
                    int zoneNo = DeviceRom[pointer + 5] * 256 + DeviceRom[pointer + 6];
                    if (zoneNo != 0)
                    {
                        child.Zone =
                            zonePanelRelationsInfo.ZonePanelItems.FirstOrDefault(
                                x => (x.No == zoneNo) && x.PanelDevice.IntAddress == device.IntAddress).Zone;
                        child.ZoneUID = child.Zone.UID;
                    }
                    var tableDynamicSize = DeviceRom[pointer + 7];
                    pointer = pointer + 8 + tableDynamicSize; // указатель на следующую запись в таблице
                    device.Children.Add(child);
                }
            }

            if ((pointer = DeviceRam[60] * 256 * 256 + DeviceRam[61] * 256 + DeviceRam[62]) != 0)
				MessageBox.Show("Пока не определено");
            if ((pointer = DeviceRam[66] * 256 * 256 + DeviceRam[67] * 256 + DeviceRam[68]) != 0)
				MessageBox.Show("Пока не определено");
            if ((pointer = DeviceRam[72] * 256 * 256 + DeviceRam[73] * 256 + DeviceRam[74]) != 0)
				MessageBox.Show("Пока не определено");
            if ((pointer = DeviceRam[78] * 256 * 256 + DeviceRam[79] * 256 + DeviceRam[80]) != 0)
				MessageBox.Show("Пока не определено");
            if ((pointer = DeviceRam[84] * 256 * 256 + DeviceRam[85] * 256 + DeviceRam[86]) != 0)
				MessageBox.Show("Пока не определено");
            if ((pointer = DeviceRam[90] * 256 * 256 + DeviceRam[91] * 256 + DeviceRam[92]) != 0)
				MessageBox.Show("Пока не определено");
            if ((pointer = DeviceRam[96] * 256 * 256 + DeviceRam[97] * 256 + DeviceRam[98]) != 0)
				MessageBox.Show("Пока не определено");
            if ((pointer = DeviceRam[102] * 256 * 256 + DeviceRam[103] * 256 + DeviceRam[104]) != 0)
				MessageBox.Show("Пока не определено");
            if ((pointer = DeviceRam[114] * 256 * 256 + DeviceRam[115] * 256 + DeviceRam[116]) != 0)
				MessageBox.Show("Пока не определено");
			//if ((pointer = DeviceRom[126] * 256 * 256 + DeviceRom[127] * 256 + DeviceRom[128]) != 0) // Выход
			//{
			//}
            if ((pointer = DeviceRam[132] * 256 * 256 + DeviceRam[133] * 256 + DeviceRam[134]) != 0)
				MessageBox.Show("Пока не определено");
            if ((pointer = DeviceRam[138] * 256 * 256 + DeviceRam[139] * 256 + DeviceRam[140]) != 0)
				MessageBox.Show("Пока не определено");
            #endregion

            foreach (var childDevice in device.Children)
            {
                remoteDeviceConfiguration.Devices.Add(childDevice);
            }
            return remoteDeviceConfiguration;
        }

        public static void GetDeviceRom(Device device)
		{
            DeviceRom = new List<byte>();
			var bytes = new List<byte>();
            var end = _deviceRomLastIndex / 0x100;
            var count = _deviceRomLastIndex % 0x100;
            var request = new List<byte>();
            for (int i = 1; i < end; i++)
            {
                bytes = new List<byte>();
                bytes.AddRange(BitConverter.GetBytes(++_usbRequestNo).Reverse());
                bytes.Add(Convert.ToByte(device.Parent.IntAddress + 2));
                bytes.Add(Convert.ToByte(device.AddressOnShleif));
                bytes.Add(0x01);
                bytes.Add(0x52);
                bytes.AddRange(BitConverter.GetBytes(i * 0x100).Reverse());
                bytes.Add(Convert.ToByte(0xFF));
                request = SendCode(bytes).Result.FirstOrDefault().Data;
                request.RemoveRange(0, 7); // удаляем служебные символы
                DeviceRom.AddRange(request);
            }

            #region Читаем последний блок
            bytes = new List<byte>();
            bytes.AddRange(BitConverter.GetBytes(++_usbRequestNo).Reverse());
            bytes.Add(Convert.ToByte(device.Parent.IntAddress + 2));
            bytes.Add(Convert.ToByte(device.AddressOnShleif));
            bytes.Add(0x01);
            bytes.Add(0x52);
            bytes.AddRange(BitConverter.GetBytes(end * 0x100).Reverse());
            bytes.Add(Convert.ToByte(count));
            request = SendCode(bytes).Result.FirstOrDefault().Data;
            request.RemoveRange(0, 7); // удаляем служебные символы
            DeviceRom.AddRange(request);
            #endregion

            // Записываем БД DeviceRom в deviceRom.txt
            var deviceRomTxt = new StreamWriter("..\\deviceRom.txt");
            int j = 256;
            //foreach (var b in DeviceRom)
            //{
            //    deviceRomTxt.WriteLine("{0}\t{1}", j, b);
            //    j++;
            //}
            foreach (var b in DeviceRom)
            {
                deviceRomTxt.Write("{0} ", b.ToString("X2"));
                j++;
                if (j % 16 == 0)
                    deviceRomTxt.Write("\n");
            }
            deviceRomTxt.Close();
		}
        static int _deviceRamFirstIndex;
        static int _deviceRomLastIndex;

        private static void GetDeviceRamFirstAndRomLastIndex(Device device)
        {
            #region Находим адрес начального блока Ram
            var bytes = new List<byte>();
            bytes.AddRange(BitConverter.GetBytes(++_usbRequestNo).Reverse());
            bytes.Add(Convert.ToByte(device.Parent.IntAddress + 2));
            bytes.Add(Convert.ToByte(device.AddressOnShleif));
            bytes.Add(0x01);
            bytes.Add(0x57);
            var result = SendCode(bytes).Result.FirstOrDefault().Data;
            var begin = 256 * result[8] + result[9];
            _deviceRamFirstIndex = begin * 0x100;
            #endregion

            #region Находим адрес конечного блока Rom
            bytes = new List<byte>();
            bytes.AddRange(BitConverter.GetBytes(++_usbRequestNo).Reverse());
            bytes.Add(Convert.ToByte(device.Parent.IntAddress + 2));
            bytes.Add(Convert.ToByte(device.AddressOnShleif));
            bytes.Add(0x38);
            bytes.AddRange(BitConverter.GetBytes(_deviceRamFirstIndex).Reverse());
            bytes.Add(0x0B);
            result = SendCode(bytes).Result.FirstOrDefault().Data;
            _deviceRomLastIndex = result[13] * 256 * 256 + result[14] * 256 + result[15];
            #endregion
        }
        public static void GetDeviceRam(Device device)
		{
            var bytes = new List<byte>();
            var begin = _deviceRamFirstIndex / 0x100;

            #region Находим адрес конечного блока Ram и число байт в этом блоке

			bytes = new List<byte>();
			bytes.AddRange(BitConverter.GetBytes(++_usbRequestNo).Reverse());
			bytes.Add(Convert.ToByte(device.Parent.IntAddress + 2));
			bytes.Add(Convert.ToByte(device.AddressOnShleif));
			bytes.Add(0x38);
            bytes.AddRange(BitConverter.GetBytes(begin*0x100).Reverse());
			bytes.Add(0xFF);
			var result = SendCode(bytes).Result.FirstOrDefault().Data;
			result.RemoveRange(0, 7); // удаляем служебные байты (id - 4б, адрес приемника - 1б, адрес получателя - 1б, код функции - 1б)
            var end = 256 * result[9] + result[10];
            var count = result[11];

			#endregion Находим адрес конечного блока Rom и число байт в этом блоке

            var request = new List<byte>();
			#region Читаем все кроме последнего блока
            for (int i = begin + 1 ; i < end ; i++)
			{
				bytes = new List<byte>();
				bytes.AddRange(BitConverter.GetBytes(++_usbRequestNo).Reverse());
				bytes.Add(Convert.ToByte(device.Parent.IntAddress + 2));
				bytes.Add(Convert.ToByte(device.AddressOnShleif));
				bytes.Add(0x38);
                bytes.AddRange(BitConverter.GetBytes(i*0x100).Reverse());
				bytes.Add(0xFF);
                request = SendCode(bytes).Result.FirstOrDefault().Data;
                request.RemoveRange(0, 7); // удаляем служебные символы
                result.AddRange(request);
			}

			#endregion

			#region Читаем последний блок

			bytes = new List<byte>();
			bytes.AddRange(BitConverter.GetBytes(++_usbRequestNo).Reverse());
			bytes.Add(Convert.ToByte(device.Parent.IntAddress + 2));
			bytes.Add(Convert.ToByte(device.AddressOnShleif));
			bytes.Add(0x38);
            bytes.AddRange(BitConverter.GetBytes(end*0x100).Reverse());
            bytes.Add(Convert.ToByte(count));
            request = SendCode(bytes).Result.FirstOrDefault().Data;
            request.RemoveRange(0, 7); // удаляем служебные символы
            result.AddRange(request);
            #endregion
            DeviceRam = new List<byte>(result);

            // Записываем БД DeviceRam в deviceRam.txt
            var deviceRamTxt = new StreamWriter("..\\deviceRam.txt");
            int j = 0;
            foreach (var b in DeviceRam)
            {
                deviceRamTxt.WriteLine("{0}\t{1}", j, b);
                j++;
            }

            //foreach (var b in DeviceRam)
            //{
            //    deviceRamTxt.Write("{0} ", b.ToString("X2"));
            //    j++;
            //    if (j%16 == 0)
            //        deviceRamTxt.Write("\n");
            //}
            deviceRamTxt.Close();
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