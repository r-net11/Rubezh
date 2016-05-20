using System;
using Microsoft.Practices.Prism.Events;
using RubezhAPI.GK;
using System.Collections.Generic;

namespace Infrastructure.Events
{
	public class SelectGKDevicesEvent : CompositePresentationEvent<SelectGKDevicesEventArg>
	{
	}

	public class SelectGKDevicesEventArg
	{
		public bool Cancel { get; set; }
		public List<GKDevice> Devices { get; set; }
	}
}