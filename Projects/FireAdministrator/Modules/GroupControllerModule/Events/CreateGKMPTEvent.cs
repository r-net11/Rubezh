using Microsoft.Practices.Prism.Events;
using RubezhAPI.GK;

namespace GKModule.Events
{
	public class CreateGKMPTEvent : CompositePresentationEvent<CreateGKMPTEventArg>
	{
	}

	public class CreateGKMPTEventArg
	{
		public bool Cancel { get; set; }
		public GKMPT MPT { get; set; }
	}
}