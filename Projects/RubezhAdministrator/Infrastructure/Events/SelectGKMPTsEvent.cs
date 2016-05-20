using System;
using Microsoft.Practices.Prism.Events;
using RubezhAPI.GK;
using System.Collections.Generic;

namespace Infrastructure.Events
{
	public class SelectGKMPTsEvent : CompositePresentationEvent<SelectGKMPTsEventArg>
	{
	}

	public class SelectGKMPTsEventArg
	{
		public bool Cancel { get; set; }
		public List<GKMPT> MPTs { get; set; }
	}
}