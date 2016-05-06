using Common;
using StrazhAPI.SKD.Device;
using Infrustructure.Plans.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Xml.Serialization;
using StrazhAPI.GK;

namespace StrazhAPI.SKD
{
	[DataContract]
	public class SKDDevice : ModelBase, IStateProvider, IPlanPresentable
	{
		public SKDDevice()
		{
			Children = new List<SKDDevice>();
			Properties = new List<SKDProperty>();
			PlanElementUIDs = new List<Guid>();
			AllowMultipleVizualization = false;
			DoorType = DoorType.TwoWay;
		}

		[XmlIgnore]
		public SKDDriver Driver { get; set; }

		[XmlIgnore]
		public SKDDriverType DriverType
		{
			get { return Driver.DriverType; }
		}

		[XmlIgnore]
		public SKDDevice Parent { get; set; }

		[XmlIgnore]
		public SKDDeviceState State { get; set; }

		[XmlIgnore]
		public SKDZone Zone { get; set; }

		[XmlIgnore]
		public SKDDoor Door { get; set; }

		[DataMember]
		public Guid DriverUID { get; set; }

		[DataMember]
		public int IntAddress { get; set; }

		[DataMember]
		public List<SKDDevice> Children { get; set; }

		[DataMember]
		public List<SKDProperty> Properties { get; set; }

		[DataMember]
		public List<Guid> PlanElementUIDs { get; set; }

		[DataMember]
		public bool AllowMultipleVizualization { get; set; }

		[DataMember]
		public Guid ZoneUID { get; set; }

		[DataMember]
		public SKDDoorConfiguration SKDDoorConfiguration { get; set; }

		[DataMember]
		public DoorType DoorType { get; set; }

		[DataMember]
		public Guid CameraUID { get; set; }

		[XmlIgnore]
		public string Address
		{
			get
			{
				switch (DriverType)
				{
					case SKDDriverType.System:
					case SKDDriverType.Controller:
						return "";

					case SKDDriverType.ChinaController_1:
					case SKDDriverType.ChinaController_2:
					case SKDDriverType.ChinaController_4:
						var property = Properties.FirstOrDefault(x => x.Name == "Address");
						if (property != null)
						{
							return property.StringValue;
						}
						return "";

					case SKDDriverType.Reader:
					case SKDDriverType.Lock:
					case SKDDriverType.LockControl:
					case SKDDriverType.Button:
						return (IntAddress + 1).ToString();

					default:
						return "";
				}
			}
		}

		[XmlIgnore]
		public bool IsRealDevice
		{
			get
			{
				if (DriverType == SKDDriverType.System)
					return false;
				return true;
			}
		}

		[XmlIgnore]
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

		[XmlIgnore]
		public string NameWithParent
		{
			get
			{
				var result = Name;
				if (Parent != null && Parent.Name != null)
				{
					result += " (" + Parent.Name + ")";
				}
				return result;
			}
		}

		[XmlIgnore]
		public bool IsEnabled
		{
			get
			{
				var isEnabled = true;
				if (Parent != null)
				{
					switch (Parent.DoorType)
					{
						case DoorType.OneWay:
							if (Parent.DriverType == SKDDriverType.ChinaController_1)
							{
								if (DriverType == SKDDriverType.Reader)
									if (IntAddress > 0)
										isEnabled = false;
							}
							break;
						case DoorType.TwoWay:
							if (DriverType == SKDDriverType.Button)
							{
								isEnabled = false;
							}
							if (Parent.DriverType == SKDDriverType.ChinaController_2)
							{
								if (DriverType == SKDDriverType.Lock || DriverType == SKDDriverType.LockControl)
									if (IntAddress > 0)
										isEnabled = false;
							}
							if (Parent.DriverType == SKDDriverType.ChinaController_4)
							{
								if (DriverType == SKDDriverType.Lock || DriverType == SKDDriverType.LockControl)
									if (IntAddress > 1)
										isEnabled = false;
							}
							break;
					}
				}
				return isEnabled;
			}
		}

		[DataMember]
		public SKDAntiPassBackConfiguration AntiPassBackConfiguration { get; set; }

		[DataMember]
		public SKDInterlockConfiguration InterlockConfiguration { get; set; }

		[DataMember]
		public SKDControllerPasswords ControllerPasswords { get; set; }

		#region IStateProvider Members

		IDeviceState IStateProvider.StateClass
		{
			get { return State; }
		}

		#endregion IStateProvider Members
	}
}