using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Windows;
using ServerFS2;

namespace USBTest
{
	public partial class MainWindow : Window
	{
		public MainWindow()
		{
			InitializeComponent();
		}

		UsbLibrary.UsbHidPort UsbDevice;

		private void Button_Click_1(object sender, RoutedEventArgs e)
		{
			UsbDevice = new UsbLibrary.UsbHidPort();
			UsbDevice.VendorId = 0xC251;
			UsbDevice.ProductId = 0x1303;
			UsbDevice.CheckDevicePresent();

			UsbDevice.SpecifiedDevice.DataRecieved += new UsbLibrary.DataRecievedEventHandler(SpecifiedDevice_DataRecieved);
		}

		void SpecifiedDevice_DataRecieved(object sender, UsbLibrary.DataRecievedEventArgs args)
		{
			if (args.data.Contains((byte)0x7e))
			{
				;
			}
			var bytesString = BytesHelper.BytesToString(args.data.ToList());
			Trace.WriteLine("SpecifiedDevice_DataRecieved " + bytesString);
		}

		private void Button_Click_2(object sender, RoutedEventArgs e)
		{
			for (int i = 0; i < 1000; i++)
			{
				Thread.Sleep(10);
				var bytes = new List<byte>() { 0x00, 0x7e, 0x00, 0x00, 0x00, 0x01, 0x03, 0x02, 0x01, 0x57, 0x3e };
				while (bytes.Count % 65 > 0)
				{
					bytes.Add(0);
				}
				UsbDevice.SpecifiedDevice.SendData(bytes.ToArray());
			}
		}

		UsbProcessor UsbProcessor;

		private void Button_Click_3(object sender, RoutedEventArgs e)
		{
			UsbProcessor = new UsbProcessor();
			UsbProcessor.Open();
			UsbProcessor.NewResponse += new Action<ServerFS2.Response>(UsbRunner2_NewResponse);
		}

		void UsbRunner2_NewResponse(Response response)
		{
			Trace.WriteLine("UsbRunner2_NewResponse " + BytesHelper.BytesToString(response.Bytes));
		}

		private void Button_Click_4(object sender, RoutedEventArgs e)
		{
			for (int i = 0; i < 1000; i++)
			{
				Thread.Sleep(10);
				//var bytes = new List<byte>() { 0x7e, 0x00, 0x00, 0x00, 0x01, 0x03, 0x02, 0x01, 0x57, 0x3e };
				//var bytes = new List<byte>() { 0x03, 0x02, 0x01, 0x57 };
				var bytes = new List<byte>() { 0x03, 0x02, 0x01, 0x21, 0x00 };
				var bytesList = new List<List<byte>>();
				bytesList.Add(bytes);
				UsbProcessor.AddRequest(i, bytesList, 1000, 1000, true);
			}
		}
	}
}