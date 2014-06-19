using System;
using System.Linq;
using ChinaSKDDriverNativeApi;
using System.Runtime.InteropServices;
using ChinaSKDDriverAPI;
using System.Threading;
using System.Collections.Generic;

namespace ChinaSKDDriver
{
	public partial class Wrapper
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

		#region CommonDevice
		public DeviceSoftwareInfo GetDeviceSoftwareInfo()
		{
			NativeWrapper.WRAP_DevConfig_TypeAndSoftInfo_Result outResult;
			var result = NativeWrapper.WRAP_DevConfig_TypeAndSoftInfo(LoginID, out outResult);

			DeviceSoftwareInfo deviceSoftwareInfo = null;
			if (result)
			{
				deviceSoftwareInfo = new DeviceSoftwareInfo();
				deviceSoftwareInfo.SoftwareBuildDate = new DateTime(outResult.dwSoftwareBuildDate_1, outResult.dwSoftwareBuildDate_2, outResult.dwSoftwareBuildDate_3);
				deviceSoftwareInfo.DeviceType = Wrapper.CharArrayToString(outResult.szDevType);
				deviceSoftwareInfo.SoftwareVersion = Wrapper.CharArrayToString(outResult.szSoftWareVersion);
			}
			return deviceSoftwareInfo;
		}

		public DeviceNetInfo GetDeviceNetInfo()
		{
			NativeWrapper.WRAP_CFG_NETWORK_INFO_Result outResult;
			var result = NativeWrapper.WRAP_Get_DevConfig_IPMaskGate(LoginID, out outResult);

			DeviceNetInfo deviceNetInfo = null;
			if (result)
			{
				deviceNetInfo = new DeviceNetInfo();
				deviceNetInfo.IP = Wrapper.CharArrayToString(outResult.szIP);
				deviceNetInfo.SubnetMask = Wrapper.CharArrayToString(outResult.szSubnetMask);
				deviceNetInfo.DefaultGateway = Wrapper.CharArrayToString(outResult.szDefGateway);
				deviceNetInfo.MTU = outResult.nMTU;
			}
			return deviceNetInfo;
		}
		#endregion

		#region Common

		public int LoginID { get; private set; }

		void OnfDisConnect(Int32 lLoginID, string pchDVRIP, Int32 nDVRPort, UInt32 dwUser)
		{
			if (dwUser == 0)
			{
				return;
			}
		}

		void OnfHaveReConnect(Int32 lLoginID, string pchDVRIP, Int32 nDVRPort, UInt32 dwUser)
		{
			return;
		}

		public int Connect(string ipAddress, int port, string login, string password)
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

		public void StartListen()
		{
			NativeWrapper.fMessCallBack onfMessCallBack = new NativeWrapper.fMessCallBack(OnfMessCallBack);
			NativeWrapper.CLIENT_SetDVRMessCallBack(onfMessCallBack, 0);

			NativeWrapper.CLIENT_StartListenEx(LoginID);
		}

		bool OnfMessCallBack(Int32 lCommand, Int32 lLoginID, IntPtr pBuf, Int32 dwBufLen, string pchDVRIP, Int32 nDVRPort, UInt32 dwUser)
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


		public event Action<SKDJournalItem> NewJournalItem;

		public bool Disconnect()
		{
			var result = NativeWrapper.CLIENT_Cleanup();
			return result;
		}
		#endregion
	}
}