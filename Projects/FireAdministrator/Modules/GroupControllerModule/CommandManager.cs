using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System;
using System.Linq;

namespace GKModule
{
	public static class CommandManager
	{
		public static List<byte> Send(short length, byte command, short inputLenght, List<byte> data = null)
		{
			List<byte> bytes = new List<byte>();
			byte whom = 2;
			byte address = 1;
			if (data == null)
				data = new List<byte>();

			bytes.Add(whom);
			bytes.Add(address);
			bytes.AddRange(ToBytes(length));
			bytes.Add(command);
			bytes.AddRange(data);

			return SendBytes(bytes.ToArray(), inputLenght);
		}

		static List<byte> SendBytes(byte[] bytes, short inputLenght)
		{
			var udpClient = new UdpClient()
			{
				EnableBroadcast = false
			};
			udpClient.Connect(IPAddress.Parse("172.16.7.102"), 1025);
			var endPoint = new IPEndPoint(IPAddress.Parse("172.16.7.102"), 1025);
			var bytesSent = udpClient.Send(bytes, bytes.Length);
			byte[] recievedBytes = udpClient.Receive(ref endPoint);
			udpClient.Close();

			if (recievedBytes[0] != bytes[0])
				throw new Exception("Не совпадает байт 0");
			if (recievedBytes[1] != bytes[1])
				throw new Exception("Не совпадает байт 1");
			if (recievedBytes[4] != bytes[4])
				throw new Exception("Не совпадает байт 4");

			var recievedInputLenght = (short)(recievedBytes[2] + 256 * recievedBytes[3]);
			if (inputLenght != recievedInputLenght)
				throw new Exception("Не совпадает байт 2, 3");

			return recievedBytes.Skip(5).ToList();
		}

		public static List<byte> ToBytes(short shortValue)
		{
			return BitConverter.GetBytes(shortValue).ToList();
		}
	}
}