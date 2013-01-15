using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Diagnostics;
using System.Threading;
using System.Collections;
using System.ComponentModel;
using FiresecAPI;

namespace TestUSB
{
	public partial class MainWindow : INotifyPropertyChanged
	{
        readonly object _locker = new object();
		UsbRunner _usbRunner;
        int _usbRequestNo = 1;
	    readonly List<UsbRequest> _usbRequests = new List<UsbRequest>();
		public List<JournalItem> JournalItems { get; set; }
		public MainWindow()
		{
			InitializeComponent();
			JournalItems = new List<JournalItem>();
			DataContext = this;
		}
		private void WindowLoaded(object sender, RoutedEventArgs e)
		{
			_usbRunner = new UsbRunner();
			_usbRunner.Open();
		}
		void ParseJournal(List<byte> bytes)
		{
			lock (_locker)
			{
				var journalItem = new JournalItem();
				if (bytes.Count < 2)
					return;
				if ((bytes.First() != 0x7E) || (bytes.Last() != 0x3E))
					return;
				bytes.RemoveAt(0);
				bytes.RemoveAt(bytes.Count - 1);
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
					Trace.WriteLine("\n Error = " + bytes[1].ToString());
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
				var journalParser = new JournalParser(bytes);
				var timeBytes = bytes.GetRange(1, 4);
				journalItem.No = usbRequest.Id;
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
				JournalItems.Add(journalItem);
			}
		}
		private void WindowClosed(object sender, EventArgs e)
		{
			_usbRunner.Close();
		}
		private void ReadJournalClick(object sender, RoutedEventArgs e)
        {
            (new Thread(delegate()
            {
                MetadataHelper.Initialize();
                int fc = GetFireJournalCount();
                for (int i = 0; i < fc; i++)
                    GetFireJournalItem(i);
                JournalItems.Add(new JournalItem() { EventName = JournalItems.Count.ToString() });
                int sc = GetSecJournalCount();
                for (int i = 0; i < sc; i++)
                    GetSecJournalItem(i);
                JournalItems.Add(new JournalItem() { EventName = JournalItems.Count.ToString() });
            })).Start();
        }
        private void ShowJournalClick(object sender, RoutedEventArgs e)
        {
            JournalItems = new List<JournalItem>(JournalItems);
            OnPropertyChanged("JournalItems");
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
            _usbRequestNo++;
            return _usbRunner.AddRequest(bytes);
        }
        int GetFireJournalCount()
        {
            var bytes = new List<byte>();
            bytes.AddRange(BitConverter.GetBytes(_usbRequestNo).Reverse());
            bytes.Add(0x04);
            bytes.Add(0x01);
            bytes.Add(0x01);
            bytes.Add(0x21);
            bytes.Add(0x00);

            try
            {
                var firecount = SendCode(bytes);
                int fc = 256 * firecount.Result.Data[10] + firecount.Result.Data[11];
                return fc;
            }
            catch (NullReferenceException ex)
            {
                MessageBox.Show(ex.Message);
                throw;
            }
        }
        int GetSecJournalCount()
        {
            var bytes = new List<byte>();
            bytes.AddRange(BitConverter.GetBytes(_usbRequestNo).Reverse());
            bytes.Add(0x04);
            bytes.Add(0x01);
            bytes.Add(0x01);
            bytes.Add(0x21);
            bytes.Add(0x02);

            try
            {
                var seccount = SendCode(bytes);
                int sc = 256 * seccount.Result.Data[10] + seccount.Result.Data[11];
                return sc;
            }
            catch (NullReferenceException ex)
            {
                MessageBox.Show(ex.Message);
                throw;
            }
        }
		void GetFireJournalItem(int journalNo)
		{
			_usbRequestNo++;
			var bytes = new List<byte>();
			bytes.AddRange(BitConverter.GetBytes(_usbRequestNo).Reverse());
			bytes.Add(0x04);
			bytes.Add(0x01);
			bytes.Add(0x01);
			bytes.Add(0x20);
			bytes.Add(0x00);
			bytes.AddRange(BitConverter.GetBytes(journalNo).Reverse());
            ParseJournal(SendCode(bytes).Result.Data);
		}
        void GetSecJournalItem(int journalNo)
        {
            _usbRequestNo++;
            var bytes = new List<byte>();
            bytes.AddRange(BitConverter.GetBytes(_usbRequestNo).Reverse());
            bytes.Add(0x04);
            bytes.Add(0x01);
            bytes.Add(0x01);
            bytes.Add(0x20);
            bytes.Add(0x02);
            bytes.AddRange(BitConverter.GetBytes(journalNo).Reverse());
            ParseJournal(SendCode(bytes).Result.Data);
        }
		public event PropertyChangedEventHandler PropertyChanged;
		void OnPropertyChanged(string propertytName)
		{
			if (PropertyChanged != null)
				PropertyChanged(this, new PropertyChangedEventArgs(propertytName));
		}
        public static Guid GetUidById(ushort driverTypeNo)
        {
            switch (driverTypeNo)
            {
                case 0x7D: return new Guid("043fbbe0-8733-4c8d-be0c-e5820dbf7039"); // "Модуль дымоудаления-1"
                case 0x51: return new Guid("dba24d99-b7e1-40f3-a7f7-8a47d4433392"); // "Пожарная адресная метка АМ-1"
                case 0x34: return new Guid("efca74b2-ad85-4c30-8de8-8115cc6dfdd2"); // "Охранная адресная метка АМ1-О"
                case 0xD2: return new Guid("f5a34ce2-322e-4ed9-a75f-fc8660ae33d8"); // "Технологическая адресная метка АМ1-Т"
                case 0x50: return new Guid("d8997f3b-64c4-4037-b176-de15546ce568"); // "Пожарная адресная метка АМП"
                case 0x70: return new Guid("8bff7596-aef4-4bee-9d67-1ae3dc63ca94"); // "Шкаф управления насосом"
                case 0x71: return new Guid("4935848f-0084-4151-a0c8-3a900e3cb5c5"); // "Шкаф управления задвижкой"
                case 0x60: return new Guid("37f13667-bc77-4742-829b-1c43fa404c1f"); // "Пожарный комбинированный извещатель ИП212/101-64-А2R1"
                case 0x55: return new Guid("641fa899-faa0-455b-b626-646e5fbe785a"); // "Ручной извещатель ИПР513-11"
                case 0x62: return new Guid("799686b6-9cfa-4848-a0e7-b33149ab940c"); // "Пожарный тепловой извещатель ИП 101-29-A3R1"
                case 0x76: return new Guid("33a85f87-e34c-45d6-b4ce-a4fb71a36c28"); // "Модуль пожаротушения МПТ-1"
                case 0x74: return new Guid("2d078d43-4d3b-497c-9956-990363d9b19b"); // "Модуль речевого оповещения МРО-2"
                case 0x75: return new Guid("4a60242a-572e-41a8-8b87-2fe6b6dc4ace"); // "Релейный исполнительный модуль РМ-1"
                case 0x61: return new Guid("1e045ad6-66f9-4f0b-901c-68c46c89e8da"); // "Пожарный дымовой извещатель ИП 212-64"
                case 0x103: return new Guid("200EED4B-3402-45B4-8122-AE51A4841E18"); // "Индикатор ГК"
                case 0x104: return new Guid("DEAA33C2-0EAA-4D4D-BA31-FCDBE0AD149A"); // "Линия ГК"
                case 0x105: return new Guid("1AC85436-61BC-441B-B6BF-C6A0FA62748B"); // "Реле ГК"
                case 0x102: return new Guid("4993E06C-85D1-4F20-9887-4C5F67C450E8"); // "Контроллер адресных устройств"
                default: return new Guid("00000000-0000-0000-0000-000000000000"); // "Неизвестное устройство"
            }
        }
        private void ShowFireJournalCountClick(object sender, RoutedEventArgs e)
        {
            MessageBox.Show(GetFireJournalCount().ToString());
        }
        private void ShowSecurityJournalCountClick(object sender, RoutedEventArgs e)
        {
            MessageBox.Show(GetSecJournalCount().ToString());
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