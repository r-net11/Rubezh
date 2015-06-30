namespace FiresecAPI.GK
{
	public class GKDirectionInternalState : GKBaseInternalState
	{
		public GKDirection Direction { get; set; }

		public GKDirectionInternalState(GKDirection direction)
		{
			Direction = direction;
		}
	}
}