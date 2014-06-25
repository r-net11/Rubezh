using System;
using System.Collections.Generic;
using FiresecAPI.GK;

namespace FiresecAPI.SKD
{
	public class SKDDriver
	{
		public SKDDriver()
		{
			Children = new List<SKDDriverType>();
			Properties = new List<XDriverProperty>();
			MeasureParameters = new List<XMeasureParameter>();
			AutoCreateChildren = new List<SKDDriverType>();
			AvailableStateBits = new List<XStateBit>();
			AvailableStateClasses = new List<XStateClass>();
			CanEditAddress = true;
			HasAddress = true;
			IsPlaceable = false;
		}

		public SKDDriverType DriverType { get; set; }
		public ushort DriverTypeNo { get; set; }
		public Guid UID { get; set; }
		public string Name { get; set; }
		public string ShortName { get; set; }
		public string DeviceClassName { get; set; }
		public List<XDriverState> XStates { get; set; }

		public List<XDriverProperty> Properties { get; set; }
		public List<XStateBit> AvailableStateBits { get; set; }
		public List<XStateClass> AvailableStateClasses { get; set; }
		public List<XMeasureParameter> MeasureParameters { get; set; }

		public List<SKDDriverType> Children { get; set; }
		public List<SKDDriverType> AutoCreateChildren { get; set; }

		public bool HasAddress { get; set; }
		public bool CanEditAddress { get; set; }
		public bool IsRangeEnabled { get; set; }
		public bool IsAutoCreate { get; set; }
		public byte MinAddress { get; set; }
		public byte MaxAddress { get; set; }

		public int DoorsCount { get; set; }
		public int ReadersCount { get; set; }

		public bool HasZone { get; set; }
		public bool IsControlDevice { get; set; }
		public bool IsPlaceable { get; set; }

		public string ImageSource
		{
			get { return "/Controls;component/SKDIcons/" + this.DriverType.ToString() + ".png"; }
		}
	}
}