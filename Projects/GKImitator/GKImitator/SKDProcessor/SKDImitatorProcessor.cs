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
using GKImitator.SKDProcessor;
using FiresecAPI;

namespace GKImitator.Processor
{
	public class SKDImitatorProcessor
	{
		public bool IsConnected { get; set; }
		int Port;
		Socket serverSocket;
		byte[] byteData = new byte[65535];
		public List<SKDImitatorJournalItem> JournalItems { get; set; }
		public int LastJournalNo { get; set; }
		public SKDDataContext Context { get; private set; }

		public SKDImitatorDBDataContext ImitatorContext { get; private set; }
		GKImitator.SKDProcessor.Devices ImitatorDevice;

		public SKDImitatorProcessor(int port)
		{
			Port = port;
			IsConnected = true;
			Context = new SKDDataContext();

			if (Context.Journals.Count() > 0)
				LastJournalNo = Context.Journals.AsEnumerable().OrderBy(x => x.CardNo).LastOrDefault().DeviceNo;
			JournalItems = new List<SKDImitatorJournalItem>();
			JournalItems.Add(new SKDImitatorJournalItem() { No = LastJournalNo });

			ImitatorContext = new SKDImitatorDBDataContext();
			ImitatorDevice = ImitatorContext.Devices.FirstOrDefault(x => x.Port == Port);
			if (ImitatorDevice == null)
			{
				ImitatorDevice = new Devices()
				{
					UID = Guid.NewGuid(),
					Port = Port
				};
				ImitatorContext.GetTable<Devices>().InsertOnSubmit(ImitatorDevice);
				ImitatorContext.SubmitChanges();
			}
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

			if (IsConnected)
			{
				var bytes = CreateAnswer();
				if (bytes != null)
				{
					var sendBytes = new List<byte>();
					sendBytes.Add(byteData[0]);
					sendBytes.AddRange(bytes);
					byte[] message = sendBytes.ToArray();

					serverSocket.BeginSendTo(message, 0, message.Length, SocketFlags.None, epSender, new AsyncCallback(OnSend), epSender);
				}
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
				case 1: // Информация об устройстве
					result.Add(9);
					return result;
				case 2: // Индекс последней записи
					result.AddRange(JournalItems.LastOrDefault().ToBytes());
					return result;
				case 3: // Чтение конкретной записи
					var no = BytesHelper.SubstructInt(byteData.ToList(), 1);
					var journalItem = JournalItems.FirstOrDefault(x => x.No == no);
					if (journalItem != null)
					{
						result.AddRange(journalItem.ToBytes());
					}
					return result;
				case 4: // Состояние устройства
					result.Add((byte)XStateClass.Norm);
					return result;
				case 5: // Запись конфигурации
					return result;
				case 6: // Обновление прошивки
					return result;
				case 7: // Запись всех идентификаторов
					return result;
				case 8: // Команда управления
					OnControlCommand(byteData[1]);
					return result;
				case 9: // Запись одного идентификатора
					AddJournalItem(new SKDImitatorJournalItem() { Source = 1, Address = 0, NameCode = 23, DescriptionCode = 1 });
					return result;
				case 10: // Запись всех временнх интервалов
					OnWriteIntervals();
					return result;
				case 11: // Запрос хэша всех временнх интервалов
					if (ImitatorDevice.TimeIntervalHash != null)
					{
						return ImitatorDevice.BytesHash.ToArray().ToList();
						return Encoding.ASCII.GetBytes(ImitatorDevice.TimeIntervalHash).ToList();
					}
					else
					{
						for (int i = 0; i < 16; i++)
							result.Add(0);
					}
					return result;
			}
			return null;
		}

		void OnControlCommand(byte b)
		{
			AddJournalItem(new SKDImitatorJournalItem() { Source = 1, Address = 0, NameCode = 22, DescriptionCode = b });
		}

		void OnWriteIntervals()
		{
			AddJournalItem(new SKDImitatorJournalItem() { Source = 1, Address = 0, NameCode = 23, DescriptionCode = 2 });

			var bytesHash = new byte[16];
			var stringBuilder = new StringBuilder();
			for(int i = 0; i < 16; i++)
			{
				bytesHash[i] = byteData[i + 1];
				stringBuilder.Append((char)byteData[i + 1]);
			}

			ImitatorDevice.BytesHash = new System.Data.Linq.Binary(bytesHash);
			ImitatorDevice.TimeIntervalHash = stringBuilder.ToString();
			ImitatorContext.SubmitChanges();
		}

		public static List<byte> ToBytes(short shortValue)
		{
			return BitConverter.GetBytes(shortValue).ToList();
		}

		public static List<byte> IntToBytes(int intValue)
		{
			return BitConverter.GetBytes(intValue).ToList();
		}

		public void AddJournalItem(SKDImitatorJournalItem skdImitatorJournalItem)
		{
			LastJournalNo++;
			skdImitatorJournalItem.No = LastJournalNo;
			JournalItems.Add(skdImitatorJournalItem);
			var dbJournal = new SKDProcessor.Journal()
			{
				UID = Guid.NewGuid(),
				DeviceNo = LastJournalNo,
				Name = "",
				Description = "",
				SysemDate = DateTime.Now,
				DeviceDate = skdImitatorJournalItem.DateTime,
				CardNo = skdImitatorJournalItem.CardNo,
				CardSeries = skdImitatorJournalItem.CardSeries,
			};
			Context.Journals.InsertOnSubmit(dbJournal);
			Context.SubmitChanges();
		}
	}
}