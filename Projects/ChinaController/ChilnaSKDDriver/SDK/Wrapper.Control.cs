using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using ChinaSKDDriverAPI;
using ChinaSKDDriverNativeApi;

namespace ChinaSKDDriver
{
	public partial class Wrapper
	{
		public bool OpenDoor(int channelNo)
		{
			var result = NativeWrapper.WRAP_OpenDoor(LoginID, channelNo);
			return result;
		}

		public bool CloseDoor(int channelNo)
		{
			var result = NativeWrapper.WRAP_CloseDoor(LoginID, channelNo);
			return result;
		}

		public int GetDoorStatus(int channelNo)
		{
			var result = NativeWrapper.WRAP_GetDoorStatus(LoginID, channelNo);
			return result;
		}
	}
}