using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Infrastructure.Common.Windows.ViewModels;
using Common.GK;
using XFiresecAPI;
using FiresecClient;
using FiresecAPI;
using FiresecAPI.XModels;

namespace GKModule.ViewModels
{
	public class JournalItemViewModel : BaseViewModel
	{
		public JournalItem JournalItem { get; private set; }
		public XDeviceState DeviceState { get; private set; }
		public XZoneState ZoneState { get; private set; }
		public string PresentationName { get; private set; }
		public string StringStates { get; private set; }

		public JournalItemViewModel(JournalItem journalItem)
		{
			JournalItem = journalItem;
			DeviceState = XManager.DeviceStates.DeviceStates.FirstOrDefault(x => x.Device.GetDatabaseNo(DatabaseType.Gk) == journalItem.GKObjectNo);
			if (DeviceState != null)
			{
				var device = DeviceState.Device;
				PresentationName = "Устройство " + device.Driver.ShortName + " " + device.DottedAddress;
			}
			ZoneState = XManager.DeviceStates.ZoneStates.FirstOrDefault(x => x.Zone.GetDatabaseNo(DatabaseType.Gk) == journalItem.GKObjectNo);
			if (ZoneState != null)
			{
				PresentationName = "Зона " + ZoneState.Zone.PresentationName;
			}

			var states = XStatesHelper.StatesFromInt(journalItem.ObjectState);
			var stringBuilder = new StringBuilder();
			foreach (var state in states)
			{
				stringBuilder.Append(state.ToDescription() + " ");
			}
			StringStates = stringBuilder.ToString();
		}
	}
}