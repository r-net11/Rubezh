using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FiresecAPI;
using System.Net;
using System.Threading;
using System.Net.Sockets;
using System.Diagnostics;

namespace SKDDriver
{
	public static class SKDDeviceProcessor
	{
		[DebuggerStepThrough]
		public static SendResult SendBytes(SKDDevice device, List<byte> bytes, bool hasAnswer = true, int receiveTimeout = 2000)
		{
			var stringIPAddress = device.Address;
			if(string.IsNullOrEmpty(stringIPAddress))
				return new SendResult("Не задан IP адрес");
			int port = 0;
			var portProperty = device.Properties.FirstOrDefault(x => x.Name == "Port");
			if (portProperty != null)
			{
				port = portProperty.Value;
			}
			else
			{
				return new SendResult("Не задан порт");
			}


			IPAddress ipAddress;
			var result = IPAddress.TryParse(stringIPAddress, out ipAddress);
			if (!result)
			{
				return new SendResult("Неверный формат IP адреса");
			}
			var endPoint = new IPEndPoint(ipAddress, port);
			var udpClient = new UdpClient();
			udpClient.Client.ReceiveTimeout = receiveTimeout;
			udpClient.Client.SendTimeout = 1000;

			try
			{
				var bytesSent = udpClient.Send(bytes.ToArray(), bytes.Count, endPoint);
				if (bytesSent != bytes.Count)
				{
					return new SendResult("Не все данные удалось отправить");
				}
			}
			catch
			{
				OnConnectionLost();
				udpClient.Close();
				return new SendResult("Ошибка открытия сокета");
			}
			if (hasAnswer == false)
			{
				udpClient.Close();
				return new SendResult(new List<byte>());
			}
			var recievedBytes = new List<byte>();
			try
			{
				recievedBytes = udpClient.Receive(ref endPoint).ToList();
			}
			catch (SocketException)
			{
				OnConnectionLost();
				udpClient.Close();
				return new SendResult("От устройства не получен ответ в заданный таймаут");
			}
			udpClient.Close();

			return new SendResult(recievedBytes.Skip(1).ToList());
		}

		public static List<byte> ToBytes(ushort shortValue)
		{
			return BitConverter.GetBytes(shortValue).ToList();
		}

		public static event Action ConnectionLost;
		static void OnConnectionLost()
		{
			if (ConnectionLost != null)
				ConnectionLost();
		}

		static bool CheckIpAddress(string ipAddress)
		{
			if (String.IsNullOrEmpty(ipAddress))
				return false;
			IPAddress address;
			return IPAddress.TryParse(ipAddress, out address);
		}
	}
}