using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.IO.Ports;
using System.Diagnostics;
using System.Threading;
using System.Collections;
using System.ComponentModel;

namespace TestUSB
{
	public partial class MainWindow : Window, INotifyPropertyChanged
	{
		UsbRunner usbRunner;
		List<UsbRequest> UsbRequests = new List<UsbRequest>();
		public List<JournalItem> JournalItems { get; set; }

		public MainWindow()
		{
			InitializeComponent();
			JournalItems = new List<JournalItem>();
			//JournalItems.Add(new JournalItem() { EventName = "event 1" });
			//JournalItems.Add(new JournalItem() { EventName = "event 2" });
			//JournalItems.Add(new JournalItem() { EventName = "event 3" });
			DataContext = this;
		}

		private void Window_Loaded(object sender, RoutedEventArgs e)
		{
			usbRunner = new UsbRunner();
			usbRunner.Open();
			usbRunner.DataRecieved += new Action<List<byte>>(usbRunner_DataRecieved);
		}

		object locker = new object();

		void usbRunner_DataRecieved(List<byte> bytes)
		{
			lock (locker)
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
				var requestNoList = bytes.GetRange(0, 4);
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

				var message = BytesToString(bytes);
				Trace.WriteLine("\n" + message);

				var timeBytes = bytes.GetRange(1, 4);
				journalItem.No = usbRequest.Id;
				journalItem.Date = TimeParceHelper.Parce(timeBytes);
				journalItem.EventName = MetadataHelper.GetEventByCode(bytes[0]);
				Trace.WriteLine(journalItem.Date + " " + journalItem.EventName);
				journalItem.ShleifNo = bytes[6] + 1;
				journalItem.IntType = bytes[7];
				journalItem.Address = bytes[8];
				journalItem.State = bytes[9];
				journalItem.ZoneNo = bytes[10] * 256 + bytes[11];
				journalItem.DescriptorNo = bytes[12] * 256 * 256 + bytes[13] * 256 + bytes[14];

				JournalItems.Add(journalItem);
			}
		}

		private void Window_Closed(object sender, EventArgs e)
		{
			usbRunner.Close();
		}

		int UsbRequestNo = 1;

		private void Button_Click(object sender, RoutedEventArgs e)
		{
			MetadataHelper.Initialize();
			for (int i = 1; i < 100; i++)
			{
				lock (locker)
				{
					GetJournalItem(i);
				}
				Thread.Sleep(100);
			}
		}

		void GetJournalItem(int journalNo)
		{
			UsbRequestNo++;
			var bytes = new List<byte>();
			bytes.AddRange(BitConverter.GetBytes(UsbRequestNo).Reverse());
			bytes.Add(0x04);
			bytes.Add(0x01);
			bytes.Add(0x01);
			bytes.Add(0x20);
			bytes.Add(0x00);
			bytes.AddRange(BitConverter.GetBytes(journalNo).Reverse());

			var usbRequest = new UsbRequest()
			{
				Id = UsbRequestNo,
				UsbAddress = 0x04,
				SelfAddress = 0x01,
				FuncCode = 0x01
			};
			UsbRequests.Add(usbRequest);

			for (int i = 0; i < 5; i++)
			{
				usbRunner.Send(bytes);
				Thread.Sleep(10);
			}
		}

		string BytesToString(IEnumerable<byte> bytes)
		{
			var result = "";
			foreach (var b in bytes)
			{
				var hexByte = b.ToString("x2");
				result += hexByte + " ";
			}
			return result;
		}

		private void Button_Click_1(object sender, RoutedEventArgs e)
		{
			JournalItems = new List<JournalItem>(JournalItems);
			OnPropertyChanged("JournalItems");
		}

		public event PropertyChangedEventHandler PropertyChanged;
		void OnPropertyChanged(string propertytName)
		{
			if (PropertyChanged != null)
				PropertyChanged(this, new PropertyChangedEventArgs(propertytName));
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