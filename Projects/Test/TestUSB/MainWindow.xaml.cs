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

namespace TestUSB
{
	public partial class MainWindow : Window
	{
		UsbRunner usbRunner;
		List<UsbRequest> UsbRequests = new List<UsbRequest>();

		public MainWindow()
		{
			InitializeComponent();
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
					//Dispatcher.BeginInvoke(new Action(() =>
					//{
					//    textBox1.Text += "\n Error = " + bytes[1].ToString();
					//}));
					return;
				}
				byte funcCode = bytes[0];
				funcCode = (byte)(funcCode << 2);
				funcCode = (byte)(funcCode >> 2);
				if (funcCode != usbRequest.FuncCode)
					return;
				bytes.RemoveRange(0, 1);

				if (bytes.Count < 5)
					return;

				var message = WriteTrace("", bytes);
				Trace.WriteLine("\n" + message);
				//Dispatcher.BeginInvoke(new Action(() =>
				//{
				//    textBox1.Text += "\n" + message;
				//}));

				var timeBytes = bytes.GetRange(1, 4);

				var bitsExtracter = new BitsExtracter(timeBytes);

				Trace.WriteLine("timeBits = " + bitsExtracter.ToString());
				//Dispatcher.BeginInvoke(new Action(() =>
				//{
				//    textBox1.Text += "\n timeBits = " + bitsExtracter.ToString();
				//}));

				var day = bitsExtracter.Get(0, 4);
				var month = bitsExtracter.Get(5, 8);
				var year = bitsExtracter.Get(9, 14);
				var hour = bitsExtracter.Get(15, 19);
				var min = bitsExtracter.Get(20, 25);
				var sec = bitsExtracter.Get(26, 31);

				string eventName = EventsHelper.Get(bytes[0]);

				Trace.WriteLine(usbRequest.Id.ToString() + ": " + day.ToString() + "/" + month.ToString() + "/" + (year + 2000).ToString() + " " + hour.ToString() + ":" + min.ToString() + ":" + sec.ToString() +
						" " + eventName);
				//Dispatcher.BeginInvoke(new Action(() =>
				//{
				//    textBox1.Text += "\n" + day.ToString() + "/" + month.ToString() + "/" + (year + 2000).ToString() + " " + hour.ToString() + ":" + min.ToString() + ":" + sec.ToString() +
				//        " " + eventName;
				//}));
			}
		}

		private void Window_Closed(object sender, EventArgs e)
		{
			usbRunner.Close();
		}

		int UsbRequestNo = 1;

		private void Button_Click(object sender, RoutedEventArgs e)
		{
			for (int i = 1; i < 1000; i++)
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

		string WriteTrace(string name, IEnumerable<byte> bytes)
		{
			var result = "";
			//Trace.WriteLine("");
			//Trace.WriteLine(name + ": ");
			foreach (var b in bytes)
			{
				var hexByte = b.ToString("x2");
				//Trace.Write(hexByte + " ");
				result += hexByte + " ";
			}
			return result;
		}

		private void Button_Click_1(object sender, RoutedEventArgs e)
		{
			var timeBytes = new List<byte>() { 0x82, 0x97, 0x95, 0x08 };

			var bitsExtracter = new BitsExtracter(timeBytes);
			textBox1.Text += "\n timeBits = " + bitsExtracter.ToString();

			var day = bitsExtracter.Get(0, 4);
			var month = bitsExtracter.Get(5, 8);
			var year = bitsExtracter.Get(9, 14);
			var hour = bitsExtracter.Get(15, 19);
			var min = bitsExtracter.Get(20, 25);
			var sec = bitsExtracter.Get(26, 31);

			Dispatcher.BeginInvoke(new Action(() =>
			{
				textBox1.Text += "\n" + day.ToString() + "/" + month.ToString() + "/" + year.ToString() + " " + hour.ToString() + ":" + min.ToString() + ":" + sec.ToString();
			}));
		}

		int BitArrayToInt(BitArray bitArray, int startIndex, int endIndex)
		{
			int result = 0;
			for (int i = startIndex; i <= endIndex; i++)
			{
				var boolValue = bitArray.Get(bitArray.Count - 1 - i);
				var intValue = boolValue ? 1 : 0;
				result += intValue << (i - startIndex);
			}
			return result;
		}
	}

	public class BitsExtracter
	{
		List<bool> bits;
		public BitsExtracter(List<byte> bytes)
		{
			bits = new List<bool>();
			foreach (var b in bytes)
			{
				bits.Add(b.GetBit(7));
				bits.Add(b.GetBit(6));
				bits.Add(b.GetBit(5));
				bits.Add(b.GetBit(4));
				bits.Add(b.GetBit(3));
				bits.Add(b.GetBit(2));
				bits.Add(b.GetBit(1));
				bits.Add(b.GetBit(0));
			}
		}

		public int Get(int startIndex, int endIndex)
		{
			int result = 0;
			for (int i = startIndex; i <= endIndex; i++)
			{
				var boolValue = bits[i];
				var intValue = boolValue ? 1 : 0;
				result += intValue << (endIndex - i);
			}
			return result;
		}

		public override string ToString()
		{
			string timeBits = "";
			foreach (var b in bits)
			{
				var intValue = (bool)b ? 1 : 0;
				timeBits += intValue.ToString();
			}
			return timeBits;
		}
	}

	public static class BitHelper
	{
		public static bool GetBit(this byte b, int bitNumber)
		{
			return (b & (1 << bitNumber)) != 0;
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