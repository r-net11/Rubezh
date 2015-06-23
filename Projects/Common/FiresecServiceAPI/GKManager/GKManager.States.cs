using FiresecAPI.GK;

namespace FiresecClient
{
	public partial class GKManager
	{
		public static void CreateStates()
		{
			foreach (var device in Devices)
			{
				device.InternalState = new GKDeviceInternalState(device);
				device.State = new GKState(device);
			}
		}

		public static XStateClass GetMinStateClass()
		{
			return XStateClass.No;
		}
	}
}