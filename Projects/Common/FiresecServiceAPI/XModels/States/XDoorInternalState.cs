namespace FiresecAPI.GK
{
	public class XDoorInternalState : XBaseInternalState
	{
		public XDoor Door { get; set; }

		public XDoorInternalState(XDoor door)
		{
			Door = door;
		}
	}
}