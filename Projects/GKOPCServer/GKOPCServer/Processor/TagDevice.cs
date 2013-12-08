using FiresecAPI.Models;
using XFiresecAPI;
using Graybox.OPC.ServerToolkit.CLRWrapper;

namespace GKOPCServer
{
	public class TagDevice : TagBase
	{
		public XState State { get; private set; }

		public TagDevice(int tagId, XState state)
		{
			TagId = tagId;
			State = state;
			state.StateChanged += new System.Action(OnStateChanged);
		}

		void OnStateChanged()
		{
			ChangedState(State.StateClass);
		}
	}
}