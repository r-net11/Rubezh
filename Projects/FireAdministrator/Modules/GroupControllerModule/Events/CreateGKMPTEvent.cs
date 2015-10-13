using System;
using RubezhAPI.GK;
using Microsoft.Practices.Prism.Events;

namespace GKModule.Events
{
	public class CreateGKMPTEvent : CompositePresentationEvent<CreateGKMPTEventArg>
	{
	}

	public class CreateGKMPTEventArg
	{
		public bool Cancel { get; set; }
		public Guid MPTUID { get; set; }
		public GKMPT MPT { get; set; }
	}
}