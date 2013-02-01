using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Diagnostics;
using System.Collections;
using Device = FiresecAPI.Models.Device;
using FiresecAPI;

namespace ServerFS2
{
    public static class ServerHelper 
    {
        static readonly object Locker = new object();
        static readonly UsbRunner UsbRunner;
        static readonly List<UsbRequest> UsbRequests = new List<UsbRequest>();
        static ServerHelper()
        {
            MetadataHelper.Initialize();
            UsbRunner = new UsbRunner();
            UsbRunner.Open();
        }
        private static int _usbRequestNo;
        public static void ParseJournal(List<byte> bytes, List<JournalItem> journalItems)
        {
            lock (Locker)
            {
                var journalItem = new JournalItem();
                var allbytes = new List<byte>(bytes);
                if (bytes.Count < 2)
                    return;
                if (bytes.Count < 4)
                    return;
                var requestNo = bytes[0] * 256 * 256 * 256 + bytes[1] * 256 * 256 + bytes[2] * 256 + bytes[3];
                bytes.RemoveRange(0, 4);
                var usbRequest = UsbRequests.FirstOrDefault(x => x.Id == requestNo);
                if (usbRequest == null)
                    return;
                UsbRequests.Remove(usbRequest);
                if (bytes.Count < 2)
                    return;
                if (bytes[0] != usbRequest.UsbAddress)
                    return;
                if (bytes[1] != usbRequest.SelfAddress)
                    return;
                bytes.RemoveRange(0, 2);
                if (bytes.Count < 1)
                    return;
                var funcCodeBitArray = new BitArray(bytes.GetRange(0, 1).ToArray());
                if (funcCodeBitArray.Get(6) == false)
                    return;
                if (funcCodeBitArray.Get(7))
                {
                    Trace.WriteLine("\n Error = " + bytes[1]);
                    return;
                }
                byte funcCode = bytes[0];
                funcCode = (byte)(funcCode << 2);
                funcCode = (byte)(funcCode >> 2);
                if (funcCode != usbRequest.FuncCode)
                    return;
                bytes.RemoveRange(0, 1);
                if (bytes.Count < 32)
                    return;
                var journalParser = new JournalParser(bytes, allbytes);
                var timeBytes = bytes.GetRange(1, 4);
                journalItem.Date = TimeParceHelper.Parce(timeBytes);
                var eventName = MetadataHelper.GetEventByCode(bytes[0]);
                journalItem.EventName = MetadataHelper.GetExactEventForFlag(eventName, bytes[5]);
                Trace.WriteLine(journalItem.Date + " " + journalItem.EventName);
                journalItem.Flag = bytes[5];
                journalItem.ShleifNo = bytes[6];
                journalItem.IntType = bytes[7];
                journalItem.Address = bytes[8];
                journalItem.State = bytes[9];
                journalItem.ZoneNo = bytes[10] * 256 + bytes[11];
                journalItem.DescriptorNo = bytes[12] * 256 * 256 + bytes[13] * 256 + bytes[14];
                journalItem = journalParser.Parce();
                journalItems.Add(journalItem);
            }
        }
        static OperationResult<Response> SendCode(List<byte> bytes)
        {
            var usbRequest = new UsbRequest()
            {
                Id = _usbRequestNo,
                UsbAddress = bytes[4],
                SelfAddress = bytes[5],
                FuncCode = bytes[6]
            };
            UsbRequests.Add(usbRequest);
            return UsbRunner.AddRequest(bytes);
        }
		public static List<JournalItem> GetSecJournalItems2Op(Device device)
        {
            int lastindex = GetLastSecJournalItemId2Op(device);
			var journlaItems = new List<JournalItem>();
            for (int i = 0; i <= lastindex; i++)
            {
                var bytes = new List<byte>();
                bytes.AddRange(BitConverter.GetBytes(++_usbRequestNo).Reverse());
                bytes.Add(Convert.ToByte(device.UsbChannel + 2));
                bytes.Add(Convert.ToByte(device.Address));
                bytes.Add(0x01);
                bytes.Add(0x20);
                bytes.Add(0x02);
                bytes.AddRange(BitConverter.GetBytes(i).Reverse());
				ParseJournal(SendCode(bytes).Result.Data, journlaItems);
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
            bytes.Add(Convert.ToByte(device.UsbChannel + 2));
            bytes.Add(Convert.ToByte(device.Address));
            bytes.Add(0x01);
            bytes.Add(0x21);
            bytes.Add(0x02);
            try
            {
                var lastindex = SendCode(bytes);
                int li = 256 * lastindex.Result.Data[9] + lastindex.Result.Data[10];
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
            if (device.Name == "Прибор РУБЕЖ-2ОП")
                return GetLastJournalItemId(device) + GetLastSecJournalItemId2Op(device);
            var bytes = new List<byte>();
            bytes.AddRange(BitConverter.GetBytes(++_usbRequestNo).Reverse());
            bytes.Add(Convert.ToByte(device.UsbChannel + 2));
            bytes.Add(Convert.ToByte(device.Address));
            bytes.Add(0x01);
            bytes.Add(0x24);
            bytes.Add(0x00);
            try
            {
                var firecount = SendCode(bytes);
                int fc = 256 * firecount.Result.Data[7] + firecount.Result.Data[8];
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
            if (device.Name == "Прибор РУБЕЖ-2ОП")
                return 0;
            return GetLastJournalItemId(device) - GetJournalCount(device) + 1;
        }
        public static int GetLastJournalItemId(Device device)
        {
            var bytes = new List<byte>();
            bytes.AddRange(BitConverter.GetBytes(++_usbRequestNo).Reverse());
            bytes.Add(Convert.ToByte(device.UsbChannel + 2));
            bytes.Add(Convert.ToByte(device.Address));
            bytes.Add(0x01);
            bytes.Add(0x21);
            bytes.Add(0x00);
            try
            {
                var lastindex = SendCode(bytes);
                int li = 256 * lastindex.Result.Data[9] + lastindex.Result.Data[10];
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
            if (device.Name == "Прибор РУБЕЖ-2ОП")
            {
                firstindex = 0;
                secJournalItems = GetSecJournalItems2Op(device);
            }
            for (int i = firstindex; i <= lastindex; i++)
            {
                var bytes = new List<byte>();
                bytes.AddRange(BitConverter.GetBytes(++_usbRequestNo).Reverse());
                bytes.Add(Convert.ToByte(device.UsbChannel + 2));
                bytes.Add(Convert.ToByte(device.Address));
                bytes.Add(0x01);
                bytes.Add(0x20);
                bytes.Add(0x00);
                bytes.AddRange(BitConverter.GetBytes(i).Reverse());
				ParseJournal(SendCode(bytes).Result.Data, journalItems);
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
        public static void AutoDetectDevice(ObservableCollection<Device> devices)
        {
            byte deviceCount;
            for (byte sleif = 0x03; sleif <= 0x04; sleif++)
                for (deviceCount = 1; deviceCount < 128; deviceCount++)
                {
                    var bytes = new List<byte>();
                    bytes.AddRange(BitConverter.GetBytes(++_usbRequestNo).Reverse());
                    bytes.Add(sleif);
                    bytes.Add(deviceCount);
                    bytes.Add(0x3C);
                    var inputBytes = SendCode(bytes).Result.Data;
                    if (inputBytes[6] == 0x7C) // Если по данному адресу найдено устройство, узнаем тип устройство и его версию ПО
                    {
                        var device = new Device();
                        device.Address = inputBytes[5].ToString();
                        bytes = new List<byte>();
                        bytes.AddRange(BitConverter.GetBytes(++_usbRequestNo).Reverse());
                        bytes.Add(sleif);
                        bytes.Add(deviceCount);
                        bytes.Add(0x01);
                        bytes.Add(0x03);
                        inputBytes = SendCode(bytes).Result.Data;
                        device.Name = DriversHelper.GetDriverNameByType(inputBytes[7]);

                        bytes = new List<byte>();
                        bytes.AddRange(BitConverter.GetBytes(++_usbRequestNo).Reverse());
                        bytes.Add(sleif);
                        bytes.Add(deviceCount);
                        bytes.Add(0x01);
                        bytes.Add(0x12);
                        inputBytes = SendCode(bytes).Result.Data;
                        device.Version = inputBytes[7].ToString("X2") + "." + inputBytes[8].ToString("X2");

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
                        inputBytes = SendCode(bytes).Result.Data;
						if (inputBytes.Count >= 18)
						{
							for (int i = 7; i <= 18; i++)
								device.SerialNo += inputBytes[i] - 0x30 + ".";
							device.SerialNo = device.SerialNo.Remove(device.SerialNo.Length - 1);
						}
                        device.UsbChannel = sleif - 2;
                        devices.Add(device);
                    }
                }
        }
        public static List<byte> SendRequest(List<byte> bytes)
        {
            return SendCode(bytes).Result.Data;
        }
        public static void GetDeviceParameters(Device device)
        {
			var bytes = new List<byte>();
			bytes.AddRange(BitConverter.GetBytes(++_usbRequestNo).Reverse());
			bytes.Add((byte)(device.UsbChannel + 2));

        }
    }
    class UsbRequest
    {
        public int Id { get; set; }
        public int UsbAddress { get; set; }
        public int SelfAddress { get; set; }
        public int FuncCode { get; set; }
    }
}