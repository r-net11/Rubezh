using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO.Ports;
using System.Diagnostics;

namespace ServerFS2.USB
{
	public static class ComPortHelper
	{
		public static void Run()
		{
			var serialPort = new SerialPort();
			serialPort.PortName = "Test Name";
			serialPort.BaudRate = 115200;
			serialPort.Parity = Parity.None;
			serialPort.DataBits = 1;
			serialPort.StopBits = StopBits.None;
			serialPort.Handshake = Handshake.None;
			serialPort.ReadTimeout = 500;
			serialPort.WriteTimeout = 500;

			serialPort.ReadBufferSize = 64;
			serialPort.WriteBufferSize = 64;

			serialPort.DataReceived += new SerialDataReceivedEventHandler(serialPort_DataReceived);

			serialPort.Open();
			var bytes = new List<byte>();
			serialPort.Write(bytes.ToArray(), 0, bytes.Count);
			serialPort.Read(bytes.ToArray(), 0, bytes.Count);
			serialPort.Close();
		}

		static void serialPort_DataReceived(object sender, SerialDataReceivedEventArgs e)
		{
			Trace.WriteLine("serialPort_DataReceived");
		}
	}
}