﻿using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Xml.Serialization;

namespace AssadProcessor
{
	public class NetManager
	{
		const int localPort = 2002;
		const int remotePort = 2002;
		string udpBroadcastMessage = "<processor><host port=\"2002\" /><state idle=\"true\" /></processor>";
		MessageProcessor messageProcessor;
		byte[] recievedBytes = new byte[102400];
		int messageNo = 0;

		Socktes.ListenSocket Socket_Listen;
		Socktes.ConnectSocket Socket_Connection;

		static NetManager tcpServer;
		public static void Send(object confirmation, string refMessageId)
		{
			tcpServer.send(confirmation, refMessageId);
		}

		public NetManager()
		{
			tcpServer = this;
			messageProcessor = new MessageProcessor();
		}

		public void Start()
		{
			Socket_Listen = new Socktes.ListenSocket();
			Socket_Listen.Port = localPort;
			Socket_Listen.accept += new Socktes.AcceptEvenetHandler(this.Socket_Listen_accept);

			Socket_Connection = new Socktes.ConnectSocket();
			Socket_Connection.IsBlocked = false;
			Socket_Connection.recieve += new Socktes.RecieveEventHandler(this.Socket_Connection_recieve);

			Socket_Listen.StratListen(false);

			SendBroadcastUdp();
		}

		void Socket_Listen_accept(object Sender, Socktes.AcceptEventArgs e)
		{
			Socket_Connection.SocketHandle = e.ConnectedSocket;
		}

		string partMessage = "";

		object recieveLocker = new object();

		void Socket_Connection_recieve(object Sender, Socktes.RecieveEventArgs e)
		{
			lock (recieveLocker)
			{
				string message = Encoding.UTF8.GetString(e.Data); //.Trim();

				int endIndex = message.IndexOf("\0");
				if (endIndex > 0)
					message = message.Remove(message.IndexOf("\0"));

				int iteration = 0;

				while (string.IsNullOrEmpty(message) == false)
				{
					iteration++;

					int firstIndexOf = message.IndexOf("<?xml");

					if (firstIndexOf == -1)
					{
						partMessage += message;
						if (partMessage.EndsWith("</message>"))
						{
							messageProcessor.ProcessMessage(partMessage);
							partMessage = "";
						}
						break;
					}
					else
					{
						partMessage += message.Substring(0, firstIndexOf);
						if (partMessage.EndsWith("</message>"))
						{
							messageProcessor.ProcessMessage(partMessage);
							partMessage = "";
						}
						message = message.Remove(0, firstIndexOf);

						int lastIndexOf = message.IndexOf("</message>");
						if (lastIndexOf == -1)
						{
							partMessage += message;
							break;
						}
						else
						{
							string fullMessage = message.Substring(0, lastIndexOf + "</message>".Length);
							messageProcessor.ProcessMessage(fullMessage);
							message = message.Remove(0, lastIndexOf + "</message>".Length);
						}
					}
				}
			}
		}

		object locker = new object();

		void send(object confirmation, string refMessageId)
		{
			lock (locker)
			{
				messageNo++;

				var messageType = new Assad.MessageType();
				messageType.msgTime = DateTime.Now.ToString("dd-MM-yyyy hh:mm:ss");
				messageType.msgId = "messageNo." + messageNo.ToString();
				if (!String.IsNullOrEmpty(refMessageId))
					messageType.refMsg = refMessageId;
				messageType.Item = confirmation;

				var serializer = new XmlSerializer(typeof(Assad.MessageType));
				var memoryStream = new MemoryStream();
				serializer.Serialize(memoryStream, messageType);
				byte[] bytes = memoryStream.ToArray();
				memoryStream.Close();

				string message = Encoding.UTF8.GetString(bytes);
				message = RemoveWhitespaces(message);
				bytes = Encoding.UTF8.GetBytes(message);

				Socket_Connection.Send(bytes);
			}
		}

		string RemoveWhitespaces(string message)
		{
			message = message.Replace("xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\" ", "");
			message = message.Replace("\r\n", "");

			while (true)
			{
				if (message.Contains("  ") == false)
					break;
				message = message.Replace("  ", "");
			}
			while (true)
			{
				if (message.Contains("> <") == false)
					break;
				message = message.Replace("> <", "><");
			}

			return message;
		}

		public void Stop()
		{
			try
			{
				Socket_Connection.ShutDown(SocketShutdown.Both);
				Socket_Listen.StopListen();
			}
			catch
			{
			}
		}

		void SendBroadcastUdp()
		{
			var udpClient = new UdpClient()
			{
				EnableBroadcast = true
			};
			var endPoint = new IPEndPoint(IPAddress.Parse("255.255.255.255"), remotePort);
			byte[] sendBytes = Encoding.UTF8.GetBytes(udpBroadcastMessage);
			udpClient.Send(sendBytes, sendBytes.Length, endPoint);
			udpClient.Close();
		}
	}
}