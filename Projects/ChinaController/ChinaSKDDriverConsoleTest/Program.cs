using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ChinaSKDDriverNativeApi;

namespace ConsoleSDK
{
	class Program
	{
		static void Main(string[] args)
		{
			NativeWrapper.CLIENT_Init(OnDisConnectDelegate, 0);
			NativeWrapper.NET_DEVICEINFO netDeviceInfo = new NativeWrapper.NET_DEVICEINFO();
			int error;
			NativeWrapper.CLIENT_Login("172.16.6.53", 37777, "system", "123456", out netDeviceInfo, out error);
			while (true)
			{
				;
			}
			NativeWrapper.CLIENT_Cleanup();
		}

		static void OnDisConnectDelegate(Int32 lLoginID, string pchDVRIP, Int32 nDVRPort, UInt32 dwUser)
		{
			;
		}
	}
}