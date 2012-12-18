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

namespace TestUSB
{
	public partial class MainWindow : Window
	{
		UsbRunner usbRunner;

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

		void usbRunner_DataRecieved(List<byte> bytes)
		{
			System.Collections.BitArray bits = new System.Collections.BitArray(bytes.GetRange(9, 4).ToArray());
			var day = BitArrayToInt(bits, 0, 4);
			var month = BitArrayToInt(bits, 5, 8);
			var year = BitArrayToInt(bits, 9, 14);
			var hour = BitArrayToInt(bits, 15, 19);
			var min = BitArrayToInt(bits, 20, 25);
			var sec = BitArrayToInt(bits, 26, 31);

			string eventName = "";
			switch(bytes[14])
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
			}

			Dispatcher.BeginInvoke(new Action(() =>
			{
				textBox1.Text += "\n" + WriteTrace("", bytes);
				textBox1.Text += "\n" + day.ToString() + "/" + month.ToString() + "/" + (year + 2000).ToString() + " " + hour.ToString() + ":" + min.ToString() + ":" + sec.ToString() +
					" " + eventName;
			}));
		}

		private void Window_Closed(object sender, EventArgs e)
		{
			usbRunner.Close();
		}

		int no = 1;

		private void Button_Click(object sender, RoutedEventArgs e)
		{
			var bytes = new List<byte>();

			bytes.Add(0x44);
			bytes.Add(0x44);
			bytes.Add(0x44);
			bytes.Add((byte)no);

			bytes.Add(0x04);
			bytes.Add(0x01);
			bytes.Add(0x01);
			bytes.Add(0x20);
			bytes.Add(0x01);
			bytes.Add(0x00);
			bytes.Add(0x00);
			bytes.Add(0x00);
			bytes.Add((byte)no);

			usbRunner.Send(bytes);

			no++;
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
}