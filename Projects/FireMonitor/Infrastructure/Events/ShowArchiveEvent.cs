using Microsoft.Practices.Prism.Events;
using FiresecAPI.SKD;

namespace Infrastructure.Events
{
	public class ShowArchiveEvent : CompositePresentationEvent<ShowArchiveEventArgs>
	{
	}

	public class ShowArchiveEventArgs
	{
		public SKDDevice Device { get; set; }
		public SKDZone Zone { get; set; }
		public SKDDoor Door { get; set; }
	}
}