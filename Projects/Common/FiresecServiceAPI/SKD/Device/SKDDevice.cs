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
			SKDControllerProperty = new SKDControllerProperty();
		}

		public SKDDriver Driver { get; set; }
		public SKDDriverType DriverType
		{
			get { return Driver.DriverType; }
		}
		public SKDDevice Parent { get; set; }
		public SKDDeviceState State { get; set; }
		public SKDZone Zone { get; set; }
		public SKDZone OuterZone { get; set; }

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

		[DataMember]
		public Guid OuterZoneUID { get; set; }

		[DataMember]
		public SKDControllerProperty SKDControllerProperty { get; set; }

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

		public List<SKDDevice> AllParents
		{
			get
			{
				if (Parent == null)
					return new List<SKDDevice>();

				List<SKDDevice> allParents = Parent.AllParents;
				allParents.Add(Parent);
				return allParents;
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