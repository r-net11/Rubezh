using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using ServerFS2;
using FiresecAPI.Models;
using System.Diagnostics;
using ServerFS2.DataBase;

namespace MonitorClientFS2
{
    public class MonitoringProcessor
    {
        int UsbRequestNo;
        static readonly object Locker = new object();
        List<DeviceResponceRelation> DeviceResponceRelations = new List<DeviceResponceRelation>();

        public MonitoringProcessor()
        {
            ServerHelper.UsbRunner.NewResponse += new Action<Response>(UsbRunner_NewResponse);
            var thread = new Thread(OnRun);
            thread.Start();
        }

        void OnRun()
        {
            while (true)
            {
                foreach (var device in ConfigurationManager.DeviceConfiguration.Devices)
                {
                    if (device.Driver.IsPanel)
                    {
                        var deviceResponceRelation = new DeviceResponceRelation(device, ++UsbRequestNo);
                        DeviceResponceRelations.Add(deviceResponceRelation);
                        SendByteCommand(new List<byte> { 0x21, 0x00 }, device, deviceResponceRelation.Id);
                    }
                }
                Thread.Sleep(1000);
            }
        }

        void SendByteCommand(List<byte> commandBytes, Device device, int requestId)
        {
            var bytes = new List<byte>();
            bytes.AddRange(BitConverter.GetBytes(requestId).Reverse());
            bytes.Add(GetSheifByte(device));
            bytes.Add(Convert.ToByte(device.AddressOnShleif));
            bytes.Add(0x01);
            bytes.AddRange(commandBytes);
            ServerHelper.SendCodeAsync(bytes);
        }

        byte GetSheifByte(Device device)
        {
            return 0x03;
        }

        void UsbRunner_NewResponse(Response response)
        {
            var deviceResponceRelation = DeviceResponceRelations.FirstOrDefault(x => x.Id == response.Id);
            if (deviceResponceRelation != null)
            {
                var lastDeviceRecord = 256 * response.Data[9] + response.Data[10];
                if (lastDeviceRecord > deviceResponceRelation.LastDisplayedRecord)
                {
                    ReadNewItems(deviceResponceRelation.Device, lastDeviceRecord, deviceResponceRelation.LastDisplayedRecord);
                    deviceResponceRelation.LastDisplayedRecord = lastDeviceRecord;
                    //XmlJournalHelper.SetLastId(this);
                }

                DeviceResponceRelations.RemoveAll(x => x.Device == deviceResponceRelation.Device);
            }
        }

        void ReadNewItems(Device device, int lastDeviceRecord, int lastDisplayedRecord)
        {
            Trace.Write("Дочитываю записи с " + lastDisplayedRecord.ToString() + " до " + lastDeviceRecord.ToString() + "с прибора " + device.PresentationName + "\n");
            var newItems = GetJournalItems(device, lastDeviceRecord, lastDisplayedRecord + 1);
            foreach (var journalItem in newItems)
            {
                if (journalItem != null)
                {
                    //AddToJournalObservable(journalItem);
                    DBJournalHelper.AddJournalItem(journalItem);
                    Trace.Write(device.PresentationAddress + " ");
                }
            }
            Trace.WriteLine(" дочитал");
        }

        List<FSJournalItem> GetJournalItems(Device device, int lastindex, int firstindex)
        {
            var journalItems = new List<FSJournalItem>();
            for (int i = firstindex; i <= lastindex; i++)
                journalItems.Add(ReadItem(device, i));
            return journalItems;
        }
        FSJournalItem ReadItem(Device device, int i)
        {
            List<byte> bytes = new List<byte> { 0x20, 0x00 };
            bytes.AddRange(BitConverter.GetBytes(i).Reverse());
            return SendBytesAndParse(bytes, device);
        }
        FSJournalItem SendBytesAndParse(List<byte> bytes, Device device)
        {
            var data = SendByteCommand(bytes, device);
            lock (Locker)
            {
                return JournalParser.FSParce(data.Data);
            }
        }
        ServerFS2.Response SendByteCommand(List<byte> commandBytes, Device device)
        {
            var bytes = new List<byte>();
            bytes.AddRange(BitConverter.GetBytes(++UsbRequestNo).Reverse());
            bytes.Add(GetSheifByte(device));
            bytes.Add(Convert.ToByte(device.AddressOnShleif));
            bytes.Add(0x01);
            bytes.AddRange(commandBytes);
            lock (Locker)
            {
                return ServerHelper.SendCode(bytes).Result.FirstOrDefault();
            }
        }
    }

    class DeviceResponceRelation
    {
        public DeviceResponceRelation(Device device, int id)
        {
            Device = device;
            Id = id;
            LastDisplayedRecord = XmlJournalHelper.GetLastId2(Device);
        }
        public Device Device { get; set; }
        public int Id { get; set; }
        public int LastDisplayedRecord { get; set; }
    }
}