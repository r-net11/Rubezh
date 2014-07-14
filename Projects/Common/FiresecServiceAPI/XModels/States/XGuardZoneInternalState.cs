namespace FiresecAPI.GK
{
	public class XGuardZoneInternalState : XBaseInternalState
	{
		public XGuardZone Zone { get; set; }

		public XGuardZoneInternalState(XGuardZone zone)
		{
			Zone = zone;
		}
	}
}