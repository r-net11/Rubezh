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
			AvailableStateBits = new List<XStateBit>();
			AvailableStateClasses = new List<XStateClass>();
			AutocreationItems = new List<SKDDriverAutocreationItem>();
			HasZone = false;
			IsControlDevice = false;
			IsPlaceable = false;
		}

		public SKDDriverType DriverType { get; set; }
		public int DriverTypeNo { get; set; }
		public Guid UID { get; set; }
		public string Name { get; set; }
		public string ShortName { get; set; }
		public List<XDriverState> XStates { get; set; }

		public List<XDriverProperty> Properties { get; set; }
		public List<XStateBit> AvailableStateBits { get; set; }
		public List<XStateClass> AvailableStateClasses { get; set; }

		public List<SKDDriverType> Children { get; set; }
		public List<SKDDriverAutocreationItem> AutocreationItems { get; set; }

		public bool HasZone { get; set; }
		public bool IsControlDevice { get; set; }
		public bool IsPlaceable { get; set; }

		public bool IsController
		{
			get
			{
				switch(DriverType)
				{
					case SKDDriverType.Controller:
					case SKDDriverType.ChinaController_1_2:
					case SKDDriverType.ChinaController_2_2:
					case SKDDriverType.ChinaController_2_4:
					case SKDDriverType.ChinaController_4_4:
						return true;
				}
				return false;
			}
		}

		public string ImageSource
		{
			get { return "/Controls;component/SKDIcons/" + this.DriverType.ToString() + ".png"; }
		}
	}

	public class SKDDriverAutocreationItem
	{
		public SKDDriverAutocreationItem(SKDDriverType driverType, int count)
		{
			DriverType = driverType;
			Count = count;
		}

		public SKDDriverType DriverType { get; set; }
		public int Count { get; set; }
	}
}