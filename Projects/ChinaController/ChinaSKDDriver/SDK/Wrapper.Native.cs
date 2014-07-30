using System;
using System.Threading;
using ChinaSKDDriverAPI;
using ChinaSKDDriverNativeApi;
using FiresecAPI.Journal;
using System.Runtime.InteropServices;

namespace ChinaSKDDriver
{
	public partial class Wrapper
	{
		public static void Initialize()
		{
			fDisConnectDelegate = new NativeWrapper.fDisConnectDelegate(OnDisConnectDelegate);
			fHaveReConnectDelegate = new NativeWrapper.fHaveReConnectDelegate(OnfHaveReConnectDelegate);
			fMessCallBackDelegate = new NativeWrapper.fMessCallBackDelegate(OnfMessCallBackDelegate);

			NativeWrapper.CLIENT_Init(fDisConnectDelegate, 0);
			NativeWrapper.CLIENT_SetAutoReconnect(fHaveReConnectDelegate, 0);
			NativeWrapper.CLIENT_SetDVRMessCallBack(fMessCallBackDelegate, 0);
		}

		public static void Deinitialize()
		{
			NativeWrapper.CLIENT_Cleanup();
		}

		public int Connect(string ipAddress, int port, string login, string password, out string error)
		{
			var deviceInfo = new ChinaSKDDriverNativeApi.NativeWrapper.NET_DEVICEINFO();
			int intError;
			LoginID = NativeWrapper.CLIENT_Login("172.16.6.54", 37777, "system", "123456", out deviceInfo, out intError);
			error = GetError(intError);
			NativeWrapper.CLIENT_StartListenEx(LoginID);
			return LoginID;
		}

		public void Disconnect()
		{
			NativeWrapper.CLIENT_StopListen(LoginID);
		}

		static NativeWrapper.fDisConnectDelegate fDisConnectDelegate;
		static NativeWrapper.fHaveReConnectDelegate fHaveReConnectDelegate;
		static NativeWrapper.fMessCallBackDelegate fMessCallBackDelegate;

		static void OnDisConnectDelegate(Int32 lLoginID, string pchDVRIP, Int32 nDVRPort, UInt32 dwUser)
		{
			var journalItem = new SKDJournalItem();
			journalItem.LoginID = lLoginID;
			journalItem.SystemDateTime = DateTime.Now;
			journalItem.DeviceDateTime = DateTime.Now;
			journalItem.JournalEventNameType = FiresecAPI.Journal.JournalEventNameType.Потеря_связи;
			AddJournalItem(journalItem);
		}

		static void OnfHaveReConnectDelegate(Int32 lLoginID, string pchDVRIP, Int32 nDVRPort, UInt32 dwUser)
		{
			var journalItem = new SKDJournalItem();
			journalItem.LoginID = lLoginID;
			journalItem.SystemDateTime = DateTime.Now;
			journalItem.DeviceDateTime = DateTime.Now;
			journalItem.JournalEventNameType = FiresecAPI.Journal.JournalEventNameType.Восстановление_связи;
			AddJournalItem(journalItem);

			var thread = new Thread(() =>
			{
				NativeWrapper.CLIENT_StopListen(lLoginID);
				NativeWrapper.CLIENT_StartListenEx(lLoginID);
			});
			thread.Start();
		}

		static bool OnfMessCallBackDelegate(Int32 lCommand, Int32 lLoginID, IntPtr pBuf, UInt32 dwBufLen, string pchDVRIP, Int32 nDVRPort, UInt32 dwUser)
		{
			var journalItem = new SKDJournalItem();
			journalItem.LoginID = lLoginID;
			journalItem.SystemDateTime = DateTime.Now;

			switch (lCommand)
			{
				case DH_ALARM_ACCESS_CTL_EVENT:
					NativeWrapper.ALARM_ACCESS_CTL_EVENT_INFO item1 = (NativeWrapper.ALARM_ACCESS_CTL_EVENT_INFO)(Marshal.PtrToStructure(pBuf, typeof(NativeWrapper.ALARM_ACCESS_CTL_EVENT_INFO)));

					if (item1.bStatus)
						journalItem.JournalEventNameType = JournalEventNameType.Проход_разрешен;
					else
						journalItem.JournalEventNameType = JournalEventNameType.Проход_запрещен;

					journalItem.DoorNo = item1.nDoor;
					journalItem.emEventType = item1.emEventType;
					journalItem.bStatus = item1.bStatus;
					journalItem.emCardType = item1.emCardType;
					journalItem.emOpenMethod = item1.emOpenMethod;
					journalItem.szPwd = item1.szPwd;

					int cardNo;
					if (int.TryParse(item1.szCardNo, out cardNo))
					{
						journalItem.CardNo = cardNo;
					}
					break;

				case DH_ALARM_ACCESS_CTL_NOT_CLOSE:
					NativeWrapper.ALARM_ACCESS_CTL_NOT_CLOSE_INFO item2 = (NativeWrapper.ALARM_ACCESS_CTL_NOT_CLOSE_INFO)(Marshal.PtrToStructure(pBuf, typeof(NativeWrapper.ALARM_ACCESS_CTL_NOT_CLOSE_INFO)));

					journalItem.JournalEventNameType = JournalEventNameType.Дверь_не_закрыта;
					journalItem.DoorNo = item2.nDoor;
					journalItem.nAction = item2.nAction;
					break;

				case DH_ALARM_ACCESS_CTL_BREAK_IN:
					NativeWrapper.ALARM_ACCESS_CTL_BREAK_IN_INFO item3 = (NativeWrapper.ALARM_ACCESS_CTL_BREAK_IN_INFO)(Marshal.PtrToStructure(pBuf, typeof(NativeWrapper.ALARM_ACCESS_CTL_BREAK_IN_INFO)));

					journalItem.JournalEventNameType = JournalEventNameType.Взлом;
					journalItem.DoorNo = item3.nDoor;
					break;

				case DH_ALARM_ACCESS_CTL_REPEAT_ENTER:
					NativeWrapper.ALARM_ACCESS_CTL_REPEAT_ENTER_INFO item4 = (NativeWrapper.ALARM_ACCESS_CTL_REPEAT_ENTER_INFO)(Marshal.PtrToStructure(pBuf, typeof(NativeWrapper.ALARM_ACCESS_CTL_REPEAT_ENTER_INFO)));

					journalItem.JournalEventNameType = JournalEventNameType.Повторный_проход;
					journalItem.DoorNo = item4.nDoor;
					break;

				case DH_ALARM_ACCESS_CTL_DURESS:
					NativeWrapper.ALARM_ACCESS_CTL_DURESS_INFO item5 = (NativeWrapper.ALARM_ACCESS_CTL_DURESS_INFO)(Marshal.PtrToStructure(pBuf, typeof(NativeWrapper.ALARM_ACCESS_CTL_DURESS_INFO)));

					journalItem.JournalEventNameType = JournalEventNameType.Принуждение;
					journalItem.DoorNo = item5.nDoor;

					if (int.TryParse(item5.szCardNo, out cardNo))
					{
						journalItem.CardNo = cardNo;
					}
					break;

				case DH_ALARM_ACCESS_CTL_STATUS:
					NativeWrapper.ALARM_ACCESS_CTL_STATUS_INFO item6 = (NativeWrapper.ALARM_ACCESS_CTL_STATUS_INFO)(Marshal.PtrToStructure(pBuf, typeof(NativeWrapper.ALARM_ACCESS_CTL_STATUS_INFO)));

					NativeWrapper.NET_ACCESS_CTL_STATUS_TYPE status = item6.emStatus;
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

					journalItem.DoorNo = item6.nDoor;
					break;

				default:
					journalItem.JournalEventNameType = JournalEventNameType.Неизвестное_событие;
					break;
			}

			AddJournalItem(journalItem);
			return true;
		}

		static void AddJournalItem(SKDJournalItem journalItem)
		{
			if (NewJournalItem != null)
				NewJournalItem(journalItem);
		}
	}
}