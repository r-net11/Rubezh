using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using Infrastructure.Common.Windows;
using XFiresecAPI;
using FiresecClient;
using Common.GK;

namespace Commom.GK
{
	public static class SendManager
	{
		public static List<byte> Send(XDevice device, short length, byte command, short inputLenght, List<byte> data = null, bool hasAnswer = true)
		{
			byte whom = 0;
			byte address = 0;

			if ((device == null) || (device.Driver == null))
			{
				return new List<byte>();
			}

			switch(device.Driver.DriverType)
			{
				case XDriverType.GK:
					whom = 2;
					address = device.IntAddress;
					break;

				case XDriverType.KAU:
					whom = 4;
					address = device.IntAddress;
					var modeProperty = device.Properties.FirstOrDefault(x => x.Name == "Mode");
					if (modeProperty != null)
					{
						switch(modeProperty.Value)
						{
							case 0:
								break;

							case 1:
								address += 127;
								break;

							default:
								throw new Exception("Неизвестный тип линии");
						}
					}
					break;

				default:
					throw new Exception("Команду можно отправлять только в ГК или в КАУ");
			}
			var bytes = new List<byte>();
			bytes.Add(whom);
			bytes.Add(address);
			bytes.AddRange(ToBytes(length));
			bytes.Add(command);
			if (data != null)
				bytes.AddRange(data);

			string ipAddress = XManager.GetIpAddress(device);
			var resultBytes = SendBytes(ipAddress, bytes, inputLenght, hasAnswer);
			return resultBytes;
		}

		static List<byte> SendBytes(string ipAddress, List<byte> bytes, short inputLenght, bool hasAnswer = true)
		{
			var udpClient = new UdpClient();
			udpClient.Client.ReceiveTimeout = 1000;
			udpClient.Client.SendTimeout = 1000;
			udpClient.Connect(IPAddress.Parse(ipAddress), 1025);
			var endPoint = new IPEndPoint(IPAddress.Parse(ipAddress), 1025);
			var bytesSent = udpClient.Send(bytes.ToArray(), bytes.Count);
			//Trace.WriteLine("<-- " + BytesHelper.BytesToString(bytes));
			if (hasAnswer == false)
				return null;
			var recievedBytes = new List<byte>();
			try
			{
				recievedBytes = udpClient.Receive(ref endPoint).ToList();
			}
			catch (SocketException)
			{
				OnConnectionLost();
				return null;
				//throw new ProtocolException();
				//MessageBoxService.Show("Устройство не отвечает");
				//return new List<byte>();
			}
			//Trace.WriteLine("--> " + BytesHelper.BytesToString(recievedBytes));
			udpClient.Close();

			if (recievedBytes[0] != bytes[0])
			{
				MessageBoxService.Show("Не совпадает байт 'Кому'");
			}
			if (recievedBytes[1] != bytes[1])
			{
				MessageBoxService.Show("Не совпадает байт 'Адрес'");
			}
			if (recievedBytes[4] != bytes[4])
			{
				MessageBoxService.Show("Не совпадает байт 'Команда'");
			}

			var recievedInputLenght = (short)(recievedBytes[2] + 256 * recievedBytes[3]);
			if (inputLenght != -1)
			{
				if (inputLenght != recievedInputLenght)
				{
					//MessageBoxService.Show("Не совпадают байты 'Длина'");
				}
			}

			return recievedBytes.Skip(5).ToList();
		}

		public static List<byte> ToBytes(short shortValue)
		{
			return BitConverter.GetBytes(shortValue).ToList();
		}

		public static event Action ConnectionLost;
		static void OnConnectionLost()
		{
			if (ConnectionLost != null)
				ConnectionLost();
		}
	}
}