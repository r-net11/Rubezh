using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Sockets;
using System.Net;
using XFiresecAPI;
using System.Diagnostics;
using GKImitator.ViewModels;
using GKProcessor;

namespace GKImitator.Processor
{
	public class SKDImitatorProcessor
	{
		int Port;
		Socket serverSocket;
		byte[] byteData = new byte[64];

		public SKDImitatorProcessor(int port)
		{
			Port = port;
		}

		public void Start()
		{
			serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
			IPEndPoint ipEndPoint = new IPEndPoint(IPAddress.Any, Port);
			serverSocket.Bind(ipEndPoint);

			IPEndPoint ipeSender = new IPEndPoint(IPAddress.Any, 0);
			EndPoint epSender = (EndPoint)ipeSender;

			serverSocket.BeginReceiveFrom(byteData, 0, byteData.Length, SocketFlags.None, ref epSender, new AsyncCallback(OnReceive), epSender);
		}

		void OnReceive(IAsyncResult ar)
		{
			IPEndPoint ipeSender = new IPEndPoint(IPAddress.Any, 0);
			EndPoint epSender = (EndPoint)ipeSender;
			serverSocket.EndReceiveFrom(ar, ref epSender);

			var bytes = CreateAnswer();
			if (bytes != null)
			{
				var sendBytes = new List<byte>();
				sendBytes.Add(byteData[0]);
				sendBytes.AddRange(bytes);
				byte[] message = sendBytes.ToArray();

				serverSocket.BeginSendTo(message, 0, message.Length, SocketFlags.None, epSender, new AsyncCallback(OnSend), epSender);
			}
			serverSocket.BeginReceiveFrom(byteData, 0, byteData.Length, SocketFlags.None, ref epSender, new AsyncCallback(OnReceive), epSender);
		}

		void OnSend(IAsyncResult ar)
		{
			serverSocket.EndSend(ar);
		}

		List<byte> CreateAnswer()
		{
			var result = new List<byte>();
			switch (byteData[0])
			{
				case 1:
					result.Add(9);
					return result;
				case 2:
					return result;
			}
			return new List<byte>();
		}

		public static List<byte> ToBytes(short shortValue)
		{
			return BitConverter.GetBytes(shortValue).ToList();
		}

		public static List<byte> IntToBytes(int intValue)
		{
			return BitConverter.GetBytes(intValue).ToList();
		}
	}
}