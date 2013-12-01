using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using FiresecAPI;
using FiresecAPI.XModels;

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
			PumpStations = new List<XPumpStation>();
			JournalFilters = new List<XJournalFilter>();
			Instructions = new List<XInstruction>();
			GuardUsers = new List<XGuardUser>();
			ParameterTemplates = new List<XParameterTemplate>();
		}

		public List<XDevice> Devices { get; set; }

		[DataMember]
		public XDevice RootDevice { get; set; }

		[DataMember]
		public List<XZone> Zones { get; set; }

		[DataMember]
		public List<XDirection> Directions { get; set; }

				[DataMember]
		public List<XPumpStation> PumpStations { get; set; }

		[DataMember]
		public List<XJournalFilter> JournalFilters { get; set; }

		[DataMember]
		public List<XInstruction> Instructions { get; set; }

		[DataMember]
		public List<XGuardUser> GuardUsers { get; set; }

		[DataMember]
		public List<XParameterTemplate> ParameterTemplates { get; set; }

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

		public List<XZone> SortedZones
		{
			get
			{
				return (
				from XZone zone in Zones
				orderby zone.No
				select zone).ToList();
			}
		}

		public void Reorder()
		{
			foreach (var device in Devices)
			{
				device.SynchronizeChildern();
			}
		}

		public override bool ValidateVersion()
		{
			bool result = true;

			if (RootDevice == null)
			{
				var device = new XDevice();
				device.DriverUID = new Guid("938947C5-4624-4A1A-939C-60AEEBF7B65C");
				RootDevice = device;
				result = false;
			}

			Update();

			if (PumpStations == null)
			{
				PumpStations = new List<XPumpStation>();
				result = false;
			}

			foreach (var journalFilter in JournalFilters)
			{
			}

			foreach (var device in Devices)
			{
				device.BaseUID = device.UID;
			}
			foreach (var zone in Zones)
			{
				zone.BaseUID = zone.UID;
			}

			foreach (var parameterTemplate in ParameterTemplates)
			{
				foreach (var deviceParameterTemplate in parameterTemplate.DeviceParameterTemplates)
				{
				}
			}
			foreach (var direction in Directions)
			{
				direction.BaseUID = direction.UID;
			}

			foreach (var journalFilter in JournalFilters)
			{
			}

			return result;
		}
	}
}