using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;

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
			byte command = 1;
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
			udpClient.Connect(IPAddress.Parse("172.16.7.103"), 1025);
			var endPoint = new IPEndPoint(IPAddress.Parse("172.16.7.103"), 1025);
			//udpClient.Send(bytes, bytes.Length, endPoint);
			var result = udpClient.Send(bytes, bytes.Length);

			//IPEndPoint RemoteIpEndPoint = new IPEndPoint(IPAddress.Any, 0);
			byte[] resievedBytes = udpClient.Receive(ref endPoint);

			udpClient.Close();
		}
	}
}