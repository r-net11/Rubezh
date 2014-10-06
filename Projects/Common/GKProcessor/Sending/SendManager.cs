using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using FiresecAPI.GK;
using FiresecClient;

namespace GKProcessor
{
	[DebuggerStepThrough]
	public static class SendManager
	{
		static object locker = new object();

		public static SendResult Send(GKDevice device, ushort length, byte command, ushort inputLenght, List<byte> data = null, bool hasAnswer = true, bool sleepInsteadOfRecieve = false, int receiveTimeout = 2000)
		{
			lock (locker)
			{
				if ((device == null) || (device.Driver == null))
				{
					return new SendResult("Неизвестное устройство");
				}

				byte whom = 0;

				switch (device.DriverType)
				{
					case GKDriverType.GK:
						whom = 2;
						break;

					case GKDriverType.KAU:
					case GKDriverType.RSR2_KAU:
						whom = 4;
						var modeProperty = device.Properties.FirstOrDefault(x => x.Name == "Mode");
						if (modeProperty != null)
						{
							switch (modeProperty.Value)
							{
								case 0:
									whom = 4;
									break;

								case 1:
									whom = 5;
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
				bytes.Add((byte)device.IntAddress);
				bytes.AddRange(ToBytes(length));
				bytes.Add(command);
				if (data != null)
					bytes.AddRange(data);

				string ipAddress = GKManager.GetIpAddress(device);
				if (string.IsNullOrEmpty(ipAddress))
				{
					return new SendResult("Не задан адрес ГК");
				}
				var resultBytes = SendBytes(ipAddress, bytes, inputLenght, hasAnswer, sleepInsteadOfRecieve, receiveTimeout);
				return resultBytes;
			}
		}

		static SendResult SendBytes(string stringIPAddress, List<byte> bytes, ushort inputLenght, bool hasAnswer = true, bool sleepInsteadOfRecieve = false, int receiveTimeout = 2000)
		{
			IPAddress ipAddress;
			var result = IPAddress.TryParse(stringIPAddress, out ipAddress);
			if (!result)
			{
				return new SendResult("Неверный формат IP адреса");
			}
			var endPoint = new IPEndPoint(ipAddress, 1025);
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
				if (sleepInsteadOfRecieve)
				{
					Thread.Sleep(10000);
					return new SendResult(new List<byte>());
				}
				recievedBytes = udpClient.Receive(ref endPoint).ToList();
			}
			catch (SocketException)
			{
				OnConnectionLost();
				udpClient.Close();
				return new SendResult("Устройство недоступно или IP-адрес компьютера отсутствует в списке разрешенных адресов прибора");
			}
			udpClient.Close();

			if (recievedBytes[0] != bytes[0])
			{
				return new SendResult("Не совпадает байт 'Кому'");
			}
			if (recievedBytes[1] != bytes[1])
			{
				return new SendResult("Не совпадает байт 'Адрес'");
			}
			if (recievedBytes[4] != bytes[4])
			{
				return new SendResult("Не совпадает байт 'Команда'");
			}

			var recievedInputLenght = (ushort)(recievedBytes[2] + 256 * recievedBytes[3]);
			if (inputLenght != ushort.MaxValue)
			{
				if (inputLenght != recievedInputLenght)
				{
					//return new SendResult("Не совпадает байт 'Длина'");
				}
			}
			return new SendResult(recievedBytes.Skip(5).ToList());
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