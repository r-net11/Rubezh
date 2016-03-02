using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using RubezhAPI.GK;
using GKImitator.ViewModels;
using GKProcessor;
using RubezhDAL.DataClasses;
using System.Diagnostics;
using RubezhAPI;

namespace GKImitator.Processor
{
	public class NetProcessor
	{
		GkDatabase GkDatabase;
		Socket serverSocket;
		byte[] byteData = new byte[1000];

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

		[DebuggerStepThrough]
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

		[DebuggerStepThrough]
		void OnSend(IAsyncResult ar)
		{
			serverSocket.EndSend(ar);
		}

		List<byte> CreateAnswer()
		{
			DatabaseType databaseType = DatabaseType.Gk;
			if (byteData[0] == 2) databaseType = DatabaseType.Gk;
			if (byteData[0] == 4) databaseType = DatabaseType.Kau;
			var kauAddress = byteData[1];

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

				case 5: // Синхронизация времени
					var descriptorNo = BytesHelper.SubstructInt(byteData.ToList(), 5);
					var descriptorViewModel = MainViewModel.Current.Descriptors.FirstOrDefault(x => databaseType == DatabaseType.Gk ? x.GKDescriptorNo == descriptorNo : x.KauDescriptorNo == descriptorNo);
					if (descriptorViewModel != null)
					{
						descriptorViewModel.SynchronyzeDateTime();
					}
					return new List<byte>();

				case 6: // Чтение последней записи журнала событий
					using(var dbService = new DbService())
					{
						if (JournalCash.Count > 0)
							return GetJournalBytes(JournalCash.Count);
					}
					return null;

				case 7: // Чтение конкретной записи журнала событий
					descriptorNo = BytesHelper.SubstructInt(byteData.ToList(), 5);
					return GetJournalBytes(descriptorNo);

				case 9: // Чтение параметра
					descriptorNo = BytesHelper.SubstructInt(byteData.ToList(), 5);
					if (databaseType == DatabaseType.Gk)
					{
						descriptorViewModel = MainViewModel.Current.Descriptors.FirstOrDefault(x => databaseType == DatabaseType.Gk ? x.GKDescriptorNo == descriptorNo : x.KauDescriptorNo == descriptorNo);
						return descriptorViewModel.GetParameters(databaseType);
					}
					if (databaseType == DatabaseType.Kau)
					{
						descriptorViewModel = MainViewModel.Current.Descriptors.FirstOrDefault(x => databaseType == DatabaseType.Gk ? x.GKDescriptorNo == descriptorNo : x.KauDescriptorNo == descriptorNo);
						return descriptorViewModel.GetParameters(databaseType);
					}
					return null;

				case 10: // Установка параметра
					descriptorNo = BytesHelper.SubstructShort(byteData.ToList(), 5);
					if (databaseType == DatabaseType.Gk)
					{
						descriptorViewModel = MainViewModel.Current.Descriptors.FirstOrDefault(x => databaseType == DatabaseType.Gk ? x.GKDescriptorNo == descriptorNo : x.KauDescriptorNo == descriptorNo);
						if (descriptorViewModel != null)
						{
							descriptorViewModel.SetParameters(byteData.Skip(7).ToList());
							descriptorViewModel.InitializeDelays();
						}
						return new List<byte>();
					}
					if (databaseType == DatabaseType.Kau)
					{
						descriptorViewModel = MainViewModel.Current.Descriptors.FirstOrDefault(x => x.GKBase.KAUDescriptorNo == descriptorNo);
						if (descriptorViewModel != null)
						{
							descriptorViewModel.SetParameters(byteData.Skip(7).ToList());
							descriptorViewModel.InitializeDelays();
						}
						return new List<byte>();
					}
					return null;

				case 12: // Запрос состояния
					descriptorNo = BytesHelper.SubstructShort(byteData.ToList(), 5);
					descriptorViewModel = MainViewModel.Current.Descriptors.FirstOrDefault(x => databaseType == DatabaseType.Gk ? x.GKDescriptorNo == descriptorNo : x.KauDescriptorNo == descriptorNo);
					if (descriptorViewModel != null)
					{
						return descriptorViewModel.GetStateBytes(descriptorNo, databaseType);
					}
					return null;

				case 13: // Команда управления
					descriptorNo = BytesHelper.SubstructShort(byteData.ToList(), 5);
					int commandCode = byteData[7];
					if(commandCode != 2 && commandCode != 3)
					{
						commandCode = commandCode - 0x80;
					}
					var stateBit = (GKStateBit)commandCode;
					descriptorViewModel = MainViewModel.Current.Descriptors.FirstOrDefault(x => databaseType == DatabaseType.Gk ? x.GKDescriptorNo == descriptorNo : x.KauDescriptorNo == descriptorNo);
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

				case 24: // Чтение пользователя
					return UsersProcessor.ReadUser(BytesHelper.SubstructShort(byteData.ToList(), 6));

				case 25: // Добавление пользователя
					UsersProcessor.AddUser(byteData.Skip(6).ToList());
					return new List<byte>();

				case 26: // Редактирование пользователя
					UsersProcessor.EditUser(byteData.Skip(6).ToList());
					return new List<byte>();

				case 28: // Редактирование графика работ
					SchedulesProcessor.EditShedule(byteData.Skip(6).ToList());
					return new List<byte>();
			}
			return new List<byte>();
		}

		public static List<byte> GetJournalBytes(int no)
		{
			var imitatorJournalItem = JournalCash.GetByGKNo(no);
			if (imitatorJournalItem != null)
			{
				return imitatorJournalItem.ToBytes();
			}
			return null;
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