using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using XFiresecAPI;

namespace FiresecAPI
{
	[DataContract]
	public class SKDDevice
	{
		public SKDDevice()
		{
			UID = Guid.NewGuid();
			Children = new List<SKDDevice>();
			Properties = new List<XProperty>();
			DeviceProperties = new List<XProperty>();
			PlanElementUIDs = new List<Guid>();
			AllowMultipleVizualization = false;
		}

		public SKDDriver Driver { get; set; }
		public SKDDriverType DriverType
		{
			get { return Driver.DriverType; }
		}
		public SKDDevice Parent { get; set; }
		public SKDDeviceState State { get; set; }
		public SKDZone Zone { get; set; }

		[DataMember]
		public Guid UID { get; set; }

		[DataMember]
		public Guid DriverUID { get; set; }

		[DataMember]
		public string Address { get; set; }

		[DataMember]
		public string Description { get; set; }

		[DataMember]
		public List<SKDDevice> Children { get; set; }

		[DataMember]
		public List<XProperty> Properties { get; set; }

		[DataMember]
		public List<XProperty> DeviceProperties { get; set; }

		[DataMember]
		public List<Guid> PlanElementUIDs { get; set; }

		[DataMember]
		public bool AllowMultipleVizualization { get; set; }

		[DataMember]
		public Guid ZoneUID { get; set; }

		public bool CanBeNotUsed
		{
			get { return (Parent != null && Parent.Driver.IsGroupDevice); }
		}

		public string PresentationName
		{
			get { return Driver.ShortName + " " + Address; }
		}

		public bool IsRealDevice
		{
			get
			{
				if (DriverType == SKDDriverType.System)
					return false;
				return true;
			}
		}

		public void OnChanged()
		{
			if (Changed != null)
				Changed();
		}
		public event Action Changed;

		public void OnAUParametersChanged()
		{
			if (AUParametersChanged != null)
				AUParametersChanged();
		}
		public event Action AUParametersChanged;
	}
}