using FiresecAPI.SKD;
using Microsoft.Practices.Prism.Events;

namespace Infrastructure.Events
{
	public class ShowSKDArchiveEvent : CompositePresentationEvent<ShowSKDArchiveEventArgs>
	{
	}

	public class ShowSKDArchiveEventArgs
	{
		public SKDDevice Device { get; set; }
	}
}