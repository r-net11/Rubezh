using System;
using Microsoft.Practices.Prism.Events;
using RubezhAPI.GK;
using System.Collections.Generic;

namespace Infrastructure.Events
{
	public class SelectGKDoorsEvent : CompositePresentationEvent<SelectGKDoorsEventArg>
	{
	}

	public class SelectGKDoorsEventArg
	{
		public bool Cancel { get; set; }
		public List<GKDoor> Doors { get; set; }
	}
}