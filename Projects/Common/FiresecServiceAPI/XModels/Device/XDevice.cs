using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace XFiresecAPI
{
	[DataContract]
	public class XDevice : XBinaryBase
	{
		public XDevice()
		{
			UID = Guid.NewGuid();
			Children = new List<XDevice>();
			Properties = new List<XProperty>();
			ZoneUIDs = new List<Guid>();
			DeviceLogic = new XDeviceLogic();
			PlanElementUIDs = new List<Guid>();
			IsNotUsed = false;

			Zones = new List<XZone>();
			Directions = new List<XDirection>();
			DevicesInLogic = new List<XDevice>();
		}

		public XDeviceState DeviceState { get; set; }
		public override XBaseState GetXBaseState() { return DeviceState; }
		public XDriver Driver { get; set; }
		public XDevice Parent { get; set; }
		public List<XZone> Zones { get; set; }
		public List<XDirection> Directions { get; set; }
		public List<XDevice> DevicesInLogic { get; set; }

		[DataMember]
		public Guid UID { get; set; }

		[DataMember]
		public Guid DriverUID { get; set; }

		[DataMember]
		public byte ShleifNo { get; set; }

		[DataMember]
		public byte IntAddress { get; set; }

		[DataMember]
		public string Description { get; set; }

		[DataMember]
		public string PredefinedName { get; set; }

		[DataMember]
		public List<XDevice> Children { get; set; }

		[DataMember]
		public List<XProperty> Properties { get; set; }

		[DataMember]
		public List<Guid> ZoneUIDs { get; set; }

		[DataMember]
		public XDeviceLogic DeviceLogic { get; set; }

		[DataMember]
		public bool IsNotUsed { get; set; }

		[DataMember]
		public List<Guid> PlanElementUIDs { get; set; }

		public string Address
		{
			get
			{
				if (Driver.DriverType == XDriverType.GK)
				{
					var ipAddress = GetGKIpAddress();
					return ipAddress != null ? ipAddress : "";
				}
				if (Driver.HasAddress == false)
					return "";

				if (Driver.IsDeviceOnShleif == false)
					return IntAddress.ToString();

				return ShleifNo.ToString() + "." + IntAddress.ToString();
			}
		}

		public string PresentationAddress
		{
			get
			{
				var address = Address;
				if (Driver.IsGroupDevice)
				{
					var lastAddressInGroup = ShleifNo.ToString() + "." + (IntAddress + Driver.GroupDeviceChildrenCount - 1).ToString();
					address += " - " + lastAddressInGroup;
				}
				return address;
			}
		}

		public string DottedAddress
		{
			get
			{
				if (Driver.DriverType == XDriverType.GK)
				{
					return Address;
				}

				var address = new StringBuilder();
				foreach (var parentDevice in AllParents.Where(x => x.Driver.HasAddress))
				{
					if (parentDevice.Driver.IsGroupDevice)
						continue;

					address.Append(parentDevice.Address);
					address.Append(".");
				}
				if (Driver.HasAddress)
				{
					address.Append(Address);
					address.Append(".");
				}

				if (address.Length > 0 && address[address.Length - 1] == '.')
					address.Remove(address.Length - 1, 1);

				return address.ToString();
			}
		}

		public string DottedPresentationAddress
		{
			get
			{
				var address = DottedAddress;
				if (Driver.IsGroupDevice)
				{
					if (Children.Count > 0)
						return Children.FirstOrDefault().DottedAddress + " - " + Children.LastOrDefault().DottedAddress;
				}
				return address;
			}
		}

		public string ShortName
		{
			get
			{
				if (!string.IsNullOrEmpty(PredefinedName))
					return PredefinedName;
				return Driver.ShortName;
			}
		}

		public string ShortNameAndDottedAddress
		{
			get { return ShortName + " " + DottedAddress; }
		}

		public string PresentationAddressAndDriver
		{
			get
			{
				if (Driver.HasAddress)
					return Address + " - " + Driver.Name;
				return Driver.Name;
			}
		}

		public string ShortPresentationAddressAndDriver
		{
			get
			{
				if (Driver.HasAddress)
					return Address + " - " + ShortName;
				return ShortName;
			}
		}

		public string PresentationDriverAndAddress
		{
			get
			{
				if (Driver.DriverType == XDriverType.GK)
					return ShortName + " " + GetGKIpAddress();

				if (Driver.HasAddress)
					return ShortName + " - " + Address;
				return ShortName;
			}
		}

		public void SetAddress(string address)
		{
			try
			{
				if (Driver.HasAddress == false)
					return;

				if (Driver.IsDeviceOnShleif == false)
				{
					IntAddress = byte.Parse(address);
					return;
				}

				var addresses = address.Split('.');
				byte shleifPart = byte.Parse(addresses[0]);
				byte addressPart = byte.Parse(addresses[1]);

				ShleifNo = shleifPart;
				IntAddress = addressPart;

				if (Driver.IsGroupDevice)
				{
					for (int i = 0; i < Children.Count; i++)
					{
						Children[i].ShleifNo = ShleifNo;
						Children[i].IntAddress = (byte)(IntAddress + i);
					}
				}
			}
			catch { }
		}

		public bool CanEditAddress
		{
			get
			{
				if (Parent != null && Parent.Driver.IsGroupDevice)
					return false;
				return (Driver.HasAddress && Driver.CanEditAddress);
			}
		}

		public List<XDevice> AllParents
		{
			get
			{
				if (Parent == null)
					return new List<XDevice>();

				List<XDevice> allParents = Parent.AllParents;
				allParents.Add(Parent);
				return allParents;
			}
		}

		public XDevice GKParent
		{
			get { return AllParents.FirstOrDefault(x => x.Driver.DriverType == XDriverType.GK); }
		}

		public override XBinaryInfo BinaryInfo
		{
			get
			{
				return new XBinaryInfo()
				{
					Type = "Устройство",
					Name = Driver.ShortName,
					Address = Address
				};
			}
		}

		public override string GetBinaryDescription()
		{
			return PresentationDriverAndAddress;
		}

		public string GetGKIpAddress()
		{
			if (Driver.DriverType == XDriverType.GK)
			{
				var ipProperty = this.Properties.FirstOrDefault(x => x.Name == "IPAddress");
				if (ipProperty != null)
				{
					return ipProperty.StringValue;
				}
			}
			return null;
		}

		public bool IsRealDevice
		{
			get
			{
				if (Driver.IsGroupDevice)
					return false;
				if (Driver.DriverType == XDriverType.System)
					return false;
				return true;
			}
		}

		public void InitializeDefaultProperties()
		{
			if (Driver != null)
				foreach (var driverProperty in Driver.Properties)
				{
					if (Properties.Any(x => x.Name == driverProperty.Name) == false)
					{
						var property = new XProperty()
						{
							Name = driverProperty.Name,
							Value = driverProperty.Default
						};
						Properties.Add(property);
					}
				}
		}

		int SortingAddress
		{
			get
			{
				if (!Driver.HasAddress)
					return 0;
				if (Driver.IsDeviceOnShleif)
					return ShleifNo * 256 + IntAddress;
				return IntAddress;
			}
		}

		public void SynchronizeChildern()
		{
			if (Children.Count > 0)
			{
				Children = new List<XDevice>(Children.OrderBy(x => { return x.SortingAddress; }));
			}
		}

		public bool CanBeNotUsed
		{
			get { return ((Parent != null) && (Parent.Driver.IsGroupDevice)); }
		}

		public void OnChanged()
		{
			if (Changed != null)
				Changed();
		}
		public event Action Changed;
	}
}