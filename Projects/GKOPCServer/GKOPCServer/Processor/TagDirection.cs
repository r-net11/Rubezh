using FiresecAPI.Models;
using XFiresecAPI;
using Graybox.OPC.ServerToolkit.CLRWrapper;

namespace GKOPCServer
{
	public class TagDirection : TagBase
	{
		public XState State { get; private set; }

		public TagDirection(int tagId, XState state)
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