using System.Linq;
using System.Text;
using Common.GK;
using FiresecAPI;
using FiresecAPI.XModels;
using FiresecClient;
using Infrastructure;
using Infrastructure.Common;
using Infrastructure.Common.Windows.ViewModels;
using Infrastructure.Events;
using XFiresecAPI;

namespace GKModule.ViewModels
{
	public class JournalItemViewModel : BaseViewModel
	{
		public JournalItem JournalItem { get; private set; }
		public XDeviceState DeviceState { get; private set; }
		public XZoneState ZoneState { get; private set; }
		public XDirectionState DirectionState { get; private set; }
		public string PresentationName { get; private set; }
		public string StringStates { get; private set; }

		public JournalItemViewModel(JournalItem journalItem)
		{
			ShowObjectCommand = new RelayCommand(OnShowObject, CanShowObject);
			ShowOnPlanCommand = new RelayCommand(OnShowOnPlan, CanShowOnPlan);
			JournalItem = journalItem;
			switch (JournalItem.JournalItemType)
			{
				case JournalItemType.Device:
					var device = XManager.DeviceConfiguration.Devices.FirstOrDefault(x => x.UID == JournalItem.ObjectUID);
					if (device != null)
					{
						DeviceState = device.DeviceState;
						PresentationName = device.Driver.ShortName + " " + device.DottedAddress;
					}
					break;

				case JournalItemType.Zone:
					var zone = XManager.DeviceConfiguration.Zones.FirstOrDefault(x => x.UID == JournalItem.ObjectUID);
					if (zone != null)
					{
						ZoneState = zone.ZoneState;
						PresentationName = zone.PresentationName;
					}
					break;

				case JournalItemType.Direction:
					var direction = XManager.DeviceConfiguration.Directions.FirstOrDefault(x => x.UID == JournalItem.ObjectUID);
					if (direction != null)
					{
						DirectionState = direction.DirectionState;
						PresentationName = direction.PresentationName;
					}
					break;

				case JournalItemType.GK:
					var gkDevice = XManager.DeviceConfiguration.Devices.FirstOrDefault(x => x.UID == JournalItem.ObjectUID);
					if (gkDevice != null)
					{
						DeviceState = gkDevice.DeviceState;
						PresentationName = DeviceState.Device.ShortNameAndDottedAddress;
					}
					break;

				case JournalItemType.System:
					break;
			}

			var states = XStatesHelper.StatesFromInt(journalItem.ObjectState);
			var stringBuilder = new StringBuilder();
			foreach (var state in states)
			{
				if (state == XStateType.Save)
					continue;

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

				case JournalItemType.Direction:
					ServiceFactory.Events.GetEvent<ShowXDirectionEvent>().Publish(JournalItem.ObjectUID);
					break;

				case JournalItemType.GK:
					ServiceFactory.Events.GetEvent<ShowXDeviceEvent>().Publish(JournalItem.ObjectUID);
					break;
			}
		}
		bool CanShowObject()
		{
			switch (JournalItem.JournalItemType)
			{
				case JournalItemType.Device:
				case JournalItemType.Zone:
				case JournalItemType.Direction:
				case JournalItemType.GK:
					return true;
			}
			return false;
		}

		public RelayCommand ShowOnPlanCommand { get; private set; }
		void OnShowOnPlan()
		{
			switch (JournalItem.JournalItemType)
			{
				case JournalItemType.Device:
					if (DeviceState != null)
					{
						ShowOnPlanHelper.ShowDevice(DeviceState.Device);
					}
					break;
				case JournalItemType.Zone:
					if (ZoneState != null)
					{
						ShowOnPlanHelper.ShowZone(ZoneState.Zone);
					}
					break;
			}
		}
		bool CanShowOnPlan()
		{
			switch (JournalItem.JournalItemType)
			{
				case JournalItemType.Device:
					if (DeviceState != null)
					{
						return ShowOnPlanHelper.CanShowDevice(DeviceState.Device);
					}
					break;
				case JournalItemType.Zone:
					if (ZoneState != null)
					{
						return ShowOnPlanHelper.CanShowZone(ZoneState.Zone);
					}
					break;
			}
			return false;
		}
	}
}