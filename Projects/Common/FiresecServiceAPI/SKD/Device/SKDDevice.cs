using System;
using System.Linq;
using System.Collections.Generic;
using System.Runtime.Serialization;
using FiresecAPI.GK;
using Common;
using Infrustructure.Plans.Interfaces;

namespace FiresecAPI.SKD
{
	[DataContract]
	public class SKDDevice : IStateProvider, IIdentity, IPlanPresentable
	{
		public SKDDevice()
		{
			UID = Guid.NewGuid();
			Children = new List<SKDDevice>();
			Properties = new List<SKDProperty>();
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
		public int IntAddress { get; set; }

		[DataMember]
		public string Name { get; set; }

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
		public Guid CameraUID { get; set; }

		public string Address
		{
			get
			{
				switch(DriverType)
				{
					case SKDDriverType.System:
					case SKDDriverType.Controller:
						case SKDDriverType.Gate:
						return "";

					case SKDDriverType.ChinaController_1_2:
					case SKDDriverType.ChinaController_2_2:
					case SKDDriverType.ChinaController_2_4:
					case SKDDriverType.ChinaController_4_4:
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
						return (IntAddress+1).ToString();

					default:
						return "";
				}
			}
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

		#region IStateProvider Members

		IDeviceState<XStateClass> IStateProvider.StateClass
		{
			get { return State; }
		}

		#endregion
	}
}