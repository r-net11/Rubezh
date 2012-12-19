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

		int BitArrayToInt(System.Collections.BitArray bitArray, int startIndex, int endIndex)
		{
			int result = 0;
			for(int i = startIndex; i <= endIndex; i++)
			{
				var boolValue = bitArray.Get(i);
				var intValue = boolValue ? 1 : 0;
				result += intValue << (i - startIndex);
			}
			return result;
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
					return;
				byte funcCode = bytes[0];
				funcCode = (byte)(funcCode << 2);
				funcCode = (byte)(funcCode >> 2);
				if (funcCode != usbRequest.FuncCode)
					return;
				//bytes.RemoveRange(0, 1);

				if (bytes.Count < 5)
					return;

				var message = WriteTrace("", bytes);
				Dispatcher.BeginInvoke(new Action(() =>
				{
					textBox1.Text += "\n" + message;
				}));
				bytes.RemoveRange(0, 1);
				//return;

				try
				{
					var bits = new BitArray(bytes.GetRange(0, 4).ToArray());

					string timeBits = "";
					foreach (var b in bits)
					{
						var intValue = (bool)b ? 1 : 0;
						timeBits += intValue.ToString();
					}
					Dispatcher.BeginInvoke(new Action(() =>
					{
						textBox1.Text += "\n timeBits = " + timeBits;
					}));

					var day = BitArrayToInt(bits, 0, 4);
					var month = BitArrayToInt(bits, 5, 8);
					var year = BitArrayToInt(bits, 9, 14);
					var hour = BitArrayToInt(bits, 15, 19);
					var min = BitArrayToInt(bits, 20, 25);
					var sec = BitArrayToInt(bits, 26, 31);

					string eventName = "";
					switch (bytes[4])
					{
						case 0x02:
							eventName = "Тест  ИП";
							break;

						case 0x09:
							eventName = "Потеря связи с устройством";
							break;

						case 0x23:
							eventName = "Неисправность устройства";
							break;

						case 0x25:
							eventName = "Предварительная запыленность извещателя";
							break;

						case 0x27:
							eventName = "Критическая запыленность извещателя";
							break;

						case 0x05:
							eventName = "Запущено ИУ";
							break;

						case 0x04:
							eventName = "Отмена автозапуска АСПТ(МУКО и/или СПТ) в зоне";
							break;

						case 0x34:
							eventName = "Обойдено устройство";
							break;

						case 0x07:
							eventName = "Неисправность системы";
							break;

						case 0x36:
							eventName = "Тест системы";
							break;

						case 0x0F:
							eventName = "Проблема со шлейфом";
							break;

						case 0x31:
							eventName = "Зона на охране";
							break;

						case 0x0C:
							eventName = "Вскрытие/корпус закрыт";
							break;

						case 0x01:
							eventName = "Пожар/пожар от обойденного ИП";
							break;

						case 0x3A:
							eventName = "Сообщение от технологических меток";
							break;

						default:
							eventName = "Неизвестно " + bytes[4].ToString("x2");
							break;
					}

					Dispatcher.BeginInvoke(new Action(() =>
					{
						textBox1.Text += "\n" + day.ToString() + "/" + month.ToString() + "/" + (year + 2000).ToString() + " " + hour.ToString() + ":" + min.ToString() + ":" + sec.ToString() +
							" " + eventName;
					}));
				}
				catch { }
			}
		}

		private void Window_Closed(object sender, EventArgs e)
		{
			usbRunner.Close();
		}

		int UsbRequestNo = 1;

		private void Button_Click(object sender, RoutedEventArgs e)
		{
			for (int i = 1; i < 2; i++)
			{
				lock (locker)
				{
					GetJournalItem(i);
				}
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
			bytes.Add(0x01);
			bytes.AddRange(BitConverter.GetBytes(journalNo).Reverse());

			var usbRequest = new UsbRequest()
			{
				Id = UsbRequestNo,
				UsbAddress = 0x04,
				SelfAddress = 0x01,
				FuncCode = 0x01
			};
			UsbRequests.Add(usbRequest);

			usbRunner.Send(bytes);
			Thread.Sleep(1);
		}

		string WriteTrace(string name, IEnumerable<byte> bytes)
		{
			var result = "";
			Trace.WriteLine("");
			Trace.WriteLine(name + ": ");
			foreach (var b in bytes)
			{
				var hexByte = b.ToString("x2");
				Trace.Write(hexByte + " ");
				result += hexByte + " ";
			}
			return result;
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