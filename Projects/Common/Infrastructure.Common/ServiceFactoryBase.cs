using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Practices.Prism.Events;

namespace Infrastructure.Common
{
	public class ServiceFactoryBase
	{
		public static IEventAggregator Events { get; protected set; }
		public static ResourceService ResourceService { get; protected set; }
	}
}
