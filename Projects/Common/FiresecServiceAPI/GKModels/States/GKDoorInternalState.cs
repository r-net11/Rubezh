namespace FiresecAPI.GK
{
	public class GKDoorInternalState : GKBaseInternalState
	{
		public GKDoor Door { get; set; }

		public GKDoorInternalState(GKDoor door)
		{
			Door = door;
		}
	}
}