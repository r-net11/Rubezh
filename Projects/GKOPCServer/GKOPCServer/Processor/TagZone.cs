using FiresecAPI.GK;
using FiresecAPI.Models;

namespace GKOPCServer
{
	public class TagZone : TagBase
	{
		public GKState ZoneState { get; private set; }

		public TagZone(int tagId, GKState state)
		{
			TagId = tagId;
			ZoneState = state;
			state.StateChanged += new System.Action(OnStateChanged);
		}

		void OnStateChanged()
		{
			ChangedState(ZoneState.StateClass);
		}
	}
}