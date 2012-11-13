using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace FiresecAPI.Models
{
	[DataContract]
	public class Zone
	{
		public Zone()
		{
			UID = Guid.NewGuid();
			ZoneType = ZoneType.Fire;
			GuardZoneType = GuardZoneType.Ordinary;
			DetectorCount = 2;
			EvacuationTime = "0";
			AutoSet = "0";
			Delay = "0";

			ShapeIds = new List<string>();
			DevicesInZone = new List<Device>();
			DevicesInZoneLogic = new List<Device>();
			IndicatorsInZone = new List<Device>();
		}

		public ZoneState ZoneState { get; set; }
		public Guid SecPanelUID { get; set; }

		List<Device> _devicesInZone;
		public List<Device> DevicesInZone
		{
			get
			{
				if (_devicesInZone == null)
					_devicesInZone = new List<Device>();
				return _devicesInZone;
			}
			set { _devicesInZone = value; }
		}

		List<Device> _devicesInZoneLogic;
		public List<Device> DevicesInZoneLogic
		{
			get
			{
				if (_devicesInZoneLogic == null)
					_devicesInZoneLogic = new List<Device>();
				return _devicesInZoneLogic;
			}
			set { _devicesInZoneLogic = value; }
		}

		List<Device> _indicatorsInZone;
		public List<Device> IndicatorsInZone
		{
			get
			{
				if (_indicatorsInZone == null)
					_indicatorsInZone = new List<Device>();
				return _indicatorsInZone;
			}
			set { _indicatorsInZone = value; }
		}

		[DataMember]
		public Guid UID { get; set; }

		[DataMember]
		public int No { get; set; }

		[DataMember]
		public string Name { get; set; }

		[DataMember]
		public string Description { get; set; }

		[DataMember]
		public ZoneType ZoneType { get; set; }

		[DataMember]
		public int DetectorCount { get; set; }

		[DataMember]
		public string EvacuationTime { get; set; }

		[DataMember]
		public string AutoSet { get; set; }

		[DataMember]
		public string Delay { get; set; }

		[DataMember]
		public bool Skipped { get; set; }

		[DataMember]
		public GuardZoneType GuardZoneType { get; set; }

		[DataMember]
		public bool IsOPCUsed { get; set; }

		[DataMember]
		public List<string> ShapeIds { get; set; }

		public string PresentationName
		{
			get
			{
				var result = No + "." + Name;
				if (!string.IsNullOrEmpty(Description))
				{
					result += "(" + Description + ")";
				}
				return result;
			}
		}

		public void UpdateExternalDevices()
		{
			foreach (var device in DevicesInZoneLogic)
			{
				device.UpdateHasExternalDevices();
			}
		}

		public Zone Clone()
		{
			var zoneCopy = new Zone()
			{
				UID = UID,
				No = No,
				Name = Name,
				Description = Description,
				ZoneType = ZoneType,
				DetectorCount = DetectorCount,
				EvacuationTime = EvacuationTime,
				AutoSet = AutoSet,
				Delay = Delay,
				Skipped = Skipped,
				GuardZoneType = GuardZoneType,
			};
			return zoneCopy;
		}

		public void OnChanged()
		{
			if (Changed != null)
				Changed();
		}
		public event Action Changed;

		public void OnColorTypeChanged()
		{
			if (ColorTypeChanged != null)
				ColorTypeChanged();
		}
		public event Action ColorTypeChanged;
	}
}