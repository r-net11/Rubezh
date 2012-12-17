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

namespace TestUSB
{
	public partial class MainWindow : Window
	{
		public MainWindow()
		{
			InitializeComponent();
		}

		private void Window_Loaded(object sender, RoutedEventArgs e)
		{
			usbRunner = new UsbRunner();
			usbRunner.Open();
		}

		private void Window_Closed(object sender, EventArgs e)
		{
			usbRunner.Close();
		}

		UsbRunner usbRunner;

		private void Button_Click(object sender, RoutedEventArgs e)
		{
			var bytes = new List<byte>(0);
			bytes.Add(0x02);
			bytes.Add(0x01);
			bytes.Add(0x01);
			bytes.Add(0x01);

			usbRunner.Send(bytes);
		}
	}
}