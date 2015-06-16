using ChinaSKDDriverNativeApi;

namespace ChinaSKDDriver
{
	public partial class Wrapper
	{
		public bool OpenDoor(int channelNo)
		{
			return NativeWrapper.WRAP_OpenDoor(LoginID, channelNo);
		}

		public bool CloseDoor(int channelNo)
		{
			return NativeWrapper.WRAP_CloseDoor(LoginID, channelNo);
		}

		public int GetDoorStatus(int channelNo)
		{
			return NativeWrapper.WRAP_GetDoorStatus(LoginID, channelNo);
		}
	}
}