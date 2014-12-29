namespace FiresecAPI.GK
{
	public class GKGuardZoneInternalState : GKBaseInternalState
	{
		public GKGuardZone Zone { get; set; }

		public GKGuardZoneInternalState(GKGuardZone zone)
		{
			Zone = zone;
		}
	}
}