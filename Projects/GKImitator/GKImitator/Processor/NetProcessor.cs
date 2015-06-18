﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using FiresecAPI.GK;
using GKImitator.ViewModels;
using GKProcessor;
using FiresecClient;

namespace GKImitator.Processor
{
	public class NetProcessor
	{
		GkDatabase GkDatabase;
		Socket serverSocket;
		byte[] byteData = new byte[64];

		public void Start()
		{
			GkDatabase = DescriptorsManager.GkDatabases.FirstOrDefault();
			StartSocket();
		}

		void StartSocket()
		{
			serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
			IPEndPoint ipEndPoint = new IPEndPoint(IPAddress.Any, 1025);
			serverSocket.Bind(ipEndPoint);

			IPEndPoint ipeSender = new IPEndPoint(IPAddress.Any, 0);
			EndPoint epSender = (EndPoint)ipeSender;

			serverSocket.BeginReceiveFrom(byteData, 0, byteData.Length, SocketFlags.None, ref epSender, new AsyncCallback(OnReceive), epSender);
		}

		void OnReceive(IAsyncResult ar)
		{
			try
			{
				IPEndPoint ipeSender = new IPEndPoint(IPAddress.Any, 0);
				EndPoint epSender = (EndPoint)ipeSender;
				serverSocket.EndReceiveFrom(ar, ref epSender);

				var bytes = CreateAnswer();
				if (bytes != null)
				{
					var sendBytes = new List<byte>();
					sendBytes.Add(byteData[0]);
					sendBytes.Add(byteData[1]);
					sendBytes.AddRange(ToBytes((short)bytes.Count));
					sendBytes.Add(byteData[4]);
					sendBytes.AddRange(bytes);
					byte[] message = sendBytes.ToArray();

					serverSocket.BeginSendTo(message, 0, message.Length, SocketFlags.None, epSender, new AsyncCallback(OnSend), epSender);
				}
				serverSocket.BeginReceiveFrom(byteData, 0, byteData.Length, SocketFlags.None, ref epSender, new AsyncCallback(OnReceive), epSender);
			}
			catch
			{
				serverSocket.Close();
				StartSocket();
			}
		}

		void OnSend(IAsyncResult ar)
		{
			serverSocket.EndSend(ar);
		}

		List<byte> CreateAnswer()
		{
			switch (byteData[4])
			{
				case 1:
					return new List<byte>() { 44 };

				case 2:
					int softwareNo = 33;
					int hardwareNo = 55;
					var result = new List<byte>();
					result.AddRange(BitConverter.GetBytes(softwareNo));
					result.AddRange(BitConverter.GetBytes(hardwareNo));
					return result;

				case 5: // DateTime Synchrinysation
					var descriptorNo = BytesHelper.SubstructInt(byteData.ToList(), 5);
					var descriptorViewModel = MainViewModel.Current.Descriptors.FirstOrDefault(x => x.DescriptorNo == descriptorNo);
					if (descriptorViewModel != null)
					{
						descriptorViewModel.SynchronyzeDateTime();
					}
					return new List<byte>();

				case 6:
					var count = DBHelper.ImitatorJournalItemCollection.ImitatorJournalItems.Count;
					if (count > 0)
						return GetJournalBytes(count);
					return null;

				case 7:
					descriptorNo = BytesHelper.SubstructInt(byteData.ToList(), 5);
					return GetJournalBytes(descriptorNo);

				case 9: // Чтение параметра
					descriptorNo = BytesHelper.SubstructInt(byteData.ToList(), 5);
					descriptorViewModel = MainViewModel.Current.Descriptors.FirstOrDefault(x => x.DescriptorNo == descriptorNo);
					if (descriptorViewModel != null)
					{
						descriptorViewModel.GetParameters();
					}
					return null;

				case 10: // Установка параметра
					descriptorNo = BytesHelper.SubstructShort(byteData.ToList(), 5);
					descriptorViewModel = MainViewModel.Current.Descriptors.FirstOrDefault(x => x.DescriptorNo == descriptorNo);
					if (descriptorViewModel != null)
					{
						descriptorViewModel.SetParameters();
					}
					return null;

				case 12: // Запрос состояния
					descriptorNo = BytesHelper.SubstructShort(byteData.ToList(), 5);
					descriptorViewModel = MainViewModel.Current.Descriptors.FirstOrDefault(x => x.DescriptorNo == descriptorNo);
					if (descriptorViewModel != null)
					{
						return descriptorViewModel.GetStateBytes(descriptorNo);
					}
					return null;

				case 13: // Команда управления
					descriptorNo = BytesHelper.SubstructShort(byteData.ToList(), 5);
					var commandCode = byteData[7] - 0x80;
					var stateBit = (GKStateBit)commandCode;
					descriptorViewModel = MainViewModel.Current.Descriptors.FirstOrDefault(x => x.DescriptorNo == descriptorNo);
					if (descriptorViewModel != null)
					{
						descriptorViewModel.ClientCommand(stateBit);
					}
					return new List<byte>();

				case 23: // Infoormational Block
					var blockNo = byteData[5];

					byte minorVersion = 1;
					byte majorVersion = 1;
					var hash1 = HashHelper.CreateHash1(GKManager.DeviceConfiguration, GKManager.DeviceConfiguration.RootDevice.Children[0]);
					var infoBlock = new List<byte>(256) { minorVersion, majorVersion };
					infoBlock.AddRange(hash1);
					var descriptorsCount = GkDatabase.Descriptors.Count();
					infoBlock.AddRange(BitConverter.GetBytes(descriptorsCount));
					var fileSize = 1000;
					infoBlock.AddRange(BitConverter.GetBytes(fileSize));
					infoBlock.AddRange(BitConverter.GetBytes(DateTime.Now.Ticks));
					while (infoBlock.Count < 256)
						infoBlock.Add(0);

					return infoBlock;
			}
			return new List<byte>();
		}

		public static List<byte> GetJournalBytes(int no)
		{
			var imitatorJournalItems = DBHelper.ImitatorJournalItemCollection.ImitatorJournalItems[no - 1];
			return imitatorJournalItems.ToBytes();
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