using System;
using System.Runtime.InteropServices;
using ChinaSKDDriverAPI;
using ChinaSKDDriverNativeApi;
using FiresecAPI.SKD;

namespace ChinaSKDDriver
{
	public partial class Wrapper
	{
		public bool GetAllLogs()
		{
			NativeWrapper.WRAP_QueryStart(LoginID);


			int structSize = Marshal.SizeOf(typeof(NativeWrapper.WRAP_Dev_QueryLogList_Result));
			IntPtr intPtr = Marshal.AllocCoTaskMem(structSize);

			var result = NativeWrapper.WRAP_QueryNext(intPtr);

			NativeWrapper.WRAP_Dev_QueryLogList_Result nativeCard = (NativeWrapper.WRAP_Dev_QueryLogList_Result)(Marshal.PtrToStructure(intPtr, typeof(NativeWrapper.WRAP_Dev_QueryLogList_Result)));
			Marshal.FreeCoTaskMem(intPtr);
			intPtr = IntPtr.Zero;


			NativeWrapper.WRAP_QueryStop();
			return true;
		}
	}
}