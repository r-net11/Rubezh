using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ChinaSKDDriverAPI;
using Infrastructure.Common.Windows.ViewModels;

namespace ControllerSDK.ViewModels
{
	public class AlarmLogItemViewModel : BaseViewModel
	{
		public AlarmLogItem AlarmLogItem { get; set; }

		public AlarmLogItemViewModel(AlarmLogItem alarmLogItem)
		{
			AlarmLogItem = alarmLogItem;
		}
	}
}
