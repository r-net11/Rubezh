namespace FiresecAPI.GK
{
	public class GKCodeInternalState : GKBaseInternalState
	{
		public GKCode Code { get; set; }

		public GKCodeInternalState(GKCode code)
		{
			Code = code;
		}
	}
}