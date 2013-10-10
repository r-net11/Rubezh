using FiresecAPI.Models;
using XFiresecAPI;
using Graybox.OPC.ServerToolkit.CLRWrapper;

namespace GKOPCServer
{
	public class TagDevice : TagBase
	{
		public XDeviceState DeviceState { get; private set; }

		public TagDevice(int tagId, XDeviceState deviceState)
		{
			TagId = tagId;
			DeviceState = deviceState;
			deviceState.StateChanged += new System.Action(OnStateChanged);
		}

		void OnStateChanged()
		{
			ChangedState(DeviceState.StateClass);
		}
	}
}