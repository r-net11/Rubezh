using Microsoft.Practices.Prism.Events;
using FiresecAPI.SKD;
using FiresecAPI.GK;

namespace Infrastructure.Events
{
	public class ShowArchiveEvent : CompositePresentationEvent<ShowArchiveEventArgs>
	{
	}

	public class ShowArchiveEventArgs
	{
		public GKDevice GKDevice { get; set; }
		public SKDDevice SKDDevice { get; set; }
		public SKDZone SKDZone { get; set; }
		public SKDDoor SKDDoor { get; set; }
	}
}