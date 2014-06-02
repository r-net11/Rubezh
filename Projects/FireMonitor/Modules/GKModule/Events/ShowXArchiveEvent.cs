using FiresecAPI.GK;
using Microsoft.Practices.Prism.Events;

namespace GKModule.Events
{
	public class ShowXArchiveEvent : CompositePresentationEvent<ShowXArchiveEventArgs>
	{
	}

	public class ShowXArchiveEventArgs
	{
		public XDevice Device { get; set; }
		public XZone Zone { get; set; }
		public XGuardZone GuardZone { get; set; }
		public XDirection Direction { get; set; }
		public XPumpStation PumpStation { get; set; }
		public XMPT MPT { get; set; }
		public XDelay Delay { get; set; }
		public XPim Pim { get; set; }
	}
}