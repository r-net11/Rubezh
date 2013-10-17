using FiresecAPI.Models;
using XFiresecAPI;
using Graybox.OPC.ServerToolkit.CLRWrapper;

namespace GKOPCServer
{
	public class TagDirection : TagBase
	{
		public XDirectionState DirectionState { get; private set; }

		public TagDirection(int tagId, XDirectionState directionState)
		{
			TagId = tagId;
			DirectionState = directionState;
			directionState.StateChanged += new System.Action(OnStateChanged);
		}

		void OnStateChanged()
		{
			ChangedState(DirectionState.StateClass);
		}
	}
}