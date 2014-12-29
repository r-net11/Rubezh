namespace FiresecAPI.GK
{
	public class GKMPTInternalState : GKBaseInternalState
	{
		public GKMPT MPT { get; set; }

		public GKMPTInternalState(GKMPT mpt)
		{
			MPT = mpt;
		}
	}
}