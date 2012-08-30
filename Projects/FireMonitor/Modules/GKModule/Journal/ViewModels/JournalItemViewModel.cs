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
using Infrastructure.Common;
using Infrastructure.Events;
using Infrastructure;

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
			ShowObjectCommand = new RelayCommand(OnShowObject, CanShowObject);
			JournalItem = journalItem;
			switch(JournalItem.JournalItemType)
			{
				case JournalItemType.Device:
					DeviceState = XManager.DeviceStates.DeviceStates.FirstOrDefault(x => x.UID == JournalItem.ObjectUID);
					PresentationName = "Устройство " + DeviceState.Device.Driver.ShortName + " " + DeviceState.Device.DottedAddress;
					break;

				case JournalItemType.Zone:
					ZoneState = XManager.DeviceStates.ZoneStates.FirstOrDefault(x => x.UID == JournalItem.ObjectUID);
					PresentationName = "Зона " + ZoneState.Zone.PresentationName;
					break;

				case JournalItemType.Direction:
					var direction = XManager.DeviceConfiguration.Directions.FirstOrDefault(x => x.UID == JournalItem.ObjectUID);
					PresentationName = "Направление " + direction.PresentationName;
					break;

				case JournalItemType.System:
					PresentationName = "Система";
					break;
			}

			var states = XStatesHelper.StatesFromInt(journalItem.ObjectState);
			var stringBuilder = new StringBuilder();
			foreach (var state in states)
			{
				stringBuilder.Append(state.ToDescription() + " ");
			}
			StringStates = stringBuilder.ToString();
		}

		public RelayCommand ShowObjectCommand { get; private set; }
		void OnShowObject()
		{
			switch (JournalItem.JournalItemType)
			{
				case JournalItemType.Device:
					ServiceFactory.Events.GetEvent<ShowXDeviceEvent>().Publish(JournalItem.ObjectUID);
					break;

				case JournalItemType.Zone:
					ServiceFactory.Events.GetEvent<ShowXZoneEvent>().Publish(JournalItem.ObjectUID);
					break;
			}
		}
		bool CanShowObject()
		{
			switch (JournalItem.JournalItemType)
			{
				case JournalItemType.Device:
				case JournalItemType.Zone:
					return true;
			}
			return false;
		}
	}
}