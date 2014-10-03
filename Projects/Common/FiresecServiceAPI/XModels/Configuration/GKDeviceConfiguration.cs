using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace FiresecAPI.GK
{
	[DataContract]
	public partial class GKDeviceConfiguration : VersionedConfiguration
	{
		public GKDeviceConfiguration()
		{
			Devices = new List<GKDevice>();
			Zones = new List<GKZone>();
			Directions = new List<GKDirection>();
			PumpStations = new List<GKPumpStation>();
			MPTs = new List<GKMPT>();
			JournalFilters = new List<GKJournalFilter>();
			Instructions = new List<GKInstruction>();
			Codes = new List<GKCode>();
			GuardZones = new List<GKGuardZone>();
			Schedules = new List<GKSchedule>();
			ParameterTemplates = new List<GKParameterTemplate>();
		}

		[XmlIgnore]
		public List<GKDevice> Devices { get; set; }

		[DataMember]
		public GKDevice RootDevice { get; set; }

		[DataMember]
		public List<GKZone> Zones { get; set; }

		[DataMember]
		public List<GKDirection> Directions { get; set; }

		[DataMember]
		public List<GKPumpStation> PumpStations { get; set; }

		[DataMember]
		public List<GKMPT> MPTs { get; set; }

		[DataMember]
		public List<GKDelay> Delays { get; set; }

		[DataMember]
		public List<GKJournalFilter> JournalFilters { get; set; }

		[DataMember]
		public List<GKInstruction> Instructions { get; set; }

		[DataMember]
		public List<GKCode> Codes { get; set; }

		[DataMember]
		public List<GKGuardZone> GuardZones { get; set; }

		[DataMember]
		public List<GKDoor> Doors { get; set; }

		[DataMember]
		public List<GKSchedule> Schedules { get; set; }

		[DataMember]
		public List<GKParameterTemplate> ParameterTemplates { get; set; }

		public void Update()
		{
			Devices = new List<GKDevice>();
			if (RootDevice != null)
			{
				RootDevice.Parent = null;
				Devices.Add(RootDevice);
				AddChild(RootDevice);
			}
		}

		void AddChild(GKDevice parentDevice)
		{
			foreach (var device in parentDevice.Children)
			{
				device.Parent = parentDevice;
				Devices.Add(device);
				AddChild(device);
			}
		}

		[XmlIgnore]
		public List<GKZone> SortedZones
		{
			get
			{
				return (
				from GKZone zone in Zones
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
				var device = new GKDevice();
				device.DriverUID = new Guid("938947C5-4624-4A1A-939C-60AEEBF7B65C");
				RootDevice = device;
				result = false;
			}

			Update();

			if (Delays == null)
			{
				Delays = new List<GKDelay>();
				result = false;
			}

			if (PumpStations == null)
			{
				PumpStations = new List<GKPumpStation>();
				result = false;
			}

			if (MPTs == null)
			{
				MPTs = new List<GKMPT>();
				result = false;
			}

			foreach (var delay in Delays)
			{
				result &= ValidateDeviceLogic(delay.DeviceLogic);
			}
			foreach (var mpt in MPTs)
			{
				result &= ValidateDeviceLogic(mpt.StartLogic);
				foreach (var mptDevice in mpt.MPTDevices)
				{
				}
			}
			foreach (var device in Devices)
			{
				device.UID = device.UID;
				result &= ValidateDeviceLogic(device.DeviceLogic);
				result &= ValidateDeviceLogic(device.NSLogic);
			}
			foreach (var zone in Zones)
			{
				zone.UID = zone.UID;
			}
			foreach (var direction in Directions)
			{
				direction.UID = direction.UID;
			}
			foreach (var pumpStation in PumpStations)
			{
				pumpStation.UID = pumpStation.UID;
				result &= ValidateDeviceLogic(pumpStation.StartLogic);
				result &= ValidateDeviceLogic(pumpStation.StopLogic);
				result &= ValidateDeviceLogic(pumpStation.AutomaticOffLogic);
			}
			foreach (var parameterTemplate in ParameterTemplates)
			{
				foreach (var deviceParameterTemplate in parameterTemplate.DeviceParameterTemplates)
				{
					deviceParameterTemplate.GKDevice.UID = deviceParameterTemplate.GKDevice.UID;
					result &= ValidateDeviceLogic(deviceParameterTemplate.GKDevice.DeviceLogic);
					result &= ValidateDeviceLogic(deviceParameterTemplate.GKDevice.NSLogic);
				}
			}

			if (GuardZones == null)
			{
				GuardZones = new List<GKGuardZone>();
				result = false;
			}
			foreach (var guardZone in GuardZones)
			{
				if (guardZone.GuardZoneDevices == null)
				{
					guardZone.GuardZoneDevices = new List<GKGuardZoneDevice>();
					result = false;
				}
			}

			if (Codes == null)
			{
				Codes = new List<GKCode>();
				result = false;
			}

			if (Doors == null)
			{
				Doors = new List<GKDoor>();
				result = false;
			}

			if (Schedules == null)
			{
				Schedules = new List<GKSchedule>();
				result = false;
			}

			return result;
		}

		bool ValidateDeviceLogic(GKDeviceLogic deviceLogic)
		{
			var result = true;

			if (deviceLogic.ClausesGroup == null)
			{
				deviceLogic.ClausesGroup = new GKClauseGroup();
				result = false;
			}
			if (deviceLogic.OffClausesGroup == null)
			{
				deviceLogic.OffClausesGroup = new GKClauseGroup();
				result = false;
			}
			return result;
		}
	}
}