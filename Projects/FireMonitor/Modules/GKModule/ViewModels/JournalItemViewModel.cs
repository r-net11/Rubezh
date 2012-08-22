using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Infrastructure.Common.Windows.ViewModels;
using Common.GK;
using XFiresecAPI;
using FiresecClient;

namespace GKModule.ViewModels
{
	public class JournalItemViewModel : BaseViewModel
	{
		public JournalItem JournalItem { get; private set; }
		public XDevice Device { get; private set; }
		public XDeviceState DeviceState { get; private set; }
		public string DeviceAddress { get; private set; }

		public JournalItemViewModel(JournalItem journalItem)
		{
			JournalItem = journalItem;
			DeviceState = XManager.DeviceStates.DeviceStates.FirstOrDefault(x => x.Device.GetDatabaseNo(DatabaseType.Gk) == journalItem.GKObjectNo);
			if (DeviceState != null)
			{
				Device = DeviceState.Device;
				DeviceAddress = Device.Driver.ShortName + " " + Device.DottedAddress;
			}
		}
	}
}