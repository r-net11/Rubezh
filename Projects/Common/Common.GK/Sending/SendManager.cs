using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using FiresecClient;
using XFiresecAPI;

namespace Common.GK
{
    public static class SendManager
    {
		static bool IsLogging = false;
		static StreamWriter StreamWriter;
		public static void StrartLog(string fileName)
		{
			IsLogging = true;
			StreamWriter = new StreamWriter(fileName);
		}
		public static void StopLog()
		{
			IsLogging = false;
			StreamWriter.Close();
		}

        public static SendResult Send(XDevice device, ushort length, byte command, ushort inputLenght, List<byte> data = null, bool hasAnswer = true)
        {
            byte whom = 0;
            byte address = 0;

            if ((device == null) || (device.Driver == null))
            {
                return new SendResult("Неизвестное устройство");
            }

            switch (device.Driver.DriverType)
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
                        switch (modeProperty.Value)
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

        static SendResult SendBytes(string ipAddress, List<byte> bytes, ushort inputLenght, bool hasAnswer = true)
        {
            if (!CheckIpAddress(ipAddress))
            {
                OnConnectionLost();
                return new SendResult("Ошибка разбора IP адреса");
            }

            IPEndPoint endPoint = new IPEndPoint(IPAddress.Parse(ipAddress), 1025);
            var udpClient = new UdpClient();
            udpClient.Client.ReceiveTimeout = 1000;
            udpClient.Client.SendTimeout = 1000;
            try
            {
                udpClient.Connect(IPAddress.Parse(ipAddress), 1025);
				if (IsLogging)
				{
					StreamWriter.WriteLine(BytesHelper.BytesToString(bytes));
				}
                var bytesSent = udpClient.Send(bytes.ToArray(), bytes.Count);
            }
            catch
            {
                OnConnectionLost();
                udpClient.Close();
                return new SendResult("Ошибка открытия сокета");
            }
            //Trace.WriteLine("<-- " + BytesHelper.BytesToString(bytes));
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
                return new SendResult("Коммуникационная ошибка");
            }
            //Trace.WriteLine("--> " + BytesHelper.BytesToString(recievedBytes));
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