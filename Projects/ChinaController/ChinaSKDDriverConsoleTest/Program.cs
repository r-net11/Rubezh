using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ChinaSKDDriverNativeApi;
using System.Threading;

namespace ConsoleSDK
{
	class Program
	{
		static void Main(string[] args)
		{
			NativeWrapper.CLIENT_Init(OnDisConnectDelegate, 0);
			NativeWrapper.CLIENT_SetAutoReconnect(OnfHaveReConnectDelegate, 0);
			NativeWrapper.CLIENT_SetDVRMessCallBack(OnfMessCallBackDelegate, 0);

			NativeWrapper.NET_DEVICEINFO netDeviceInfo = new NativeWrapper.NET_DEVICEINFO();
			int error;
			int loginID = NativeWrapper.CLIENT_Login("172.16.6.53", 37777, "system", "123456", out netDeviceInfo, out error);
			bool bRet = NativeWrapper.CLIENT_StartListenEx(loginID);
			while (true)
			{
				Thread.Sleep(TimeSpan.FromSeconds(1));
				;
			}
			bRet = NativeWrapper.CLIENT_StopListen(loginID);
			NativeWrapper.CLIENT_Cleanup();
		}

		static void OnDisConnectDelegate(Int32 lLoginID, string pchDVRIP, Int32 nDVRPort, UInt32 dwUser)
		{
			;
		}

		static void OnfHaveReConnectDelegate(Int32 lLoginID, string pchDVRIP, Int32 nDVRPort, UInt32 dwUser)
		{
			;
		}

		static bool OnfMessCallBackDelegate(Int32 lCommand, Int32 lLoginID, IntPtr pBuf, UInt32 dwBufLen, string pchDVRIP, Int32 nDVRPort, UInt32 dwUser)
		{
			return true;
		}
	}
}