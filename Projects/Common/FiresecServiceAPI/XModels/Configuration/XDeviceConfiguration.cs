using System.Collections.Generic;
using System.Runtime.Serialization;
using FiresecAPI.Models;
using FiresecAPI;
using System;

namespace XFiresecAPI
{
    [DataContract]
    public class XDeviceConfiguration : VersionedConfiguration
    {
        public XDeviceConfiguration()
        {
            Devices = new List<XDevice>();
            Zones = new List<XZone>();
            Directions = new List<XDirection>();
            JournalFilters = new List<XJournalFilter>();
        }

        public List<XDevice> Devices { get; set; }

        [DataMember]
        public XDevice RootDevice { get; set; }

        [DataMember]
        public List<XZone> Zones { get; set; }

        [DataMember]
        public List<XDirection> Directions { get; set; }

        [DataMember]
        public List<XJournalFilter> JournalFilters { get; set; }

        public void Update()
        {
            Devices = new List<XDevice>();
            if (RootDevice != null)
            {
                RootDevice.Parent = null;
                Devices.Add(RootDevice);
                AddChild(RootDevice);
            }
        }

        void AddChild(XDevice parentDevice)
        {
            foreach (var device in parentDevice.Children)
            {
                device.Parent = parentDevice;
                Devices.Add(device);
                AddChild(device);
            }
        }

        public override void ValidateVersion()
        {
            if (JournalFilters == null)
                JournalFilters = new List<XJournalFilter>();

			if (Devices == null)
				Devices = new List<XDevice>();
			if (Zones == null)
				Zones = new List<XZone>();
			if (Directions == null)
				Directions = new List<XDirection>();

			foreach (var device in Devices)
			{
				if (device.ZoneUIDs == null)
					device.ZoneUIDs = new List<Guid>();

				if (device.DeviceLogic == null)
					device.DeviceLogic = new XDeviceLogic();
				if (device.DeviceLogic.Clauses == null)
					device.DeviceLogic.Clauses = new List<XClause>();
				foreach (var clause in device.DeviceLogic.Clauses)
				{
					if (clause.DeviceUIDs == null)
						clause.DeviceUIDs = new List<Guid>();

					if (clause.ZoneUIDs == null)
						clause.ZoneUIDs = new List<Guid>();
				}
			}
			foreach (var zone in Zones)
			{
			}
			foreach (var direction in Directions)
			{
				if (direction.DeviceUIDs == null)
					direction.DeviceUIDs = new List<Guid>();

				if (direction.ZoneUIDs == null)
					direction.ZoneUIDs = new List<Guid>();
			}
        }
    }
}