using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using FiresecAPI.GK;

namespace FiresecAPI.SKD
{
	[DataContract]
	public class SKDDevice
	{
		public SKDDevice()
		{
			UID = Guid.NewGuid();
			Children = new List<SKDDevice>();
			Properties = new List<XProperty>();
			PlanElementUIDs = new List<Guid>();
			AllowMultipleVizualization = false;
			SKDReaderProperty = new SKDReaderProperty();
		}

		public SKDDriver Driver { get; set; }
		public SKDDriverType DriverType
		{
			get { return Driver.DriverType; }
		}
		public SKDDevice Parent { get; set; }
		public SKDDeviceState State { get; set; }
		public SKDZone Zone { get; set; }
		public int IntAddress { get; set; }

		[DataMember]
		public Guid UID { get; set; }

		[DataMember]
		public Guid DriverUID { get; set; }

		[DataMember]
		public string Address { get; set; }

		[DataMember]
		public string Name { get; set; }

		[DataMember]
		public List<SKDDevice> Children { get; set; }

		[DataMember]
		public List<XProperty> Properties { get; set; }

		[DataMember]
		public List<Guid> PlanElementUIDs { get; set; }

		[DataMember]
		public bool AllowMultipleVizualization { get; set; }

		[DataMember]
		public Guid ZoneUID { get; set; }

		[DataMember]
		public SKDReaderProperty SKDReaderProperty { get; set; }

		[DataMember]
		public Guid CameraUID { get; set; }

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
	}
}