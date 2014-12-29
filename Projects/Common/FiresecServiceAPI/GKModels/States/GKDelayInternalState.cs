namespace FiresecAPI.GK
{
	public class GKDelayInternalState : GKBaseInternalState
	{
		public GKDelay Delay { get; set; }

		public GKDelayInternalState(GKDelay delay)
		{
			Delay = delay;
		}
	}
}