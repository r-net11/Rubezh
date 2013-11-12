using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using FiresecAPI.Models;

namespace XFiresecAPI
{
	[DataContract]
	public class XDevice : XBase
	{
		public XDevice()
		{
			UID = Guid.NewGuid();
			Children = new List<XDevice>();
			Properties = new List<XProperty>();
			DeviceProperties = new List<XProperty>();
			ZoneUIDs = new List<Guid>();
			DeviceLogic = new XDeviceLogic();
			NSLogic = new XDeviceLogic();
			PlanElementUIDs = new List<Guid>();
			IsNotUsed = false;
			AllowMultipleVizualization = false;

			Zones = new List<XZone>();
			Directions = new List<XDirection>();
			DevicesInLogic = new List<XDevice>();
		}

		public XDeviceState DeviceState { get; set; }
		public override XBaseState GetXBaseState() { return DeviceState; }
		public XDriver Driver { get; set; }
		public XDriverType DriverType
		{
			get { return Driver.DriverType; }
		}
		public XDevice Parent { get; set; }
		public List<XZone> Zones { get; set; }
		public List<XDirection> Directions { get; set; }
		public List<XDevice> DevicesInLogic { get; set; }
		public bool HasDifferences { get; set; }
		public bool HasMissingDifferences { get; set; }
		public object Clone()
		{
			return this.MemberwiseClone();
		}

		[DataMember]
		public Guid UID { get; set; }

		[DataMember]
		public Guid DriverUID { get; set; }

		[DataMember]
		public byte ShleifNo { get; set; }

		public byte ShleifNoNew
		{
			get
			{
				var allParents = AllParents;
				var shleif = allParents.FirstOrDefault(x => x.DriverType == XDriverType.KAU_Shleif || x.DriverType == XDriverType.RSR2_KAU_Shleif);
				if (shleif != null)
				{
					return shleif.IntAddress;
				}
				return 0;
			}
		}

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
		public List<XProperty> DeviceProperties { get; set; }

		[DataMember]
		public List<Guid> ZoneUIDs { get; set; }

		[DataMember]
		public XDeviceLogic DeviceLogic { get; set; }

		[DataMember]
		public XDeviceLogic NSLogic { get; set; }

		[DataMember]
		public bool IsNotUsed { get; set; }

		[DataMember]
		public List<Guid> PlanElementUIDs { get; set; }

		[DataMember]
		public bool AllowMultipleVizualization { get; set; }

		[DataMember]
		public bool IsOPCUsed { get; set; }

		public string Address
		{
			get
			{
				if (DriverType == XDriverType.GK)
				{
					var ipAddress = GetGKIpAddress();
					return ipAddress != null ? ipAddress : "";
				}
				if (Driver.HasAddress == false)
					return "";

				if (Driver.IsDeviceOnShleif == false)
					return IntAddress.ToString();

				var shleifNo = ShleifNoNew;
				if (shleifNo != 0)
					return shleifNo.ToString() + "." + IntAddress.ToString();
				return IntAddress.ToString(); ;
			}
		}

		public string PresentationAddress
		{
			get
			{
				var address = Address;
				if (Driver.IsGroupDevice)
				{
					var lastAddressInGroup = Parent.IntAddress.ToString() + "." + (IntAddress + Driver.GroupDeviceChildrenCount - 1).ToString();
					address += " - " + lastAddressInGroup;
				}
				return address;
			}
		}

		public string DottedAddress
		{
			get
			{
				if (DriverType == XDriverType.GK)
				{
					return Address;
				}

				var address = new StringBuilder();
				var allParents = AllParents;
				var rootDevice = allParents.FirstOrDefault();

				foreach (var parentDevice in allParents.Where(x => x.Driver.HasAddress))
				{
					if (parentDevice.Driver.IsGroupDevice)
						continue;

					if (parentDevice.DriverType == XDriverType.KAU_Shleif || parentDevice.DriverType == XDriverType.RSR2_KAU_Shleif)
						continue;

					if (parentDevice.DriverType == XDriverType.MPT || parentDevice.DriverType == XDriverType.MRO_2)
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

				if (rootDevice != null && rootDevice.Children.Count > 1)
				{
					address.Append("(" + allParents[1].Address + ")");
				}

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
				if (DriverType == XDriverType.Pump)
				{
					if (IntAddress <= 8)
						return "Пожарный насос";
					switch (IntAddress)
					{
						case 12:
							return "Жокей насос";
						case 14:
							return "Дренажный Насос";
					}
					return "Насос с неизвестным типом";
				}
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

		public string PresentationDriverAndAddress
		{
			get
			{
				if (DriverType == XDriverType.GK)
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

				byte intAddress = byte.Parse(address);
				IntAddress = intAddress;

				if (Driver.IsGroupDevice)
				{
					for (int i = 0; i < Children.Count; i++)
					{
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
				if(AllParents.Any(x => x.DriverType == XDriverType.RSR2_KAU))
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
			get { return AllParents.FirstOrDefault(x => x.DriverType == XDriverType.GK); }
		}

		public XDevice KAUParent
		{
			get
			{
				var allParents = AllParents;
				allParents.Add(this);
				return allParents.FirstOrDefault(x => x.DriverType == XDriverType.KAU || x.DriverType == XDriverType.RSR2_KAU);
			}
		}

		public XDevice KAURSR2Parent
		{
			get
			{
				var allParents = AllParents;
				allParents.Add(this);
				return allParents.FirstOrDefault(x => x.DriverType == XDriverType.RSR2_KAU);
			}
		}

		public XDevice KAURSR2ShleifParent
		{
			get
			{
				var allParents = AllParents;
				allParents.Add(this);
				return allParents.FirstOrDefault(x => x.DriverType == XDriverType.RSR2_KAU_Shleif);
			}
		}

		public bool IsConnectedToKAURSR2OrIsKAURSR2
		{
			get { return KAURSR2Parent != null; }
		}

		public override string PresentationName
		{
			get { return ShortName + " " + DottedPresentationAddress; }
		}

		public override string DescriptorInfo
		{
			get { return "Устройство " + ShortName + " " + Address; }
		}

		public override string GetDescriptorName()
		{
			return PresentationDriverAndAddress;
		}

		public string GetGKIpAddress()
		{
			if (DriverType == XDriverType.GK)
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
				if (DriverType == XDriverType.System)
					return false;
				if (DriverType == XDriverType.KAU_Shleif || DriverType == XDriverType.RSR2_KAU_Shleif)
					return false;
				return true;
			}
		}

		public bool IsChildMPTOrMRO()
		{
			return Parent != null && (Parent.DriverType == XDriverType.MPT || Parent.DriverType == XDriverType.MRO_2);
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

		public void OnAUParametersChanged()
		{
			if (AUParametersChanged != null)
				AUParametersChanged();
		}
		public event Action AUParametersChanged;
	}
}