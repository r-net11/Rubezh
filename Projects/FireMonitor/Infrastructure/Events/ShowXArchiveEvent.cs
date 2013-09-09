using Microsoft.Practices.Prism.Events;
using XFiresecAPI;

namespace Infrastructure.Events
{
	public class ShowXArchiveEvent : CompositePresentationEvent<ShowXArchiveEventArgs>
    {
    }

	public class ShowXArchiveEventArgs
	{
		public XDevice Device { get; set; }
	}
}