using System.Linq;
using System.Text;
using GKProcessor;
using FiresecAPI;
using FiresecAPI.XModels;
using FiresecClient;
using Infrastructure;
using Infrastructure.Common;
using Infrastructure.Common.Windows.ViewModels;
using Infrastructure.Events;
using XFiresecAPI;
using System.Diagnostics;
using System;
using Common;

namespace GKModule.ViewModels
{
	public class JournalItemViewModel : BaseViewModel
	{
		public JournalItem JournalItem { get; private set; }
		public XDeviceState DeviceState { get; private set; }
		public XZoneState ZoneState { get; private set; }
		public XDirectionState DirectionState { get; private set; }
		public XDelayState DelayState { get; private set; }
		public string PresentationName { get; private set; }
		public string StringStates { get; private set; }
		
		public JournalItemViewModel(JournalItem journalItem)
		{
			ShowObjectOrPlanCommand = new RelayCommand(OnShowObjectOrPlan);
			ShowObjectCommand = new RelayCommand(OnShowObject, CanShowInTree);
			ShowOnPlanCommand = new RelayCommand(OnShowOnPlan, CanShowOnPlan);
			JournalItem = journalItem;
			IsExistsInConfig = true;
			
			try
			{
				switch (JournalItem.JournalItemType)
				{
					case JournalItemType.Device:
						var device = XManager.Devices.FirstOrDefault(x => x.UID == JournalItem.ObjectUID);
						if (device != null)
						{
							DeviceState = device.DeviceState;
							PresentationName = device.ShortName + " " + device.DottedAddress;
						}
						break;

					case JournalItemType.Zone:
						var zone = XManager.Zones.FirstOrDefault(x => x.UID == JournalItem.ObjectUID);
						if (zone != null)
						{
							ZoneState = zone.ZoneState;
							PresentationName = zone.PresentationName;
						}
						break;

					case JournalItemType.Direction:
						var direction = XManager.Directions.FirstOrDefault(x => x.UID == JournalItem.ObjectUID);
						if (direction != null)
						{
							DirectionState = direction.DirectionState;
							PresentationName = direction.PresentationName;
						}
						break;

					case JournalItemType.Delay:
						XDelay delay = null;
						foreach (var gkDatabase in DescriptorsManager.GkDatabases)
						{
							delay = gkDatabase.Delays.FirstOrDefault(x => x.Name == JournalItem.ObjectName);
							if (delay != null)
								break;
						}
						if (delay != null)
						{
							DelayState = delay.DelayState;
							PresentationName = delay.Name;
						}
						break;

					case JournalItemType.GK:
						var gkDevice = XManager.Devices.FirstOrDefault(x => x.UID == JournalItem.ObjectUID);
						if (gkDevice != null)
						{
							DeviceState = gkDevice.DeviceState;
							PresentationName = DeviceState.Device.ShortNameAndDottedAddress;
						}
						break;

					case JournalItemType.GkUser:
						PresentationName = JournalItem.UserName;
						break;

					case JournalItemType.System:
						//PresentationName = JournalItem.UserName;
						break;
				}

				if (PresentationName == null)
				{
					PresentationName = JournalItem.ObjectName;
					IsExistsInConfig = false;
				}

				if(PresentationName == null)
					PresentationName = "<Нет в конфигурации>";

				var states = XStatesHelper.StatesFromInt(journalItem.ObjectState);
				var stringBuilder = new StringBuilder();
				foreach (var state in states)
				{
					if (state != XStateBit.Save)
					{
						stringBuilder.Append(state.ToDescription() + " ");
					}
				}
				StringStates = stringBuilder.ToString();
			}
			catch (Exception e)
			{
				Logger.Error(e, "JournalItemViewModel ctr");
			}
		}

		public bool CanShow
		{
			get
			{
				return CanShowInTree() || CanShowOnPlan();
			}
		}

		//public string ImageSource
		//{
		//    get
		//    {
		//        switch (JournalItem.JournalItemType)
		//        {
		//            case JournalItemType.Device:
		//                var device = XManager.Devices.FirstOrDefault(x => x.UID == JournalItem.ObjectUID);
		//                return device == null ? "/Controls;component/StateClassIcons/Off.png" : device.Driver.ImageSource;
		//            case JournalItemType.Zone:
		//                return "/Controls;component/Images/zone.png";
		//            case JournalItemType.Direction:
		//                return "/Controls;component/Images/Blue_Direction.png";
		//            case JournalItemType.Delay:
		//                return "/Controls;component/Images/Delay.png";
		//            case JournalItemType.GK:
		//                return "/Controls;component/GKIcons/GK.png";
		//            case JournalItemType.User:
		//                return "/Controls;component/Images/Chip.png";
		//            case JournalItemType.System:
		//                return "/Controls;component/Images/PC.png";
		//            default:
		//                return "/Controls;component/StateClassIcons/Off.png";
		//        }
		//    }
		//}

		public RelayCommand ShowObjectOrPlanCommand { get; private set; }
		void OnShowObjectOrPlan()
		{
			if (CanShowOnPlan())
				OnShowOnPlan();
			else if (CanShowInTree())
				OnShowObject();
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

				case JournalItemType.Delay:
					ServiceFactory.Events.GetEvent<ShowXDelayEvent>().Publish(JournalItem.ObjectUID);
					break;

				case JournalItemType.GK:
					ServiceFactory.Events.GetEvent<ShowXDeviceEvent>().Publish(JournalItem.ObjectUID);
					break;
			}
		}

		public bool IsExistsInConfig { get; private set; }

		bool CanShowInTree()
		{
			if (!IsExistsInConfig)
				return false;

			switch (JournalItem.JournalItemType)
			{
				case JournalItemType.Device:
				case JournalItemType.Zone:
				case JournalItemType.Direction:
				case JournalItemType.Delay:
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
			if (!IsExistsInConfig)
				return false;
			
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

		public bool ShowAdditionalProperties
		{
			get
			{
#if DEBUG
				return true;
#endif
				return false;
			}
		}
	}
}