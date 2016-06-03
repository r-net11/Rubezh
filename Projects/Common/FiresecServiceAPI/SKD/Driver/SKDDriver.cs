using System;
using System.Collections.Generic;
using StrazhAPI.GK;

namespace StrazhAPI.SKD
{
	public class SKDDriver
	{
		public SKDDriver()
		{
			Children = new List<SKDDriverType>();
			Properties = new List<SKDDriverProperty>();
			AvailableStateClasses = new List<XStateClass>();
			AutocreationItems = new List<SKDDriverAutocreationItem>();
			HasZone = false;
			IsPlaceable = false;
		}

		public SKDDriverType DriverType { get; set; }

		public int DriverTypeNo { get; set; }

		public Guid UID { get; set; }

		public string Name { get; set; }

		public string ShortName { get; set; }

		public List<SKDDriverProperty> Properties { get; set; }

		public List<XStateClass> AvailableStateClasses { get; set; }

		public List<SKDDriverType> Children { get; set; }

		public List<SKDDriverAutocreationItem> AutocreationItems { get; set; }

		public bool HasZone { get; set; }

		public bool IsPlaceable { get; set; }

		public bool IsController
		{
			get
			{
				switch (DriverType)
				{
					case SKDDriverType.Controller:
					case SKDDriverType.ChinaController_1:
					case SKDDriverType.ChinaController_2:
					case SKDDriverType.ChinaController_4:
						return true;
				}
				return false;
			}
		}

		public string ImageSource
		{
			get { return "/Controls;component/SKDIcons/" + DriverType + ".png"; }
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