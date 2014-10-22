using System;
using System.Threading;
using ChinaSKDDriverAPI;
using ChinaSKDDriverNativeApi;
using FiresecAPI.Journal;
using System.Runtime.InteropServices;
using System.Globalization;

namespace ChinaSKDDriver
{
	public partial class Wrapper
	{
		const int DH_ALARM_ACCESS_CTL_EVENT = 0x3181;
		const int DH_ALARM_ACCESS_CTL_NOT_CLOSE = 0x3177;
		const int DH_ALARM_ACCESS_CTL_BREAK_IN = 0x3178;
		const int DH_ALARM_ACCESS_CTL_REPEAT_ENTER = 0x3179;
		const int DH_ALARM_ACCESS_CTL_DURESS = 0x3180;
		const int DH_ALARM_ACCESS_CTL_STATUS = 0x3185;
		const int DH_ALARM_CHASSISINTRUDED = 0x3173;
		const int DH_ALARM_OPENDOORGROUP = 0x318c;     // Multi-people group unlock event
		const int DH_ALARM_FINGER_PRINT = 0x318d;
		const int DH_ALARM_ALARM_EX2 = 0x2175;		// local alarm event

		public static void Initialize()
		{
			fDisConnectDelegate = new NativeWrapper.fDisConnectDelegate(OnDisConnectDelegate);
			fHaveReConnectDelegate = new NativeWrapper.fHaveReConnectDelegate(OnfHaveReConnectDelegate);
			fMessCallBackDelegate = new NativeWrapper.fMessCallBackDelegate(OnfMessCallBackDelegate);

			var result = NativeWrapper.CLIENT_Init(fDisConnectDelegate, 0);
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
			LoginID = NativeWrapper.CLIENT_Login(ipAddress, (UInt16)port, login, password, out deviceInfo, out intError);
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
			journalItem.JournalEventNameType = FiresecAPI.Journal.JournalEventNameType.Потеря_связи;
			AddJournalItem(journalItem);
		}

		static void OnfHaveReConnectDelegate(Int32 lLoginID, string pchDVRIP, Int32 nDVRPort, UInt32 dwUser)
		{
			var journalItem = new SKDJournalItem();
			journalItem.LoginID = lLoginID;
			journalItem.SystemDateTime = DateTime.Now;
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
					journalItem.DeviceDateTime = NET_TIMEToDateTime(item1.stuTime);

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
					journalItem.szReaderID = item1.szReaderID;

					int cardNo;
					if (int.TryParse(item1.szCardNo, NumberStyles.HexNumber, null, out cardNo))
					{
						journalItem.CardNo = cardNo;
					}
					break;

				case DH_ALARM_ACCESS_CTL_NOT_CLOSE:
					journalItem.JournalEventNameType = JournalEventNameType.Дверь_не_закрыта;
					NativeWrapper.ALARM_ACCESS_CTL_NOT_CLOSE_INFO item2 = (NativeWrapper.ALARM_ACCESS_CTL_NOT_CLOSE_INFO)(Marshal.PtrToStructure(pBuf, typeof(NativeWrapper.ALARM_ACCESS_CTL_NOT_CLOSE_INFO)));
					journalItem.DeviceDateTime = NET_TIMEToDateTime(item2.stuTime);
					journalItem.DoorNo = item2.nDoor;
					journalItem.nAction = item2.nAction;
					journalItem.szDoorName = item2.szDoorName;
					break;

				case DH_ALARM_ACCESS_CTL_BREAK_IN:
					journalItem.JournalEventNameType = JournalEventNameType.Взлом;
					NativeWrapper.ALARM_ACCESS_CTL_BREAK_IN_INFO item3 = (NativeWrapper.ALARM_ACCESS_CTL_BREAK_IN_INFO)(Marshal.PtrToStructure(pBuf, typeof(NativeWrapper.ALARM_ACCESS_CTL_BREAK_IN_INFO)));
					journalItem.DeviceDateTime = NET_TIMEToDateTime(item3.stuTime);
					journalItem.DoorNo = item3.nDoor;
					break;

				case DH_ALARM_ACCESS_CTL_REPEAT_ENTER:
					journalItem.JournalEventNameType = JournalEventNameType.Повторный_проход;
					NativeWrapper.ALARM_ACCESS_CTL_REPEAT_ENTER_INFO item4 = (NativeWrapper.ALARM_ACCESS_CTL_REPEAT_ENTER_INFO)(Marshal.PtrToStructure(pBuf, typeof(NativeWrapper.ALARM_ACCESS_CTL_REPEAT_ENTER_INFO)));
					journalItem.DeviceDateTime = NET_TIMEToDateTime(item4.stuTime);
					journalItem.DoorNo = item4.nDoor;
					journalItem.szDoorName = item4.szDoorName;
					break;

				case DH_ALARM_ACCESS_CTL_DURESS:
					journalItem.JournalEventNameType = JournalEventNameType.Принуждение;
					NativeWrapper.ALARM_ACCESS_CTL_DURESS_INFO item5 = (NativeWrapper.ALARM_ACCESS_CTL_DURESS_INFO)(Marshal.PtrToStructure(pBuf, typeof(NativeWrapper.ALARM_ACCESS_CTL_DURESS_INFO)));
					journalItem.DeviceDateTime = NET_TIMEToDateTime(item5.stuTime);
					journalItem.DoorNo = item5.nDoor;

					if (int.TryParse(item5.szCardNo, out cardNo))
					{
						journalItem.CardNo = cardNo;
					}
					break;

				case DH_ALARM_ACCESS_CTL_STATUS:
					NativeWrapper.ALARM_ACCESS_CTL_STATUS_INFO item6 = (NativeWrapper.ALARM_ACCESS_CTL_STATUS_INFO)(Marshal.PtrToStructure(pBuf, typeof(NativeWrapper.ALARM_ACCESS_CTL_STATUS_INFO)));
					journalItem.DeviceDateTime = NET_TIMEToDateTime(item6.stuTime);
					NativeWrapper.NET_ACCESS_CTL_STATUS_TYPE status = item6.emStatus;
					switch (status)
					{
						case NativeWrapper.NET_ACCESS_CTL_STATUS_TYPE.NET_ACCESS_CTL_STATUS_TYPE_UNKNOWN:
							journalItem.JournalEventNameType = JournalEventNameType.Неизвестный_статус_двери;
							break;

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

				case DH_ALARM_CHASSISINTRUDED:
					journalItem.JournalEventNameType = JournalEventNameType.Вскрытие_контроллера;
					NativeWrapper.ALARM_CHASSISINTRUDED_INFO item7 = (NativeWrapper.ALARM_CHASSISINTRUDED_INFO)(Marshal.PtrToStructure(pBuf, typeof(NativeWrapper.ALARM_CHASSISINTRUDED_INFO)));
					journalItem.DeviceDateTime = NET_TIMEToDateTime(item7.stuTime);
					//journalItem.DoorNo = item7.nChannelID;
					journalItem.nAction = item7.nAction;
					journalItem.szReaderID = item7.szReaderID;
					break;

				case DH_ALARM_OPENDOORGROUP:
					journalItem.JournalEventNameType = JournalEventNameType.Множественный_проход;
					NativeWrapper.ALARM_OPEN_DOOR_GROUP_INFO item8 = (NativeWrapper.ALARM_OPEN_DOOR_GROUP_INFO)(Marshal.PtrToStructure(pBuf, typeof(NativeWrapper.ALARM_OPEN_DOOR_GROUP_INFO)));
					journalItem.DeviceDateTime = NET_TIMEToDateTime(item8.stuTime);
					journalItem.DoorNo = item8.nChannelID;
					break;

				case DH_ALARM_FINGER_PRINT:
					journalItem.JournalEventNameType = JournalEventNameType.Проход_по_отпечатку_пальца;
					NativeWrapper.ALARM_CAPTURE_FINGER_PRINT_INFO item9 = (NativeWrapper.ALARM_CAPTURE_FINGER_PRINT_INFO)(Marshal.PtrToStructure(pBuf, typeof(NativeWrapper.ALARM_CAPTURE_FINGER_PRINT_INFO)));
					journalItem.DeviceDateTime = NET_TIMEToDateTime(item9.stuTime);
					journalItem.DoorNo = item9.nChannelID;
					journalItem.szReaderID = item9.szReaderID;
					break;

				case DH_ALARM_ALARM_EX2:
					journalItem.JournalEventNameType = JournalEventNameType.Местная_тревога;
					NativeWrapper.ALARM_ALARM_INFO_EX2 item10 = (NativeWrapper.ALARM_ALARM_INFO_EX2)(Marshal.PtrToStructure(pBuf, typeof(NativeWrapper.ALARM_ALARM_INFO_EX2)));
					journalItem.DeviceDateTime = NET_TIMEToDateTime(item10.stuTime);
					journalItem.DoorNo = item10.nChannelID;
					journalItem.nAction = item10.nAction;
					break;

				default:
					journalItem.JournalEventNameType = JournalEventNameType.Неизвестное_событие;
					journalItem.Description = lCommand.ToString();
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