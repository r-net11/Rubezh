using System;
using Microsoft.Practices.Prism.Events;
using RubezhAPI.GK;

namespace Infrastructure.Events
{
	public class SelectGKMPTEvent : CompositePresentationEvent<SelectGKMPTEventArg>
	{
	}

	public class SelectGKMPTEventArg
	{
		public bool Cancel { get; set; }
		public GKMPT MPT { get; set; }
	}
}