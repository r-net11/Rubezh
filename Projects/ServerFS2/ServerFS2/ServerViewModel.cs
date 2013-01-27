using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Diagnostics;
using System.Threading;
using System.Collections;
using System.Windows.Controls;
using FiresecAPI;

namespace ServerFS2
{
    public class ServerViewModel : BaseViewModel  
    {
        readonly object _locker = new object();
        readonly UsbRunner _usbRunner;
        readonly List<UsbRequest> _usbRequests = new List<UsbRequest>();

        public ObservableCollection<Device> Devices { get; private set; }
        public ServerViewModel()
        {
            ReadJournalCommand = new RelayCommand(OnReadJournal);
            SendRequestCommand = new RelayCommand(OnSendRequest);
            AutoDetectDeviceCommand = new RelayCommand(OnAutoDetectDevice);
            
            MetadataHelper.Initialize();
            Devices = new ObservableCollection<Device>();
            _usbRunner = new UsbRunner();
            _usbRunner.Open();
        }
        private int _usbRequestNo;

        private Device _selectedDevice;
        public Device SelectedDevice
        {
            get { return _selectedDevice; }
            set
            {
                _selectedDevice = value;
                OnPropertyChanged("SelectedDevice");
            }
        }

        private string _textBoxRequest;
        public string TextBoxRequest
        {
            get { return _textBoxRequest; }
            set
            {
                _textBoxRequest = value;
                OnPropertyChanged("TextBoxRequest");
            }
        }

        private string _textBoxResponse;
        public string TextBoxResponse
        {
            get { return _textBoxResponse; }
            set
            {
                _textBoxResponse = value;
                OnPropertyChanged("TextBoxResponse");
            }
        }

        public RelayCommand ReadJournalCommand { get; private set; }
        void OnReadJournal()
        {
            var device = SelectedDevice;
            GetJournalItems(device);
            ShowJournal(device);
        }

        void ShowJournal(Device device)
        {
            var win = new Window();
            var dataGrid = new DataGrid { ItemsSource = device.JournalItems };
            win.Content = dataGrid;
            win.Show();
        }
        public RelayCommand SendRequestCommand { get; private set; }
        private void OnSendRequest()
        {
            var bytes = TextBoxRequest.Split()
                   .Select(t => byte.Parse(t, NumberStyles.AllowHexSpecifier)).ToList();
            var inbytes = SendCode(bytes).Result.Data;
            foreach (var b in inbytes)
                TextBoxResponse += b.ToString("X2") + " ";
        }
        public RelayCommand AutoDetectDeviceCommand { get; private set; }
        private void OnAutoDetectDevice()
        {
            AutoDetectDevice();
        }

        void ParseJournal(List<byte> bytes, List<JournalItem> journalItems)
        {
            lock (_locker)
            {
                var journalItem = new JournalItem();
                var allbytes = new List<byte>(bytes);
                if (bytes.Count < 2)
                    return;
                if (bytes.Count < 4)
                    return;
                var requestNo = bytes[0] * 256 * 256 * 256 + bytes[1] * 256 * 256 + bytes[2] * 256 + bytes[3];
                bytes.RemoveRange(0, 4);
                var usbRequest = _usbRequests.FirstOrDefault(x => x.Id == requestNo);
                if (usbRequest == null)
                    return;
                _usbRequests.Remove(usbRequest);
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
        OperationResult<Response> SendCode(List<byte> bytes)
        {
            var usbRequest = new UsbRequest()
            {
                Id = _usbRequestNo,
                UsbAddress = bytes[4],
                SelfAddress = bytes[5],
                FuncCode = bytes[6]
            };
            _usbRequests.Add(usbRequest);
            return _usbRunner.AddRequest(bytes);
        }
        void GetSecJournalItems2Op(Device device)
        {
            int lastindex = GetLastSecJournalItemId2Op(device);
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
                ParseJournal(SendCode(bytes).Result.Data, device.SecJournalItems);
            }
            device.SecJournalItems = device.SecJournalItems.OrderByDescending(x => x.IntDate).ToList();
            int no = 0;
            foreach (var item in device.SecJournalItems)
            {
                no++;
                item.No = no;
            }
        }
        int GetLastSecJournalItemId2Op(Device device)
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
        int GetJournalCount(Device device)
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
        int GetFirstJournalItemId(Device device)
        {
            if (device.Name == "Прибор РУБЕЖ-2ОП")
                return 0;
            return GetLastJournalItemId(device) - GetJournalCount(device) + 1;
        }
        int GetLastJournalItemId(Device device)
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
        void GetJournalItems(Device device)
        {
            int lastindex = GetLastJournalItemId(device);
            int firstindex = GetFirstJournalItemId(device);
            if (device.Name == "Прибор РУБЕЖ-2ОП")
            {
                firstindex = 0;
                GetSecJournalItems2Op(device);
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
                ParseJournal(SendCode(bytes).Result.Data, device.JournalItems);
            }
            //device.JournalItems = device.JournalItems.OrderByDescending(x => x.IntDate).ToList();
            
            int no = 0;
            foreach (var item in device.JournalItems)
            {
                no++;
                item.No = no;
            }
            device.SecJournalItems.ForEach(x => device.JournalItems.Add(x));
        }
        void AutoDetectDevice()
        {
            Devices = new ObservableCollection<Device>();
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
                        for (int i = 7; i <= 18; i++)
                            device.SerialNo += inputBytes[i] - 0x30 + ".";
                        device.SerialNo = device.SerialNo.Remove(device.SerialNo.Length - 1);
                        device.UsbChannel = sleif - 2;
                        Devices.Add(device);
                        OnPropertyChanged("Devices");
                    }
                }
        }
    }
    public class UsbRequest
    {
        public int Id { get; set; }
        public int UsbAddress { get; set; }
        public int SelfAddress { get; set; }
        public int FuncCode { get; set; }
    }
}