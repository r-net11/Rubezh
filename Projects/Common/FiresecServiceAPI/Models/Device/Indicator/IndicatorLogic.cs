using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace FiresecAPI.Models
{
	[DataContract]
	public class IndicatorLogic
	{
		public IndicatorLogic()
		{
			ZoneUIDs = new List<Guid>();
			Zones = new List<Zone>();
		}

		[XmlIgnore]
		public Device Device { get; set; }
		[XmlIgnore]
		public List<Zone> Zones { get; set; }

		[DataMember]
		public IndicatorLogicType IndicatorLogicType { get; set; }

		[DataMember]
		public List<Guid> ZoneUIDs { get; set; }

		[DataMember]
		public Guid DeviceUID { get; set; }

		[DataMember]
		public IndicatorColorType OnColor { get; set; }

		[DataMember]
		public IndicatorColorType OffColor { get; set; }

		[DataMember]
		public IndicatorColorType FailureColor { get; set; }

		[DataMember]
		public IndicatorColorType ConnectionColor { get; set; }

		public override string ToString()
		{
			switch (IndicatorLogicType)
			{
				case IndicatorLogicType.Device:
					{
						if (DeviceUID != Guid.Empty)
						{
							var deviceString = "Устр: ";
							deviceString += Device.DottedPresentationNameAndAddress;
							if (Device.Driver.DriverType == DriverType.Indicator)
							{
								if (Device.IndicatorLogic.Device != null)
								{
									deviceString += "(" + Device.IndicatorLogic.Device.DottedPresentationNameAndAddress + ")";
								}
							}
							return deviceString;
						}
						break;
					}
				case IndicatorLogicType.Zone:
					{
						if ((Zones != null) && (Zones.Count > 0))
						{
							var zonesString = "Зоны: ";

							for (int i = 0; i < Zones.Count; i++)
							{
								if (i > 0)
									zonesString += ",";
								zonesString += Zones[i].PresentationName;
							}

							return zonesString;
						}
						break;
					}
			}
			return "";
		}
	}
}