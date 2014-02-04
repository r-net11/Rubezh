using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Infrastructure.Common.Windows.ViewModels;
using Infrastructure.Common.TreeList;
using FiresecAPI;
using XFiresecAPI;

namespace SKDModule.ViewModels
{
	public class AccessZoneViewModel : TreeNodeViewModel<AccessZoneViewModel>
	{
		public SKDZone Zone { get; private set; }

		public AccessZoneViewModel(SKDZone zone)
		{
			Zone = zone;

			CanSelect = false;
			foreach (var device in zone.Devices)
			{
				if (device.DriverType == SKDDriverType.Controller)
					CanSelect = true;
			}
		}

		public bool CanSelect { get; private set; }

		bool _isChecked;
		public bool IsChecked
		{
			get { return _isChecked; }
			set
			{
				_isChecked = value;
				OnPropertyChanged("IsChecked");
			}
		}
	}
}