using System;
using Microsoft.Practices.Prism.Events;
using XFiresecAPI;

namespace Infrastructure.Events
{
	public class CreateXDirectionEvent : CompositePresentationEvent<CreateXDirectionEventArg>
	{
	}

	public class CreateXDirectionEventArg
	{
		public bool Cancel { get; set; }
        public Guid DirectionUID { get; set; }
		public XDirection XDirection { get; set; }
	}
}