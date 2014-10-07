using FiresecAPI.GK;

namespace GKOPCServer
{
	public class TagDirection : TagBase
	{
		public GKState State { get; private set; }

		public TagDirection(int tagId, GKState state)
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