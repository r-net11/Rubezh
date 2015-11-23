﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Xml.Serialization;
using Infrustructure.Plans.Interfaces;
using RubezhClient;

namespace RubezhAPI.GK
{
	/// <summary>
	/// Устройство ГК
	/// </summary>
	[DataContract]
	public class GKDevice : GKBase, IPlanPresentable
	{
		public GKDevice()
		{
			Children = new List<GKDevice>();
			Properties = new List<GKProperty>();
			DeviceProperties = new List<GKProperty>();
			ZoneUIDs = new List<Guid>();
			Logic = new GKLogic();
			NSLogic = new GKLogic();
			PlanElementUIDs = new List<Guid>();
			GKMirrorItem = new GKMirrorItem();
			IsNotUsed = false;
			AllowMultipleVizualization = false;

			Zones = new List<GKZone>();
			GuardZones = new List<GKGuardZone>();
		}

		public override void Invalidate(GKDeviceConfiguration deviceConfiguration)
		{
			if (Driver.HasLogic)
			{
				UpdateLogic(deviceConfiguration);
				Logic.GetObjects().ForEach(x =>
				{
					AddDependentElement(x);
				});
				NSLogic.GetObjects().ForEach(x =>
				{
					AddDependentElement(x);
				});
			}
			if (Driver.HasZone)
			{
				var zoneUIDs = new List<Guid>();
				var zones = new List<GKZone>();

				foreach (var zoneUID in ZoneUIDs)
				{
					var zone = deviceConfiguration.Zones.FirstOrDefault(x => x.UID == zoneUID);
					if (zone != null)
					{
						zones.Add(zone);
						zoneUIDs.Add(zoneUID);
						if (!zone.Devices.Contains(this))
							zone.Devices.Add(this);
						AddDependentElement(zone);
					}
				}
				Zones = zones;
				ZoneUIDs = zoneUIDs;
			}
			//if (Driver.HasGuardZone)
			//{
			//	var guardZones = new List<GKGuardZone>();

			//	foreach (var guardZone in deviceConfiguration.GuardZones.Where(x => GuardZones.Any(y=> y.UID == x.UID)))
			//	{
			//			guardZones.Add(guardZone);
			//			AddDependentElement(guardZone);
			//	}
			//	GuardZones = guardZones;
			//}
		}

		public override void UpdateLogic(GKDeviceConfiguration deviceConfiguration)
		{
			deviceConfiguration.InvalidateOneLogic(this, Logic);
			deviceConfiguration.InvalidateOneLogic(this, NSLogic);
		}

		[XmlIgnore]
		public bool IsDisabled { get; set; }
		[XmlIgnore]
		public override GKBaseObjectType ObjectType { get { return GKBaseObjectType.Device; } }
		[XmlIgnore]
		public GKDriver Driver { get; set; }
		[XmlIgnore]
		public GKDriverType DriverType
		{
			get { return Driver.DriverType; }
		}
		[XmlIgnore]
		public GKDevice Parent { get; set; }
		[XmlIgnore]
		public List<GKZone> Zones { get; set; }
		[XmlIgnore]
		public List<GKGuardZone> GuardZones { get; set; }
		[XmlIgnore]
		public GKDoor Door { get; set; }
		[XmlIgnore]
		public bool IsInMPT { get; set; }
		public object Clone()
		{
			return this.MemberwiseClone();
		}

		/// <summary>
		/// Идентификатор драйвера
		/// </summary>
		[DataMember]
		public Guid DriverUID { get; set; }

		/// <summary>
		/// Адрес
		/// </summary>
		[DataMember]
		public int IntAddress { get; set; }

		[DataMember]
		public string PredefinedName { get; set; }

		/// <summary>
		/// Идентификаторы дочерних устройств
		/// </summary>
		[DataMember]
		public List<GKDevice> Children { get; set; }

		/// <summary>
		/// Свойства, настроенные в системе
		/// </summary>
		[DataMember]
		public List<GKProperty> Properties { get; set; }

		/// <summary>
		/// Свойства, настроенные в приборе
		/// </summary>
		[DataMember]
		public List<GKProperty> DeviceProperties { get; set; }

		/// <summary>
		/// Идентификаторы зон
		/// </summary>
		[DataMember]
		public List<Guid> ZoneUIDs { get; set; }

		/// <summary>
		/// Логика сработки
		/// </summary>
		[DataMember]
		public GKLogic Logic { get; set; }

		/// <summary>
		/// Логика насосной станции
		/// </summary>
		[DataMember]
		public GKLogic NSLogic { get; set; }

		[DataMember]
		public bool IsNotUsed { get; set; }

		[DataMember]
		public List<Guid> PlanElementUIDs { get; set; }

		/// <summary>
		/// Разрешить множественное размещение на плане
		/// </summary>
		[DataMember]
		public bool AllowMultipleVizualization { get; set; }

		/// <summary>
		/// Проектный адрес
		/// </summary>
		[DataMember]
		public string ProjectAddress { get; set; }

		/// <summary>
		/// Отражение объекта ГК
		/// </summary>
		[DataMember]
		public GKMirrorItem GKMirrorItem { get; set; }


		[XmlIgnore]
		public byte ShleifNo
		{
			get
			{
				var allParents = AllParents;
				var shleif = allParents.FirstOrDefault(x => x.DriverType == GKDriverType.RSR2_KAU_Shleif);
				if (shleif != null)
				{
					return (byte)shleif.IntAddress;
				}
				return 0;
			}
		}

		[XmlIgnore]
		public string Address
		{
			get
			{
				if (DriverType == GKDriverType.GK)
				{
					var ipAddress = GetGKIpAddress();
					return ipAddress != null ? ipAddress : "";
				}
				if (!Driver.HasAddress)
					return "";

				if (DriverType == GKDriverType.RSR2_KAU)
				{
					ushort lineNo = GKManager.GetKauLine(this);
					if (lineNo > 0)
						return "РЛС " + IntAddress.ToString();
					return IntAddress.ToString();
				}

				if (!Driver.IsDeviceOnShleif)
					return IntAddress.ToString();

				var shleifNo = ShleifNo;
				if (shleifNo != 0)
					return shleifNo.ToString() + "." + IntAddress.ToString();
				return IntAddress.ToString();
			}
		}

		[XmlIgnore]
		public string PresentationAddress
		{
			get
			{
				var address = Address;
				if (Driver.IsGroupDevice)
				{
					var firstDevice = Children.FirstOrDefault();
					var lastDevice = Children.LastOrDefault();
					if (firstDevice != null && lastDevice != null)
					{
						address = firstDevice.Address + " - " + lastDevice.Address;
					}
				}
				return address;
			}
		}
		/// <summary>
		/// Адрес устройства в ГК
		/// </summary>
		[XmlIgnore]
		public string DottedAddress
		{
			get
			{
				if (DriverType == GKDriverType.GK && !Driver.HasAddress)
					return "";

				var address = new StringBuilder();

				foreach (var parentDevice in AllParents.Where(x => x.Driver.HasAddress))
				{
					if (parentDevice.Driver.IsGroupDevice)
						continue;

					if (parentDevice.DriverType == GKDriverType.RSR2_KAU_Shleif)
						continue;

					if (parentDevice.DriverType == GKDriverType.RSR2_MVP || parentDevice.DriverType == GKDriverType.RSR2_MVP_Part)
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
		/// <summary>
		/// Адрес утройства для отображения в интерфейсе в плоских списках устройств (напр. "1.2.1") и ip-адрес ГК, в случае более чем одного ГК в конфигурации (напр. "1.2.1 (192.168.0.1)").
		/// </summary>
		[XmlIgnore]
		public string DottedPresentationAddress
		{
			get
			{
				if (DriverType == GKDriverType.GK) 
				{
					return Address;
				}
				var address = DottedAddress;
				if (Driver.IsGroupDevice)
				{
					if (Children.Count > 0)
						return Children.FirstOrDefault().DottedAddress + " - " + Children.LastOrDefault().DottedAddress;
				}

				var allParents = AllParents;
				var rootDevice = allParents.FirstOrDefault();
				if (rootDevice != null && rootDevice.Children.Count > 1 && rootDevice.Driver.DriverType == GKDriverType.System)
				{
					address = address + (" (" + allParents[1].Address + ")");
				}
				return address;
			}
		}

		[XmlIgnore]
		public string ShortName
		{
			get
			{
				if (!string.IsNullOrEmpty(PredefinedName))
					return PredefinedName;
				return Driver.ShortName;
			}
		}

		[XmlIgnore]
		public override string PresentationName
		{
			get
			{
				var result = ShortName + " " + DottedPresentationAddress;
				if (!string.IsNullOrEmpty(Description))
				{
					result += "(" + Description + ")";
				}
				return result;
			}
		}

		[XmlIgnore]
		public override string ImageSource
		{
			get { return Driver.ImageSource; }
		}

		public override string GetGKDescriptorName(GKNameGenerationType gkNameGenerationType)
		{
			var result = ShortName + " " + DottedAddress;
			switch (gkNameGenerationType)
			{
				case GKNameGenerationType.DriverTypePlusAddressPlusDescription:
					if (!string.IsNullOrEmpty(Description))
						result += "(" + Description + ")";
					break;

				case GKNameGenerationType.Description:
					if (!string.IsNullOrEmpty(Description))
						result = Description;
					break;

				case GKNameGenerationType.ProjectAddress:
					if (!string.IsNullOrEmpty(ProjectAddress))
						result = ProjectAddress;
					break;

				case GKNameGenerationType.DescriptionOrProjectAddress:
					if (!string.IsNullOrEmpty(Description))
						result = Description;
					else if (!string.IsNullOrEmpty(ProjectAddress))
						result = ProjectAddress;
					break;

				case GKNameGenerationType.ProjectAddressOrDescription:
					if (!string.IsNullOrEmpty(ProjectAddress))
						result = ProjectAddress;
					else if (!string.IsNullOrEmpty(Description))
						result = Description;
					break;
			}
			if (result.Length > 32)
				result = result.Substring(0, 32);
			return result.TrimEnd(' ');
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

		[XmlIgnore]
		public bool CanEditAddress
		{
			get
			{
				if (Parent != null && Parent.Driver.IsGroupDevice)
					return false;
				if (AllParents.Any(x => x.DriverType == GKDriverType.RSR2_KAU))
					return false;
				return Driver.HasAddress && Driver.CanEditAddress;
			}
		}

		[XmlIgnore]
		public List<GKDevice> AllParents
		{
			get
			{
				if (Parent == null)
					return new List<GKDevice>();

				List<GKDevice> allParents = Parent.AllParents;
				allParents.Add(Parent);
				return allParents;
			}
		}

		[XmlIgnore]
		public List<GKDevice> AllChildren
		{
			get
			{
				var allChildren = new List<GKDevice>();
				foreach (var child in Children)
				{
					allChildren.Add(child);
					allChildren.AddRange(child.AllChildren);
				}
				return allChildren;
			}
		}

		[XmlIgnore]
		public List<GKDevice> AllChildrenAndSelf
		{
			get
			{
				var allChildren = new List<GKDevice>();
				allChildren.Add(this);
				allChildren.AddRange(AllChildren);
				return allChildren;
			}
		}

		[XmlIgnore]
		public GKDevice GKParent
		{
			get
			{
				var allParents = AllParents;
				allParents.Add(this);
				return allParents.FirstOrDefault(x => x.DriverType == GKDriverType.GK);
			}
		}

		[XmlIgnore]
		public GKDevice KAUParent
		{
			get
			{
				var allParents = AllParents;
				allParents.Add(this);
				return allParents.FirstOrDefault(x => x.DriverType == GKDriverType.RSR2_KAU);
			}
		}

		[XmlIgnore]
		public GKDevice KAUShleifParent
		{
			get
			{
				var allParents = AllParents;
				allParents.Add(this);
				return allParents.FirstOrDefault(x => x.DriverType == GKDriverType.RSR2_KAU_Shleif);
			}
		}

		[XmlIgnore]
		public GKDevice MVPPartParent
		{
			get
			{
				var allParents = AllParents;
				allParents.Add(this);
				return allParents.LastOrDefault(x => x.DriverType == GKDriverType.RSR2_MVP_Part);
			}
		}

		[XmlIgnore]
		public GKDevice MirrorParent
		{
			get
			{
				var allParents = AllParents;
				allParents.Add(this);
				return allParents.LastOrDefault(x => x.DriverType == GKDriverType.RSR2_GKMirror);
			}
		}

		[XmlIgnore]
		public bool IsConnectedToKAU
		{
			get { return KAUParent != null; }
		}

		public string GetGKIpAddress()
		{
			if (DriverType == GKDriverType.GK)
			{
				var ipProperty = this.Properties.FirstOrDefault(x => x.Name == "IPAddress");
				if (ipProperty != null)
				{
					return ipProperty.StringValue;
				}
			}
			return null;
		}

		[XmlIgnore]
		public bool IsRealDevice
		{
			get
			{
				if (Driver == null || Driver.IsGroupDevice)
					return false;
				return Driver.IsReal;
			}
		}

		public void InitializeDefaultProperties()
		{
			if (Driver != null)
				foreach (var driverProperty in Driver.Properties)
				{
					if (Properties.Any(x => x.Name == driverProperty.Name) == false)
					{
						var property = new GKProperty()
						{
							DriverProperty = driverProperty,
							Name = driverProperty.Name,
							Value = driverProperty.Default
						};
						Properties.Add(property);
					}
				}
		}

		[XmlIgnore]
		int SortingAddress
		{
			get
			{
				if (!Driver.HasAddress)
					return 0;
				return IntAddress;
			}
		}

		public void SynchronizeChildren()
		{
			if (Children.Count > 0)
			{
				Children = new List<GKDevice>(Children.OrderBy(x => { return x.SortingAddress; }));
			}
		}

		[XmlIgnore]
		public bool CanBeNotUsed
		{
			get { return Parent != null && Parent.Driver.IsGroupDevice; }
		}

		public void OnAUParametersChanged()
		{
			if (AUParametersChanged != null)
				AUParametersChanged();
		}
		public event Action AUParametersChanged;
	}
}