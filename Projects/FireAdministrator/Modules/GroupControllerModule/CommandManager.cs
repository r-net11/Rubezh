using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System;
using System.Linq;
using XFiresecAPI;
using System.Diagnostics;

namespace GKModule
{
	public static class CommandManager
	{
		public static List<byte> Send(XDevice device, short length, byte command, short inputLenght, List<byte> data = null, bool hasAnswer = true)
		{
			byte whom = 0;
			byte address = 0;

			switch(device.Driver.DriverType)
			{
				case XDriverType.GK:
					whom = 2;
					address = device.IntAddress;
					break;

				case XDriverType.KAU:
					whom = 4;
					address = device.IntAddress;
					var modeProperty = device.Properties.FirstOrDefault(x=>x.Name == "");
					if (modeProperty != null)
					{
						switch(modeProperty.Value)
						{
							case 0:
								break;

							case 1:
								address += 127;
								throw new Exception("Неизвестный тип линии");

							default:
								break;
						}
					}
					break;

				default:
					throw new Exception("Команду пожно отправлять только в ГК или в КАУ");
			}
			var bytes = new List<byte>();
			bytes.Add(whom);
			bytes.Add(address);
			bytes.AddRange(ToBytes(length));
			bytes.Add(command);
			if (data != null)
				bytes.AddRange(data);

			if (command == 17 && data.Count != 5)
			{
				var bytesCount = bytes.Count;
				for (int i = 0; i < 256 - bytesCount; i++)
				{
					bytes.Add(0);
				}
			}

			Trace.WriteLine(BytesHelper.BytesToString(bytes));
			return SendBytes(bytes.ToArray(), inputLenght, hasAnswer);
		}

		static List<byte> SendBytes(byte[] bytes, short inputLenght, bool hasAnswer = true)
		{
			var udpClient = new UdpClient()
			{
				EnableBroadcast = false
			};
			udpClient.Connect(IPAddress.Parse("172.16.7.102"), 1025);
			var endPoint = new IPEndPoint(IPAddress.Parse("172.16.7.102"), 1025);
			var bytesSent = udpClient.Send(bytes, bytes.Length);
			if (hasAnswer == false)
				return null;
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