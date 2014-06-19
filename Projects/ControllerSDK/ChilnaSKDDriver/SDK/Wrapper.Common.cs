using System;
using System.Linq;
using ChinaSKDDriverNativeApi;
using System.Runtime.InteropServices;
using ChinaSKDDriverAPI;
using System.Threading;
using System.Collections.Generic;

namespace ChinaSKDDriver
{
	public static partial class Wrapper
	{
		#region Helpers
		public static string CharArrayToString(char[] charArray)
		{
			var result = new string(charArray);
			int i = result.IndexOf('\0');
			if (i >= 0)
				result = result.Substring(0, i);
			return result;
		}

		public static string CharArrayToStringNoTrim(char[] charArray)
		{
			var result = new string(charArray);
			//int i = result.IndexOf('\0');
			//if (i >= 0)
			//    result = result.Substring(0, i);
			return result;
		}

		public static char[] StringToCharArray(string str, int size)
		{
			var result = new char[size];
			var charArray = str.ToCharArray();
			for (int i = 0; i < Math.Min(charArray.Count(), size); i++)
			{
				result[i] = charArray[i];
			}
			return result;
		}

		public static DateTime NET_TIMEToDateTime(NativeWrapper.NET_TIME netTime)
		{
			DateTime dateTime = DateTime.MinValue;
			try
			{
				if (netTime.dwYear == 0 || netTime.dwMonth == 0 || netTime.dwDay == 0)
					return new DateTime();
				dateTime = new DateTime(netTime.dwYear, netTime.dwMonth, netTime.dwDay, netTime.dwHour, netTime.dwMinute, netTime.dwSecond);
			}
			catch { }
			return dateTime;
		}
		#endregion

		#region Common

		static int LoginID;

		static void OnfDisConnect(Int32 lLoginID, string pchDVRIP, Int32 nDVRPort, UInt32 dwUser)
		{
			if (dwUser == 0)
			{
				return;
			}
		}

		static void OnfHaveReConnect(Int32 lLoginID, string pchDVRIP, Int32 nDVRPort, UInt32 dwUser)
		{
			return;
		}

		public static int Connect(string ipAddress, int port, string login, string password)
		{
			NativeWrapper.fDisConnectDelegate dCbFunc = new NativeWrapper.fDisConnectDelegate(OnfDisConnect);
			var result = NativeWrapper.CLIENT_Init(dCbFunc, (UInt32)0);

			NativeWrapper.NET_DEVICEINFO deviceInfo;
			int error = 0;
			LoginID = NativeWrapper.CLIENT_Login(ipAddress, (UInt16)37777, login, password, out deviceInfo, out error);

			NativeWrapper.fHaveReConnect onHaveReConnect = new NativeWrapper.fHaveReConnect(OnfHaveReConnect);
			NativeWrapper.CLIENT_SetAutoReconnect(onHaveReConnect, 0);

			return LoginID;
		}

		public static void StartListen()
		{
			NativeWrapper.fMessCallBack onfMessCallBack = new NativeWrapper.fMessCallBack(OnfMessCallBack);
			NativeWrapper.CLIENT_SetDVRMessCallBack(onfMessCallBack, 0);

			NativeWrapper.CLIENT_StartListenEx(LoginID);
		}

		static bool OnfMessCallBack(Int32 lCommand, Int32 lLoginID, IntPtr pBuf, Int32 dwBufLen, string pchDVRIP, Int32 nDVRPort, UInt32 dwUser)
		{
			if (lCommand == 0x3181)
			{
				var nativeJournalItem = (NativeWrapper.ALARM_ACCESS_CTL_EVENT_INFO)Marshal.PtrToStructure(pBuf, typeof(NativeWrapper.ALARM_ACCESS_CTL_EVENT_INFO));

				var doorNo = nativeJournalItem.nDoor;
				var status = nativeJournalItem.bStatus;
				var eventType = nativeJournalItem.emEventType;
				var cardType = nativeJournalItem.emCardType;
				var openMethod = nativeJournalItem.emOpenMethod;
				var doorName = Wrapper.CharArrayToString(nativeJournalItem.szDoorName);
				var cardNo = Wrapper.CharArrayToString(nativeJournalItem.szCardNo);
				var password = Wrapper.CharArrayToString(nativeJournalItem.szPwd);
				var dateTime = Wrapper.NET_TIMEToDateTime(nativeJournalItem.stuTime);

				var journalItem = new SKDJournalItem();
				journalItem.SystemDateTime = DateTime.Now;
				journalItem.DeviceDateTime = dateTime;
				journalItem.Name = "Проход через считыватель";

				var description = "";
				switch (eventType)
				{
					case NativeWrapper.NET_ACCESS_CTL_EVENT_TYPE.NET_ACCESS_CTL_EVENT_ENTRY:
						description = "Вход через дверь";
						break;

					case NativeWrapper.NET_ACCESS_CTL_EVENT_TYPE.NET_ACCESS_CTL_EVENT_EXIT:
						description = "Выход через дверь";
						break;

					case NativeWrapper.NET_ACCESS_CTL_EVENT_TYPE.NET_ACCESS_CTL_EVENT_UNKNOWN:
						description = "Неизвестно";
						break;
				}
				journalItem.Description = description;

				if (NewJournalItem != null)
					NewJournalItem(journalItem);
			}

			return true;
		}


		public static event Action<SKDJournalItem> NewJournalItem;

		public static bool Disconnect()
		{
			var result = NativeWrapper.CLIENT_Cleanup();
			return result;
		}
		#endregion
	}
}