using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Sockets;
using System.Net;

namespace GKModule
{
	public static class CommandManager
	{
		public static void Send()
		{
			List<byte> bytes = new List<byte>();
			byte whom = 2;
			byte address = 0;
			byte length = 0;
			byte command = 0;
			List<byte> data = new List<byte>();

			bytes.Add(whom);
			bytes.Add(address);
			bytes.Add(length);
			bytes.Add(command);
			bytes.AddRange(data);

			SendBytes(bytes.ToArray());
		}

		static void SendBytes(byte[] bytes)
		{
			var udpClient = new UdpClient()
			{
				EnableBroadcast = false
			};
			udpClient.Connect(IPAddress.Parse("126.1.1.2"), 80);
			var endPoint = new IPEndPoint(IPAddress.Parse("126.1.1.2"), 80);
			//udpClient.Send(bytes, bytes.Length, endPoint);
			udpClient.Send(bytes, bytes.Length);

			IPEndPoint RemoteIpEndPoint = new IPEndPoint(IPAddress.Any, 0);
			byte[] resievedBytes = udpClient.Receive(ref RemoteIpEndPoint);

			udpClient.Close();
		}
	}
}