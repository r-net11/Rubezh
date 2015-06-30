namespace FiresecAPI.GK
{
	public class GKSKDZoneInternalState : GKBaseInternalState
	{
		public GKSKDZone Zone { get; set; }

		public GKSKDZoneInternalState(GKSKDZone zone)
		{
			Zone = zone;
		}
	}
}