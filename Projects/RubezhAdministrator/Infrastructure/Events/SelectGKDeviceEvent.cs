using System;
using Microsoft.Practices.Prism.Events;
using RubezhAPI.GK;

namespace Infrastructure.Events
{
	public class SelectGKDeviceEvent : CompositePresentationEvent<SelectGKDeviceEventArg>
	{
	}

	public class SelectGKDeviceEventArg
	{
		public bool Cancel { get; set; }
		public GKDevice Device { get; set; }
	}
}