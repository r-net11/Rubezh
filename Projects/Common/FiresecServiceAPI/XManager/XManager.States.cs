using System.Collections.Generic;
using XFiresecAPI;

namespace FiresecClient
{
    public partial class XManager
    {
        public static void CreateStates()
        {
            foreach (var device in Devices)
            {
				device.InternalState = new XDeviceState(device);
				device.State = new XState(device);
            }
            foreach (var zone in Zones)
            {
				zone.InternalState = new XZoneState(zone);
				zone.State = new XState(zone);
            }
			foreach (var direction in Directions)
			{
				direction.InternalState = new XDirectionState(direction);
				direction.State = new XState(direction);
			}
			foreach (var pumpStation in PumpStations)
			{
				pumpStation.InternalState = new XPumpStationState(pumpStation);
				pumpStation.State = new XState(pumpStation);
			}
        }

        static List<XDevice> allDeviceChildren;
        public static List<XDevice> GetAllDeviceChildren(XDevice device)
        {
            allDeviceChildren = new List<XDevice>();
            AddChildren(device);
			allDeviceChildren.RemoveAt(0);
            return allDeviceChildren;
        }
        public static void AddChildren(XDevice device)
        {
            allDeviceChildren.Add(device);
			if ((device.Children != null) && (device.Children.Count != 0))
			{
				foreach (var childDevice in device.Children)
				{
					AddChildren(childDevice);
				}
			}
        }

		public static List<XZoneState> GetAllGKZoneStates(XDeviceState gkDeviceState)
		{
			var zoneStates = new List<XZoneState>();
			foreach (var zone in Zones)
			{
				if (zone.GkDatabaseParent == gkDeviceState.Device)
				{
					zoneStates.Add(zone.InternalState);
				}
			}
			return zoneStates;
		}

		public static List<XDirectionState> GetAllGKDirectionStates(XDeviceState gkDeviceState)
		{
			var directionStates = new List<XDirectionState>();
			foreach (var direction in Directions)
			{
				if (direction.GkDatabaseParent == gkDeviceState.Device)
				{
					directionStates.Add(direction.InternalState);
				}
			}
			return directionStates;
		}

		public static XStateClass GetMinStateClass()
		{
			var minStateClass = XStateClass.No;
			foreach (var device in XManager.Devices)
			{
				var stateClass = device.State.StateClass;
				if (device.DriverType == XDriverType.AM1_T && stateClass == XStateClass.Fire2)
				{
					stateClass = XStateClass.Info;
				}
				if (stateClass < minStateClass)
					minStateClass = device.State.StateClass;
			}
			foreach (var zone in XManager.Zones)
			{
				if (zone.State.StateClass < minStateClass)
					minStateClass = zone.State.StateClass;
			}
			foreach (var direction in XManager.Directions)
			{
				if (direction.State.StateClass < minStateClass)
					minStateClass = direction.State.StateClass;
			}
			return minStateClass;
		}
    }
}