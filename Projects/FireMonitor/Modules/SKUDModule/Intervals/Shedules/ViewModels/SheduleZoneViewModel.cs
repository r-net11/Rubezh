using System;
using System.Collections.ObjectModel;
using System.Linq;
using FiresecAPI;
using Infrastructure.Common.TreeList;
using XFiresecAPI;

namespace SKDModule.ViewModels
{
	public class SheduleZoneViewModel : TreeNodeViewModel<SheduleZoneViewModel>
	{
		public SKDZone Zone { get; private set; }

		public SheduleZoneViewModel(SKDZone zone)
		{
			Zone = zone;
		}
	}
}