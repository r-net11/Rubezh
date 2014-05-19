using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;

namespace ControllerSDK
{
	public class NativeSDKWrap
	{
		void CallAll()
		{
			SDKImport.fDisConnectDelegate dCbFunc = new SDKImport.fDisConnectDelegate(OnfDisConnect);
			var result = SDKImport.CLIENT_Init(dCbFunc, (UInt32)0);

			ControllerSDK.SDKImport.NET_DEVICEINFO deviceInfo;
			int error = 0;
			Int32 loginID = SDKImport.CLIENT_Login("172.16.2.55", (UInt16)37777, "admin", "admin", out deviceInfo, out error);

			DevConfig_TypeAndSoftInfo(loginID);
			DevConfig_IPMaskGate(loginID);
			DevConfig_MAC(loginID);
			DevConfig_RecordFinderCaps(loginID);
			DevConfig_CurrentTime(loginID);
			//Dev_QueryLogList(loginID);
			DevConfig_AccessGeneral(loginID);
			DevConfig_AccessControl(loginID);
			DevConfig_AccessTimeSchedule(loginID);

			var result2 = SDKImport.CLIENT_Cleanup();
		}

		void OnfDisConnect(Int32 lLoginID, string pchDVRIP, Int32 nDVRPort, UInt32 dwUser)
		{
			if (dwUser == 0)
			{
				return;
			}
		}

		bool DevConfig_TypeAndSoftInfo(Int32 loginID)
		{
			SDKImport.DHDEV_VERSION_INFO versionInfo = new SDKImport.DHDEV_VERSION_INFO();
			int versionInfoSize = Marshal.SizeOf(versionInfo);
			IntPtr versionInfoPtr = Marshal.AllocHGlobal(versionInfoSize);
			Marshal.StructureToPtr(versionInfo, versionInfoPtr, false);

			int nRet = 0;
			var result = SDKImport.CLIENT_QueryDevState(loginID, 0x000F, versionInfoPtr, versionInfoSize, out nRet, 3000);

			var bufferResult = (SDKImport.DHDEV_VERSION_INFO)Marshal.PtrToStructure(versionInfoPtr, typeof(SDKImport.DHDEV_VERSION_INFO));

			return result;
		}

		bool DevConfig_IPMaskGate(Int32 loginID)
		{
			int nError = 0;
			int size = 32 * 1024;
			char[] szBuffer = new char[size];
			var result = SDKImport.CLIENT_GetNewDevConfig(loginID, "Network", -1, szBuffer, 32 * 1024, out nError, 3000);

			SDKImport.CFG_NETWORK_INFO networkInfo = new SDKImport.CFG_NETWORK_INFO();
			int networkInfoSize = Marshal.SizeOf(networkInfo);
			IntPtr networkInfoPtr = Marshal.AllocHGlobal(networkInfoSize);
			Marshal.StructureToPtr(networkInfo, networkInfoPtr, false);

			SDKImport.CLIENT_ParseData("Network", szBuffer, networkInfoPtr, networkInfoSize, IntPtr.Zero);


			return true;


			char[] szOutBuffer = new char[32 * 1024];
			var bRet = SDKImport.CLIENT_PacketData("Network", networkInfoPtr, networkInfoSize, szOutBuffer, 32 * 1024);

			int nRestart = 0;
			SDKImport.CLIENT_SetNewDevConfig(loginID, "Network", -1, szOutBuffer, 32 * 1024, out nError, out nRestart, 3000);

			return true;
		}

		bool DevConfig_MAC(Int32 loginID)
		{
			SDKImport.DHDEV_NETINTERFACE_INFO netInterfaceInfo = new SDKImport.DHDEV_NETINTERFACE_INFO();
			int netInterfaceInfoSize = Marshal.SizeOf(netInterfaceInfo);
			IntPtr netInterfaceInfoPtr = Marshal.AllocHGlobal(netInterfaceInfoSize);
			Marshal.StructureToPtr(netInterfaceInfo, netInterfaceInfoPtr, false);

			int nRet = 0;
			bool bRet = SDKImport.CLIENT_QueryDevState(loginID, 0x0045, netInterfaceInfoPtr, netInterfaceInfoSize, out nRet, 3000);
			return true;
		}

		bool DevConfig_RecordFinderCaps(Int32 loginID)
		{
			int nError = 0;
			char[] szBuffer = new char[32 * 1024];
			bool bRet = SDKImport.CLIENT_QueryNewSystemInfo(loginID, "RecordFinder.getCaps", -1, szBuffer, 32 * 1024, out nError, 5000);
			if (bRet)
			{
				SDKImport.CFG_CAP_RECORDFINDER_INFO recordFinderInfo = new SDKImport.CFG_CAP_RECORDFINDER_INFO();
				int recordFinderInfoSize = Marshal.SizeOf(recordFinderInfo);
				IntPtr recordFinderInfoPtr = Marshal.AllocHGlobal(recordFinderInfoSize);
				Marshal.StructureToPtr(recordFinderInfo, recordFinderInfoPtr, false);

				bRet = SDKImport.CLIENT_ParseData("RecordFinder.getCaps", szBuffer, recordFinderInfoPtr, recordFinderInfoSize, IntPtr.Zero);
			}
			return true;
		}

		bool DevConfig_CurrentTime(Int32 loginID)
		{
			SDKImport.NET_TIME netTime = new SDKImport.NET_TIME();
			int netTimeSize = Marshal.SizeOf(netTime);
			IntPtr netTimePtr = Marshal.AllocHGlobal(netTimeSize);
			Marshal.StructureToPtr(netTime, netTimePtr, false);


			bool bRet = SDKImport.CLIENT_QueryDeviceTime(loginID, netTimePtr, 5000);
			if (bRet)
			{
				return true;
				bRet = SDKImport.CLIENT_SetupDeviceTime(loginID, netTimePtr);
			}
			return true;
		}

		bool Dev_QueryLogList(Int32 loginID)
		{
			SDKImport.QUERY_DEVICE_LOG_PARAM stuGetLog = new SDKImport.QUERY_DEVICE_LOG_PARAM();
			stuGetLog.emLogType = SDKImport.DH_LOG_QUERY_TYPE.DHLOG_ALL;
			stuGetLog.stuStartTime.dwYear = 2013;
			stuGetLog.stuStartTime.dwMonth = 10;
			stuGetLog.stuStartTime.dwDay = 1;
			stuGetLog.stuStartTime.dwHour = 0;
			stuGetLog.stuStartTime.dwMinute = 0;
			stuGetLog.stuStartTime.dwSecond = 0;

			stuGetLog.stuEndTime.dwYear = 2013;
			stuGetLog.stuEndTime.dwMonth = 10;
			stuGetLog.stuEndTime.dwDay = 7;
			stuGetLog.stuEndTime.dwHour = 0;
			stuGetLog.stuEndTime.dwMinute = 0;
			stuGetLog.stuEndTime.dwSecond = 0;

			stuGetLog.nLogStuType = 1;

			int nMaxNum = 20;
			stuGetLog.nStartNum = 0;
			stuGetLog.nEndNum = nMaxNum - 1;

			SDKImport.DH_DEVICE_LOG_ITEM_EX[] logItems = new SDKImport.DH_DEVICE_LOG_ITEM_EX[32];
			int logItemsSize = Marshal.SizeOf(logItems);
			IntPtr netTimePtr = Marshal.AllocHGlobal(logItemsSize);
			Marshal.StructureToPtr(logItems, netTimePtr, false);

			int nRetLogNum = 0;
			bool bRet = SDKImport.CLIENT_QueryDeviceLog(loginID, netTimePtr, netTimePtr, logItemsSize, out nRetLogNum, 5000);
			if (bRet)
			{
				if (nRetLogNum < nMaxNum)
				{
					stuGetLog.nStartNum += nRetLogNum;
					nRetLogNum = 0;
					bRet = SDKImport.CLIENT_QueryDeviceLog(loginID, netTimePtr, netTimePtr, logItemsSize, out nRetLogNum, 5000);
				}
			}
			return true;
		}

		void DevConfig_AccessGeneral(Int32 loginId)
		{
			char[] szJsonBuf = new char[1024 * 40];
			int nerror = 0;

			bool bRet = SDKImport.CLIENT_GetNewDevConfig(loginId, "AccessControlGeneral", -1, szJsonBuf, 1024 * 40, out nerror, 5000);

			if (bRet)
			{
				SDKImport.CFG_ACCESS_GENERAL_INFO accessGeneralInfo = new SDKImport.CFG_ACCESS_GENERAL_INFO();
				int accessGeneralInfoSize = Marshal.SizeOf(accessGeneralInfo);
				IntPtr accessGeneralInfoPtr = Marshal.AllocHGlobal(accessGeneralInfoSize);
				Marshal.StructureToPtr(accessGeneralInfo, accessGeneralInfoPtr, false);


				bRet = SDKImport.CLIENT_ParseData("AccessControlGeneral", szJsonBuf, accessGeneralInfoPtr, accessGeneralInfoSize, IntPtr.Zero);

				if (bRet)
				{
					char[] szJsonBufSet = new char[1024 * 40];
					{
						accessGeneralInfo.abProjectPassword = true;
						accessGeneralInfo.szProjectPassword[0] = 'a';
						accessGeneralInfo.szProjectPassword[1] = 'b';
						accessGeneralInfo.szProjectPassword[2] = 'c';
					}

					bRet = SDKImport.CLIENT_PacketData("AccessControlGeneral", accessGeneralInfoPtr, accessGeneralInfoSize, szJsonBufSet, 1024 * 40);

					nerror = 0;
					int nrestart = 0;
					bRet = SDKImport.CLIENT_SetNewDevConfig(loginId, "AccessControlGeneral", -1, szJsonBufSet, 1024 * 40, out nerror, out nrestart, 5000);
				}
			}
		}

		void DevConfig_AccessControl(Int32 loginId)
		{
			char[] szJsonBuf = new char[1024 * 40];
			int nerror = 0;
			int nChannel = 0;

			bool bRet = SDKImport.CLIENT_GetNewDevConfig(loginId, "AccessControl", nChannel, szJsonBuf, 1024 * 40, out nerror, 5000);

			if (bRet)
			{
				SDKImport.CFG_ACCESS_EVENT_INFO accessEventInfo = new SDKImport.CFG_ACCESS_EVENT_INFO();
				int accessEventInfoSize = Marshal.SizeOf(accessEventInfo);
				IntPtr accessEventInfoPtr = Marshal.AllocHGlobal(accessEventInfoSize);
				Marshal.StructureToPtr(accessEventInfo, accessEventInfoPtr, false);

				bRet = SDKImport.CLIENT_ParseData("AccessControl", szJsonBuf, accessEventInfoPtr, accessEventInfoSize, IntPtr.Zero);

				if (bRet)
				{
					char[] szJsonBufSet = new char[1024 * 40];

					bRet = SDKImport.CLIENT_PacketData("AccessControl", accessEventInfoPtr, accessEventInfoSize, szJsonBufSet, 1024 * 40);
					nerror = 0;
					int nrestart = 0;
					bRet = SDKImport.CLIENT_SetNewDevConfig(loginId, "AccessControl", nChannel, szJsonBufSet, 1024 * 40, out nerror, out nrestart, 5000);
				}
			}
		}

		void DevConfig_AccessTimeSchedule(Int32 loginId)
		{
			char[] szJsonBuf = new char[1024 * 40];
			int nerror = 0;
			int nChannel = 0;

			bool bRet = SDKImport.CLIENT_GetNewDevConfig(loginId, "AccessTimeSchedule", nChannel, szJsonBuf, 1024 * 40, out nerror, 5000);

			if (bRet)
			{
				SDKImport.CFG_ACCESS_TIMESCHEDULE_INFO accessTimeSheduleInfo = new SDKImport.CFG_ACCESS_TIMESCHEDULE_INFO();
				int accessTimeSheduleInfoSize = Marshal.SizeOf(accessTimeSheduleInfo);
				IntPtr accessTimeSheduleInfoPtr = Marshal.AllocHGlobal(accessTimeSheduleInfoSize);
				Marshal.StructureToPtr(accessTimeSheduleInfo, accessTimeSheduleInfoPtr, false);

				bRet = SDKImport.CLIENT_ParseData("AccessTimeSchedule", szJsonBuf, accessTimeSheduleInfoPtr, accessTimeSheduleInfoSize, IntPtr.Zero);

				if (bRet)
				{
					char[] szJsonBufSet = new char[1024 * 40];

					bRet = SDKImport.CLIENT_PacketData("AccessTimeSchedule", accessTimeSheduleInfoPtr, 1024 * 40, szJsonBufSet, 1024 * 40);
					nerror = 0;
					int nrestart = 0;
					bRet = SDKImport.CLIENT_SetNewDevConfig(loginId, "AccessTimeSchedule", nChannel, szJsonBufSet, 1024 * 40, out nerror, out nrestart, 5000);
				}
			}
		}
	}
}