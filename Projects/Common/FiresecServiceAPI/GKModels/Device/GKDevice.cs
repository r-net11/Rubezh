﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Xml.Serialization;
using Infrustructure.Plans.Interfaces;

namespace FiresecAPI.GK
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
			GuardZoneUIDs = new List<Guid>();
			Logic = new GKLogic();
			NSLogic = new GKLogic();
			PlanElementUIDs = new List<Guid>();
			IsNotUsed = false;
			AllowMultipleVizualization = false;

			Zones = new List<GKZone>();
			GuardZones = new List<GKGuardZone>();
			Directions = new List<GKDirection>();
        }

		public override void Update(GKDevice device)
		{
			Logic.GetAllClauses().FindAll(x => x.Devices.Contains(device)).ForEach(y => { y.Devices.Remove(device); y.DeviceUIDs.Remove(device.UID); });
			UnLinkObject(device);
			OnChanged();
		}

		public override void Update(GKDirection direction)
		{
			Logic.GetAllClauses().FindAll(x => x.Directions.Contains(direction)).ForEach(y => { y.Directions.Remove(direction); y.DirectionUIDs.Remove(direction.UID); });
			UnLinkObject(direction);
			OnChanged();
		}

        [XmlIgnore]
		public bool IsDisabled{ get; set; }
		[XmlIgnore]
		public override GKBaseObjectType ObjectType { get { return GKBaseObjectType.Deivce; } }
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
		public List<GKDirection> Directions { get; set; }
		[XmlIgnore]
		public GKDoor Door { get; set; }
		[XmlIgnore]
		public bool HasDifferences { get; set; }
		[XmlIgnore]
		public bool HasMissingDifferences { get; set; }
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
		public byte IntAddress { get; set; }

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
		/// Идентификаторы охранных зон
		/// </summary>
		[DataMember]
		public List<Guid> GuardZoneUIDs { get; set; }

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
		public GKMirrorItem MirrorItem { get; set; }

		[DataMember]
		public bool IsOPCUsed { get; set; }

		[XmlIgnore]
		public byte ShleifNo
		{
			get
			{
				var allParents = AllParents;
				var shleif = allParents.FirstOrDefault(x => x.DriverType == GKDriverType.RSR2_KAU_Shleif);
				if (shleif != null)
				{
					return shleif.IntAddress;
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
					ushort lineNo = FiresecClient.GKManager.GetKauLine(this);
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

		[XmlIgnore]
		public string DottedAddress
		{
			get
			{
				if (DriverType == GKDriverType.GK)
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

				if (rootDevice != null && rootDevice.Children.Count > 1 && rootDevice.Driver.DriverType == GKDriverType.System)
				{
					address.Append("(" + allParents[1].Address + ")");
				}

				return address.ToString();
			}
		}

		[XmlIgnore]
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
			var result = ShortName + " " + DottedPresentationAddress;
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
				return (Driver.HasAddress && Driver.CanEditAddress);
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
			get { return AllParents.FirstOrDefault(x => x.DriverType == GKDriverType.GK); }
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

		public void SynchronizeChildern()
		{
			if (Children.Count > 0)
			{
				Children = new List<GKDevice>(Children.OrderBy(x => { return x.SortingAddress; }));
			}
		}

		[XmlIgnore]
		public bool CanBeNotUsed
		{
			get { return (Parent != null && Parent.Driver.IsGroupDevice); }
		}

		public void OnRemoved()
		{
			var newOutputObjects = new List<GKBase>(OutputObjects);
			foreach (var outputObject in newOutputObjects)
			{
				outputObject.Update(this);
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