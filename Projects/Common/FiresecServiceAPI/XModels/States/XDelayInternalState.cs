namespace FiresecAPI.GK
{
	public class XDelayInternalState : XBaseInternalState
	{
		public XDelay Delay { get; set; }

		public XDelayInternalState(XDelay delay)
		{
			Delay = delay;
		}
	}
}