using StrazhDeviceSDK.API;
using StrazhDeviceSDK.NativeAPI;
using FiresecAPI.Journal;
using System;
using System.Threading;

namespace StrazhDeviceSDK
{
	public partial class Wrapper
	{
		public static event Action<SKDJournalItem> NewJournalItem;

		private static Thread Thread;
		private static bool IsStopping;
		private static AutoResetEvent AutoResetEvent = new AutoResetEvent(false);

		public int LoginID { get; private set; }

		public static void WrapInitialize()
		{
			NativeAPI.NativeWrapper.WRAP_Initialize();
		}

		#region Connct

		public int WrapConnect(string ipAddress, int port, string login, string password, out string error)
		{
			int intError = 1;
			LoginID = NativeWrapper.WRAP_Connect(ipAddress, port, login, password, out intError);
			error = GetError(intError);
			return LoginID;
		}

		private string GetError(int error)
		{
			if (error != 0)
			{
				switch (error)
				{
					case 1:
						return "Неверный пароль";

					case 2:
						return "Неверный логин";

					case 3:
						return "Таймаут";

					case 4:
						return "Пользователь уже залогинен";

					case 5:
						return "Пользователь был заблокирован";

					case 6:
						return "Пользователь нелегальный";

					case 7:
						return "Контроллер занят";

					case 9:
						return "Контроллер не найден";

					default:
						return "Неизвестная причина";
				}
			}
			return "";
		}

		public bool WrapDisconnect()
		{
			var result = NativeWrapper.WRAP_Disconnect(LoginID);
			return result;
		}

		#endregion Connct

		public static void WrapStart()
		{
			IsStopping = false;
			AutoResetEvent = new AutoResetEvent(false);
			Thread = new Thread(OnStart);
			Thread.Start();
		}

		public static void WrapStop()
		{
			IsStopping = true;
			if (AutoResetEvent != null)
			{
				AutoResetEvent.Set();
				if (Thread != null)
				{
					Thread.Join(TimeSpan.FromSeconds(1));
				}
			}
		}

		private static void OnStart()
		{
			var lastIndex = -1;
			while (true)
			{
				if (IsStopping)
					return;
				if (AutoResetEvent.WaitOne(TimeSpan.FromMilliseconds(100)))
				{
					return;
				}

				try
				{
					var index = NativeWrapper.WRAP_GetLastIndex();
					if (index > lastIndex)
					{
						for (int i = lastIndex + 1; i <= index; i++)
						{
							var wrapJournalItem = new NativeWrapper.WRAP_JournalItem();
							NativeWrapper.WRAP_GetJournalItem(i, out wrapJournalItem);
							var journalItem = ParceJournal(wrapJournalItem);

							if (NewJournalItem != null)
								NewJournalItem(journalItem);
						}
						lastIndex = index;
					}
				}
				catch { }
			}
		}

		private static SKDJournalItem ParceJournal(NativeWrapper.WRAP_JournalItem wrapJournalItem)
		{
			var journalItem = new SKDJournalItem();
			journalItem.LoginID = wrapJournalItem.LoginID;
			journalItem.SystemDateTime = DateTime.Now;
			journalItem.DeviceDateTime = Wrapper.NET_TIMEToDateTime(wrapJournalItem.DeviceDateTime);

			switch (wrapJournalItem.ExtraEventType)
			{
				case 1:
					journalItem.JournalEventNameType = JournalEventNameType.Потеря_связи;
					return journalItem;

				case 2:
					journalItem.JournalEventNameType = JournalEventNameType.Восстановление_связи;
					return journalItem;
			}

			switch (wrapJournalItem.EventType)
			{
				case DH_ALARM_ACCESS_CTL_EVENT:
					if (wrapJournalItem.bStatus)
						journalItem.JournalEventNameType = JournalEventNameType.Проход_разрешен;
					else
						journalItem.JournalEventNameType = JournalEventNameType.Проход_запрещен;

					journalItem.DoorNo = wrapJournalItem.nDoor;
					journalItem.emEventType = wrapJournalItem.emEventType;
					journalItem.bStatus = wrapJournalItem.bStatus;
					journalItem.emCardType = wrapJournalItem.emCardType;
					journalItem.emOpenMethod = wrapJournalItem.emOpenMethod;
					journalItem.szPwd = wrapJournalItem.szPwd;
					journalItem.CardNo = wrapJournalItem.szCardNo;
					break;

				case DH_ALARM_ACCESS_CTL_NOT_CLOSE:
					if (wrapJournalItem.nAction == 0)
						journalItem.JournalEventNameType = JournalEventNameType.Дверь_не_закрыта_начало;
					else if (wrapJournalItem.nAction == 1)
						journalItem.JournalEventNameType = JournalEventNameType.Дверь_не_закрыта_конец;
					journalItem.DoorNo = wrapJournalItem.nDoor;
					journalItem.nAction = wrapJournalItem.nAction;
					break;

				case DH_ALARM_ACCESS_CTL_BREAK_IN:
					journalItem.JournalEventNameType = JournalEventNameType.Взлом;
					journalItem.DoorNo = wrapJournalItem.nDoor;
					break;

				case DH_ALARM_ACCESS_CTL_REPEAT_ENTER:
					journalItem.JournalEventNameType = JournalEventNameType.Повторный_проход;
					journalItem.DoorNo = wrapJournalItem.nDoor;
					break;

				case DH_ALARM_ACCESS_CTL_DURESS:
					journalItem.JournalEventNameType = JournalEventNameType.Принуждение;
					journalItem.DoorNo = wrapJournalItem.nDoor;
					journalItem.CardNo = wrapJournalItem.szCardNo;
					break;

				case DH_ALARM_ACCESS_CTL_STATUS:
					NativeWrapper.NET_ACCESS_CTL_STATUS_TYPE status = wrapJournalItem.emStatus;
					switch (status)
					{
						case NativeWrapper.NET_ACCESS_CTL_STATUS_TYPE.NET_ACCESS_CTL_STATUS_TYPE_OPEN:
							journalItem.JournalEventNameType = JournalEventNameType.Открытие_двери;
							break;

						case NativeWrapper.NET_ACCESS_CTL_STATUS_TYPE.NET_ACCESS_CTL_STATUS_TYPE_CLOSE:
							journalItem.JournalEventNameType = JournalEventNameType.Закрытие_двери;
							break;

						default:
							journalItem.JournalEventNameType = JournalEventNameType.Неизвестный_статус_двери;
							break;
					}

					journalItem.DoorNo = wrapJournalItem.nDoor;
					break;

				case DH_ALARM_CHASSISINTRUDED:
					journalItem.JournalEventNameType = JournalEventNameType.Вскрытие_контроллера_начало;
					break;

				default:
					journalItem.JournalEventNameType = JournalEventNameType.Неизвестное_событие;
					journalItem.Description = wrapJournalItem.EventType.ToString();
					break;
			}

			return journalItem;
		}
	}
}