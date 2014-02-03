using Microsoft.Practices.Prism.Events;
using XFiresecAPI;
using FiresecAPI;

namespace Infrastructure.Events
{
	public class ShowSKDArchiveEvent : CompositePresentationEvent<ShowSKDArchiveEventArgs>
    {
    }

	public class ShowSKDArchiveEventArgs
	{
		public SKDDevice Device { get; set; }
		public SKDZone Zone { get; set; }
	}
}