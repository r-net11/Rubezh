using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LibUsbDotNet;
using LibUsbDotNet.Main;
using LibUsbDotNet.Info;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Threading;

namespace TestUSB
{
	public class UsbRunner
	{
		UsbDevice UsbDevice;
		UsbEndpointReader reader;
		UsbEndpointWriter writer;

		public void Open()
		{
			UsbDeviceFinder UsbFinder = new UsbDeviceFinder(0xC251, 0x1303);

			UsbDevice = UsbDevice.OpenUsbDevice(UsbFinder);
			if (UsbDevice == null)
				throw new Exception("Device Not Found.");
			IUsbDevice wholeUsbDevice = UsbDevice as IUsbDevice;
			if (!ReferenceEquals(wholeUsbDevice, null))
			{
				wholeUsbDevice.SetConfiguration(1);
				wholeUsbDevice.ClaimInterface(0);
			}

			reader = UsbDevice.OpenEndpointReader(ReadEndpointID.Ep01);
			reader.DataReceived += (OnDataRecieved);
			reader.ReadBufferSize = 64;
			reader.DataReceivedEnabled = true;

			writer = UsbDevice.OpenEndpointWriter(WriteEndpointID.Ep01);
		}

		public void Close()
		{
			reader.DataReceivedEnabled = false;
			reader.DataReceived -= (OnDataRecieved);

			if (UsbDevice != null)
			{
				if (UsbDevice.IsOpen)
				{
					IUsbDevice wholeUsbDevice = UsbDevice as IUsbDevice;
					if (!ReferenceEquals(wholeUsbDevice, null))
					{
						wholeUsbDevice.ReleaseInterface(0);
					}
					UsbDevice.Close();
				}
				UsbDevice = null;
				UsbDevice.Exit();
			}
		}

		public void Send(List<byte> data)
		{
			var output = CreateOutputBytes(data).ToArray();
			int bytesWrite;
			//WriteTrace("Sending", output);
			var errorCode = writer.Write(output, 2000, out bytesWrite);
		}

		void OnDataRecieved(object sender, EndpointDataEventArgs e)
		{
			//WriteTrace("OnDataRecieved", e.Buffer);

			var result = new List<byte>();
			foreach (var b in e.Buffer)
			{
				if (result.Count > 0)
				{
					result.Add(b);

					if (b == 0x3E)
					{
						if (DataRecieved != null)
							DataRecieved(result);
						result = new List<byte>();
					}
				}
				if (b == 0x7E)
				{
					result = new List<byte>();
					result.Add(b);
				}
			}
		}

		List<byte> CreateOutputBytes(List<byte> messageBytes)
		{
			var bytes = new List<byte>(0);
			bytes.Add(0x7e);
			foreach (var b in messageBytes)
			{
				if (b == 0x7E)
				{
					bytes.Add(0x7D);
					bytes.Add(0x5E);
					continue;
				}
				if (b == 0x7D)
				{
					bytes.Add(0x7D);
					bytes.Add(0x5D);
					continue;
				}
				if (b == 0x3E)
				{
					bytes.Add(0x3D);
					bytes.Add(0x1E);
					continue;
				}
				if (b == 0x3D)
				{
					bytes.Add(0x3D);
					bytes.Add(0x1D);
					continue;
				}
				bytes.Add(b);
			}
			bytes.Add(0x3e);
			var bytesCount = bytes.Count;

			if (bytesCount < 64)
			{
				for (int i = 0; i < 64 - bytesCount; i++)
				{
					bytes.Add(0);
				}
			}

			return bytes;
		}

		object locker = new object();

		string WriteTrace(string name, IEnumerable<byte> bytes)
		{
			lock (locker)
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

		public event Action<List<byte>> DataRecieved;
	}
}