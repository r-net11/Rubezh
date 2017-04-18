using StrazhAPI.Journal;
using StrazhDeviceSDK.API;
using StrazhDeviceSDK.NativeAPI;
using System;
using System.Runtime.InteropServices;
using System.Threading;

namespace StrazhDeviceSDK
{
	public partial class Wrapper
	{
		private const int DH_ALARM_ACCESS_CTL_EVENT = 0x3181;
		private const int DH_ALARM_ACCESS_CTL_NOT_CLOSE = 0x3177;
		private const int DH_ALARM_ACCESS_CTL_BREAK_IN = 0x3178;
		private const int DH_ALARM_ACCESS_CTL_REPEAT_ENTER = 0x3179;
		private const int DH_ALARM_ACCESS_CTL_DURESS = 0x3180;
		private const int DH_ALARM_ACCESS_CTL_STATUS = 0x3185;
		private const int DH_ALARM_CHASSISINTRUDED = 0x3173;
		private const int DH_ALARM_OPENDOORGROUP = 0x318c;   // Multi-people group unlock event
		private const int DH_ALARM_FINGER_PRINT = 0x318d;
		private const int DH_ALARM_ALARM_EX2 = 0x2175;      // local alarm event
		private const int DH_ALARM_ACCESS_LOCK_STATUS = 0x31a8; // Door status events

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
			var deviceInfo = new NativeAPI.NativeWrapper.NET_DEVICEINFO();
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

		private static NativeWrapper.fDisConnectDelegate fDisConnectDelegate;
		private static NativeWrapper.fHaveReConnectDelegate fHaveReConnectDelegate;
		private static NativeWrapper.fMessCallBackDelegate fMessCallBackDelegate;

		private static void OnDisConnectDelegate(Int32 lLoginID, string pchDVRIP, Int32 nDVRPort, UInt32 dwUser)
		{
			var journalItem = new SKDJournalItem
			{
				LoginID = lLoginID,
				SystemDateTime = DateTime.Now,
				JournalEventNameType = JournalEventNameType.Потеря_связи
			};
			AddJournalItem(journalItem);
		}

		private static void OnfHaveReConnectDelegate(Int32 lLoginID, string pchDVRIP, Int32 nDVRPort, UInt32 dwUser)
		{
			var journalItem = new SKDJournalItem
			{
				LoginID = lLoginID,
				SystemDateTime = DateTime.Now,
				JournalEventNameType = JournalEventNameType.Восстановление_связи
			};
			AddJournalItem(journalItem);

			var thread = new Thread(() =>
			{
				NativeWrapper.CLIENT_StopListen(lLoginID);
				NativeWrapper.CLIENT_StartListenEx(lLoginID);
			});
			thread.Start();
		}

		private static bool OnfMessCallBackDelegate(Int32 lCommand, Int32 lLoginID, IntPtr pBuf, UInt32 dwBufLen, string pchDVRIP, Int32 nDVRPort, UInt32 dwUser)
		{
			var journalItem = new SKDJournalItem { LoginID = lLoginID };

			switch (lCommand)
			{
				case DH_ALARM_ACCESS_CTL_EVENT:
					{
						var eventInfo = (NativeWrapper.ALARM_ACCESS_CTL_EVENT_INFO)(Marshal.PtrToStructure(pBuf, typeof(NativeWrapper.ALARM_ACCESS_CTL_EVENT_INFO)));
						journalItem.DeviceDateTime = NET_TIMEToDateTime(eventInfo.stuTime);

						if (eventInfo.bStatus)
							journalItem.JournalEventNameType = JournalEventNameType.Проход_разрешен;
						else
							journalItem.JournalEventNameType = JournalEventNameType.Проход_запрещен;

						journalItem.DoorNo = eventInfo.nDoor;
						journalItem.emEventType = eventInfo.emEventType;
						journalItem.bStatus = eventInfo.bStatus;
						journalItem.emCardType = eventInfo.emCardType;
						journalItem.emOpenMethod = eventInfo.emOpenMethod;
						journalItem.szPwd = eventInfo.szPwd;
						journalItem.szReaderID = eventInfo.szReaderID;
						journalItem.CardNo = eventInfo.szCardNo;
						journalItem.ErrorCode = (ErrorCode)eventInfo.nErrorCode;
						journalItem.No = eventInfo.nPunchingRecNo;

						break;
					}

				case DH_ALARM_ACCESS_CTL_NOT_CLOSE:
					{
						var eventInfo = (NativeWrapper.ALARM_ACCESS_CTL_NOT_CLOSE_INFO)(Marshal.PtrToStructure(pBuf, typeof(NativeWrapper.ALARM_ACCESS_CTL_NOT_CLOSE_INFO)));
						if (eventInfo.nAction == 0)
							journalItem.JournalEventNameType = JournalEventNameType.Дверь_не_закрыта_начало;
						else if (eventInfo.nAction == 1)
							journalItem.JournalEventNameType = JournalEventNameType.Дверь_не_закрыта_конец;
						journalItem.DeviceDateTime = NET_TIMEToDateTime(eventInfo.stuTime);
						journalItem.DoorNo = eventInfo.nDoor;
						journalItem.nAction = eventInfo.nAction;
						journalItem.szDoorName = eventInfo.szDoorName;
						journalItem.No = eventInfo.nEventID;
						break;
					}

				case DH_ALARM_ACCESS_CTL_BREAK_IN:
					{
						journalItem.JournalEventNameType = JournalEventNameType.Взлом;
						var eventInfo = (NativeWrapper.ALARM_ACCESS_CTL_BREAK_IN_INFO)(Marshal.PtrToStructure(pBuf, typeof(NativeWrapper.ALARM_ACCESS_CTL_BREAK_IN_INFO)));
						journalItem.DeviceDateTime = NET_TIMEToDateTime(eventInfo.stuTime);
						journalItem.DoorNo = eventInfo.nDoor;
						journalItem.No = eventInfo.nEventID;
						break;
					}

				case DH_ALARM_ACCESS_CTL_REPEAT_ENTER:
					{
						journalItem.JournalEventNameType = JournalEventNameType.Повторный_проход;
						var eventInfo = (NativeWrapper.ALARM_ACCESS_CTL_REPEAT_ENTER_INFO)(Marshal.PtrToStructure(pBuf, typeof(NativeWrapper.ALARM_ACCESS_CTL_REPEAT_ENTER_INFO)));
						journalItem.DeviceDateTime = NET_TIMEToDateTime(eventInfo.stuTime);
						journalItem.DoorNo = eventInfo.nDoor;
						journalItem.szDoorName = eventInfo.szDoorName;
						journalItem.No = eventInfo.nEventID;
						break;
					}

				case DH_ALARM_ACCESS_CTL_DURESS:
					{
						journalItem.JournalEventNameType = JournalEventNameType.Принуждение;
						var eventInfo = (NativeWrapper.ALARM_ACCESS_CTL_DURESS_INFO)(Marshal.PtrToStructure(pBuf, typeof(NativeWrapper.ALARM_ACCESS_CTL_DURESS_INFO)));
						journalItem.DeviceDateTime = NET_TIMEToDateTime(eventInfo.stuTime);
						journalItem.DoorNo = eventInfo.nDoor;
						journalItem.CardNo = eventInfo.szCardNo;
						journalItem.No = eventInfo.nEventID;
						break;
					}

				case DH_ALARM_ACCESS_CTL_STATUS:
					{
						var eventInfo = (NativeWrapper.ALARM_ACCESS_CTL_STATUS_INFO)(Marshal.PtrToStructure(pBuf, typeof(NativeWrapper.ALARM_ACCESS_CTL_STATUS_INFO)));
						journalItem.DeviceDateTime = NET_TIMEToDateTime(eventInfo.stuTime);
						switch (eventInfo.emStatus)
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

						journalItem.DoorNo = eventInfo.nDoor;
						break;
					}

				case DH_ALARM_CHASSISINTRUDED:
					{
						var eventInfo = (NativeWrapper.ALARM_CHASSISINTRUDED_INFO)(Marshal.PtrToStructure(pBuf, typeof(NativeWrapper.ALARM_CHASSISINTRUDED_INFO)));

						if (eventInfo.nAction == 0)
							journalItem.JournalEventNameType = JournalEventNameType.Вскрытие_контроллера_начало;
						else if (eventInfo.nAction == 1)
							journalItem.JournalEventNameType = JournalEventNameType.Вскрытие_контроллера_конец;

						journalItem.DeviceDateTime = NET_TIMEToDateTime(eventInfo.stuTime);
						journalItem.nAction = eventInfo.nAction;
						journalItem.szReaderID = eventInfo.szReaderID;
						break;
					}

				case DH_ALARM_OPENDOORGROUP:
					{
						journalItem.JournalEventNameType = JournalEventNameType.Множественный_проход;
						var eventInfo = (NativeWrapper.ALARM_OPEN_DOOR_GROUP_INFO)(Marshal.PtrToStructure(pBuf, typeof(NativeWrapper.ALARM_OPEN_DOOR_GROUP_INFO)));
						journalItem.DeviceDateTime = NET_TIMEToDateTime(eventInfo.stuTime);
						journalItem.DoorNo = eventInfo.nChannelID;
						break;
					}

				case DH_ALARM_FINGER_PRINT:
					{
						journalItem.JournalEventNameType = JournalEventNameType.Проход_по_отпечатку_пальца;
						var eventInfo = (NativeWrapper.ALARM_CAPTURE_FINGER_PRINT_INFO)(Marshal.PtrToStructure(pBuf, typeof(NativeWrapper.ALARM_CAPTURE_FINGER_PRINT_INFO)));
						journalItem.DeviceDateTime = NET_TIMEToDateTime(eventInfo.stuTime);
						journalItem.DoorNo = eventInfo.nChannelID;
						journalItem.szReaderID = eventInfo.szReaderID;
						break;
					}

				case DH_ALARM_ALARM_EX2:
					{
						var eventInfo = (NativeWrapper.ALARM_ALARM_INFO_EX2)(Marshal.PtrToStructure(pBuf, typeof(NativeWrapper.ALARM_ALARM_INFO_EX2)));
						if (eventInfo.nAction == 0)
							journalItem.JournalEventNameType = JournalEventNameType.Местная_тревога_начало;
						else if (eventInfo.nAction == 1)
							journalItem.JournalEventNameType = JournalEventNameType.Местная_тревога_конец;
						journalItem.DeviceDateTime = NET_TIMEToDateTime(eventInfo.stuTime);
						journalItem.DoorNo = eventInfo.nChannelID;
						journalItem.nAction = eventInfo.nAction;
						break;
					}

				case DH_ALARM_ACCESS_LOCK_STATUS:
					{
						var eventInfo = (NativeWrapper.ALARM_ACCESS_LOCK_STATUS_INFO)(Marshal.PtrToStructure(pBuf, typeof(NativeWrapper.ALARM_ACCESS_LOCK_STATUS_INFO)));
						journalItem.DeviceDateTime = NET_TIMEToDateTime(eventInfo.stuTime);
						switch (eventInfo.emLockStatus)
						{
							case NativeWrapper.NET_ACCESS_CTL_STATUS_TYPE.NET_ACCESS_CTL_STATUS_TYPE_UNKNOWN:
								journalItem.JournalEventNameType = JournalEventNameType.Неизвестный_статус_замка_двери;
								break;

							case NativeWrapper.NET_ACCESS_CTL_STATUS_TYPE.NET_ACCESS_CTL_STATUS_TYPE_OPEN:
								journalItem.JournalEventNameType = JournalEventNameType.Открытие_замка_двери;
								break;

							case NativeWrapper.NET_ACCESS_CTL_STATUS_TYPE.NET_ACCESS_CTL_STATUS_TYPE_CLOSE:
								journalItem.JournalEventNameType = JournalEventNameType.Закрытие_замка_двери;
								break;

							default:
								journalItem.JournalEventNameType = JournalEventNameType.Неизвестный_статус_замка_двери;
								break;
						}

						journalItem.DoorNo = eventInfo.nChannel;
						break;
					}

				default:
					journalItem.JournalEventNameType = JournalEventNameType.Неизвестное_событие;
					journalItem.Description = lCommand.ToString();
					break;
			}
			Processor.TimeZoneCorrection(journalItem);

			AddJournalItem(journalItem);
			return true;
		}

		private static void AddJournalItem(SKDJournalItem journalItem)
		{
			if (NewJournalItem != null)
				NewJournalItem(journalItem);
		}
	}
}