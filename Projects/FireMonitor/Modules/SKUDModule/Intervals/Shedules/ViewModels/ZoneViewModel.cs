using System;
using System.Collections.ObjectModel;
using System.Linq;
using FiresecAPI;
using Infrastructure.Common.TreeList;
using XFiresecAPI;

namespace SKDModule.Intervals.Schedules.ViewModels
{
	public class ZoneViewModel : TreeNodeViewModel<ZoneViewModel>
	{
		public SKDZone Zone { get; private set; }

		public ZoneViewModel(SKDZone zone)
		{
			Zone = zone;
		}
	}
}