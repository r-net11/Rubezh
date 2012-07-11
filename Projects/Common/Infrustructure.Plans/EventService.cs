using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Practices.Prism.Events;

namespace Infrustructure.Plans
{
	public static class EventService
	{
		private static IEventAggregator _eventAggregator = null;

		public static void RegisterEventAggregator(IEventAggregator eventAggregator)
		{
			_eventAggregator = eventAggregator;
		}
		public static IEventAggregator EventAggregator
		{
			get { return _eventAggregator; }
		}
	}
}
