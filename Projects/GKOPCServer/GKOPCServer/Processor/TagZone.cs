using FiresecAPI.Models;
using XFiresecAPI;

namespace GKOPCServer
{
	public class TagZone : TagBase
	{
		public XZoneState ZoneState { get; private set; }

		public TagZone(int tagId, XZoneState zoneState)
		{
			TagId = tagId;
			ZoneState = zoneState;
			zoneState.StateChanged += new System.Action(OnStateChanged);
		}

		void OnStateChanged()
		{
			ChangedState(ZoneState.StateClass);
		}
	}
}