namespace FiresecAPI.GK
{
	public class XCodeInternalState : XBaseInternalState
	{
		public XCode Code { get; set; }

		public XCodeInternalState(XCode code)
		{
			Code = code;
		}
	}
}