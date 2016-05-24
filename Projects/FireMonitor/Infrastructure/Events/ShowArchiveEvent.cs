using Microsoft.Practices.Prism.Events;
using StrazhAPI.Integration.OPC;
using StrazhAPI.SKD;

namespace Infrastructure.Events
{
	public class ShowArchiveEvent : CompositePresentationEvent<ShowArchiveEventArgs>
	{
	}

	public class ShowArchiveEventArgs
	{
		public SKDDevice SKDDevice { get; set; }
		public SKDZone SKDZone { get; set; }
		public SKDDoor SKDDoor { get; set; }
		public OPCZone OPCZone { get; set; }
	}
}