namespace FiresecAPI.GK
{
	public class XZoneInternalState : XBaseInternalState
	{
		public XZone Zone { get; set; }

		public XZoneInternalState(XZone zone)
		{
			Zone = zone;
		}
	}
}