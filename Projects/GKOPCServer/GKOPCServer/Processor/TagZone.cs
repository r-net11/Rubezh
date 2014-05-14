using FiresecAPI.GK;
using FiresecAPI.Models;

namespace GKOPCServer
{
	public class TagZone : TagBase
	{
		public XState ZoneState { get; private set; }

		public TagZone(int tagId, XState state)
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