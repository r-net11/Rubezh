namespace FiresecAPI.GK
{
	public class XMPTInternalState : XBaseInternalState
	{
		public XMPT MPT { get; set; }

		public XMPTInternalState(XMPT mpt)
		{
			MPT = mpt;
		}
	}
}