namespace RubezhAPI.GK
{
	public class GKZoneInternalState : GKBaseInternalState
	{
		public GKZone Zone { get; set; }

		public GKZoneInternalState(GKZone zone)
		{
			Zone = zone;
		}
	}
}