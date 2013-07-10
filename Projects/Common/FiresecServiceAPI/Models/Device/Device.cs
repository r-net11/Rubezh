using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using FiresecAPI.Models.Binary;


namespace FiresecAPI.Models
{
	[DataContract]
	public class Device : ICloneable
	{
		public Device()
		{
			UID = Guid.NewGuid();
			Children = new List<Device>();
			Properties = new List<Property>();
			SystemAUProperties = new List<Property>();
			DeviceAUProperties = new List<Property>();
			IndicatorLogic = new IndicatorLogic();
			PDUGroupLogic = new PDUGroupLogic();
			PlanElementUIDs = new List<Guid>();
			ZoneLogic = new ZoneLogic();
			IsRmAlarmDevice = false;
			IsNotUsed = false;
			AllowMultipleVizualization = false;

			ShapeIds = new List<string>();
			ZonesInLogic = new List<Zone>();
			DependentDevices = new List<Device>();
			ZonesConfiguration = new DeviceConfiguration();
			StateWordBytes = new List<byte>();
			RawParametersBytes = new List<byte>();
		}
        
        public bool IsUsb
	    {
	        get
	        {
	            switch (PresentationName)
	            {
                    case "USB Рубеж-2AM":
	                    return true;
                    case "USB Рубеж-4A":
                        return true;
                    case "USB Рубеж-2ОП":
                        return true;
                    case "USB БУНС":
                        return true;
                    case "USB БУНС-2":
                        return true;
                    case "USB Рубеж-П":
                        return true;
                    default:
                        return false;
	            }
	        }
	    }
		public DeviceConfiguration ZonesConfiguration { get; set; }
		public Driver Driver { get; set; }
		public Device Parent { get; set; }
		public DeviceState DeviceState { get; set; }
		public Zone Zone { get; set; }
		public bool HasDifferences { get; set; }
		public BinaryDevice BinaryDevice { get; set; }
		public List<byte> StateWordBytes { get; set; }
		public List<byte> RawParametersBytes { get; set; }

		List<Zone> _zonesInLogic;
		public List<Zone> ZonesInLogic
		{
			get
			{
				if (_zonesInLogic == null)
					_zonesInLogic = new List<Zone>();
				return _zonesInLogic;
			}
			set { _zonesInLogic = value; }
		}

		List<Device> _dependentDevices;
		public List<Device> DependentDevices
		{
			get
			{
				if (_dependentDevices == null)
					_dependentDevices = new List<Device>();
				return _dependentDevices;
			}
			set { _dependentDevices = value; }
		}

		bool _hasExternalDevices;
		public bool HasExternalDevices
		{
			get { return _hasExternalDevices; }
			set
			{
				if (_hasExternalDevices != value)
				{
					_hasExternalDevices = value;
					OnChanged();
				}
			}
		}

		[DataMember]
		public Guid UID { get; set; }

		[DataMember]
		public List<Device> Children { get; set; }

		[DataMember]
		public string DatabaseId { get; set; }

		[DataMember]
		public Guid DriverUID { get; set; }

		[DataMember]
		public int IntAddress { get; set; }

		[DataMember]
		public Guid ZoneUID { get; set; }

		[DataMember]
		public ZoneLogic ZoneLogic { get; set; }

		[DataMember]
		public IndicatorLogic IndicatorLogic { get; set; }

		[DataMember]
		public PDUGroupLogic PDUGroupLogic { get; set; }

		[DataMember]
		public List<Property> Properties { get; set; }

		[DataMember]
		public List<Property> DeviceAUProperties { get; set; }

		[DataMember]
		public List<Property> SystemAUProperties { get; set; }

		[DataMember]
		public string Description { get; set; }

		[DataMember]
		public bool IsRmAlarmDevice { get; set; }

		[DataMember]
		public List<Guid> PlanElementUIDs { get; set; }

		[DataMember]
		public bool IsMonitoringDisabled { get; set; }

		[DataMember]
		public bool IsAltInterface { get; set; }

		[DataMember]
		public bool IsNotUsed { get; set; }

		[DataMember]
		public bool IsOPCUsed { get; set; }

		[DataMember]
		public string PlaceInTree { get; set; }

		[DataMember]
		public string PathId { get; set; }

		[DataMember]
		public List<string> ShapeIds { get; set; }

		[DataMember]
		public bool AllowMultipleVizualization { get; set; }

		[DataMember]
		public int StateWordOffset { get; set; }

		[DataMember]
		public int RawParametersOffset { get; set; }

		public bool CanBeNotUsed
		{
			get { return ((Parent != null) && (Parent.Driver.ChildAddressReserveRangeCount > 0) && (Parent.Driver.DriverType != DriverType.FanControl)); }
		}

		public int GetReservedCount()
		{
			int reservedCount = Driver.ChildAddressReserveRangeCount;
			if (Driver.DriverType == DriverType.MRK_30)
			{
				reservedCount = 30;

				var reservedCountProperty = Properties.FirstOrDefault(x => x.Name == "MRK30ChildCount");
				if (reservedCountProperty != null)
				{
					if (!string.IsNullOrEmpty(reservedCountProperty.Value))
					{
						var parcedReservedCount = 0;
						if (int.TryParse(reservedCountProperty.Value, out parcedReservedCount))
						{
							reservedCount = parcedReservedCount;
							if (reservedCount < 1 || reservedCount > 30)
							{
								reservedCount = 30;
								reservedCountProperty.Value = "30";
							}
						}
					}
				}
				return reservedCount;
			}
			return reservedCount - 1;
		}

		public void SetAddress(string address)
		{
			var intAddress = AddressConverter.StringToIntAddress(Driver, address);
			if (Driver.IsChildAddressReservedRange)
			{
				if ((intAddress & 0xff) + GetReservedCount() > 255)
					return;
			}

			var oldIntAddress = IntAddress;
			IntAddress = intAddress;
			if (Driver.IsChildAddressReservedRange)
			{
				if (Driver.DriverType == DriverType.MRK_30)
				{
					foreach (var child in Children)
					{
						child.IntAddress += IntAddress - oldIntAddress;
					}
				}
				else
				{
					for (int i = 0; i < Children.Count; i++)
					{
						Children[i].IntAddress = IntAddress + i;
					}
				}
			}
			OnChanged();
		}

		public string AddressFullPath
		{
			get
			{
				string address = IntAddress.ToString();

				if (Parent != null && Parent.Driver.DriverType == DriverType.Computer)
				{
					if ((Parent.Children != null) && (Parent.Children.Where(x => (x.Driver.DriverType == Driver.DriverType)).Count() > 1))
					{
						var serialNoProperty = Properties.FirstOrDefault(x => x.Name == "SerialNo");
						if (serialNoProperty != null)
							address = serialNoProperty.Value;
					}
				}

				if (Driver.IsDeviceOnShleif)
					address = AddressConverter.IntToStringAddress(Driver, IntAddress);
				return address;
			}
		}

		public string GetId()
		{
			string currentId = Driver.UID.ToString() + ":" + AddressFullPath;
			if (Parent != null)
				return Parent.GetId() + @"/" + currentId;
			return currentId;
		}

		public bool CanEditAddress
		{
			get
			{
				if (Parent != null && Parent.Driver.IsChildAddressReservedRange && Parent.Driver.DriverType != DriverType.MRK_30)
					return false;
				return (Driver.HasAddress && Driver.CanEditAddress);
			}
		}

		public string GetPlaceInTree()
		{
			if (Parent == null)
				return "";
			var parentPlaceInTree = Parent.GetPlaceInTree();
			if (parentPlaceInTree == "")
				return Parent.Children.IndexOf(this).ToString();
			return parentPlaceInTree + @"\" + Parent.Children.IndexOf(Parent.Children.FirstOrDefault(x => x.UID == UID)).ToString();
		}
		public List<Device> AllParents
		{
			get
			{
				if (Parent == null)
					return new List<Device>();

				List<Device> allParents = Parent.AllParents;
				allParents.Add(Parent);
				return allParents;
			}
		}

		public Device ParentUSB
		{
			get
			{
				var allParents = AllParents;
				allParents.Add(this);
				return allParents.FirstOrDefault(x => (x.Driver.DriverType == DriverType.USB_BUNS ||
					x.Driver.DriverType == DriverType.USB_Rubezh_2AM ||
					x.Driver.DriverType == DriverType.USB_Rubezh_2OP ||
					x.Driver.DriverType == DriverType.USB_Rubezh_4A ||
					x.Driver.DriverType == DriverType.USB_Rubezh_P ||
					x.Driver.DriverType == DriverType.MS_1 ||
					x.Driver.DriverType == DriverType.MS_2));
			}
		}

		public Device ParentChannel
		{
			get
			{
				var usbDevice = AllParents.FirstOrDefault(x => (x.Driver.DriverType == DriverType.USB_BUNS ||
					x.Driver.DriverType == DriverType.USB_Rubezh_2AM ||
					x.Driver.DriverType == DriverType.USB_Rubezh_2OP ||
					x.Driver.DriverType == DriverType.USB_Rubezh_4A ||
					x.Driver.DriverType == DriverType.USB_Rubezh_P));

				if (usbDevice != null)
					return usbDevice;

				var channelDevice = AllParents.FirstOrDefault(x => (x.Driver.DriverType == DriverType.USB_Channel ||
					x.Driver.DriverType == DriverType.USB_Channel_1 ||
					x.Driver.DriverType == DriverType.USB_Channel_2));

				return channelDevice;
			}
		}

		public bool IsParentMonitoringDisabled
		{
			get
			{
				var allParents = AllParents;
				allParents.Add(this);
				return allParents.Any(x => x.IsMonitoringDisabled);
			}
		}

		public Device ParentPanel
		{
			get
			{
				var allParents = AllParents;
				allParents.Add(this);
				return allParents.FirstOrDefault(x => (x.Driver.IsPanel));
			}
		}

		public string ConnectedTo
		{
			get
			{
				if (Parent == null)
					return null;

				string parentPart = Parent.PresentationAddressAndName;

				if (Parent.ConnectedTo == null || Parent.Parent.ConnectedTo == null)
					return parentPart;

				return parentPart + @"\" + Parent.ConnectedTo;
			}
		}

		public void UpdateHasExternalDevices()
		{
			if (this.ZoneLogic != null)
			{
				foreach (var clause in this.ZoneLogic.Clauses)
				{
					foreach (var zone in clause.Zones)
					{
						foreach (var deviceInZone in zone.DevicesInZone)
						{
							if (this.ParentPanel.UID != deviceInZone.ParentPanel.UID)
							{
								HasExternalDevices = true;
								return;
							}
						}
					}
				}
			}
			if (Driver.DriverType == DriverType.MPT)
			{
				if (Zone != null)
				{
					foreach (var deviceInZone in Zone.DevicesInZone)
					{
						if (this.ParentPanel.UID != deviceInZone.ParentPanel.UID)
						{
							HasExternalDevices = true;
							return;
						}
					}
				}
			}
			HasExternalDevices = false;
		}

		public void SynchronizeChildern()
		{
			if (Children.Count > 0)
			{
				Children = new List<Device>(Children.OrderBy(x => { return x.IntAddress; }));
			}
		}

		public string EditingPresentationAddress
		{
			get
			{
				if (Driver.HasAddress == false)
					return "";

				return AddressConverter.IntToStringAddress(Driver, IntAddress);
			}
		}

		public string PresentationAddress
		{
			get
			{
				if (Driver == null)
					return "";

				if (Driver.HasAddress == false)
					return "";

				string address = AddressConverter.IntToStringAddress(Driver, IntAddress);

				if (Driver.IsChildAddressReservedRange)
				{
					int reservedCount = GetReservedCount();

					int endAddress = IntAddress + reservedCount;
					if (endAddress >> 8 != IntAddress >> 8)
						endAddress = (IntAddress / 256) * 256 + 255;
					address += " - " + AddressConverter.IntToStringAddress(Driver, endAddress);
				}

				return address;
			}
		}

		public string DottedAddress
		{
			get
			{
				var address = new StringBuilder();
				foreach (var parentDevice in AllParents.Where(x => x.Driver.HasAddress))
				{
					if (parentDevice.Driver.DriverType == DriverType.MPT || parentDevice.Driver.DriverType == DriverType.MRO_2)
						continue;
					if (parentDevice.Driver.IsChildAddressReservedRange)
						continue;

					address.Append(parentDevice.PresentationAddress);
					address.Append(".");
				}
				if (Driver.HasAddress)
				{
					address.Append(PresentationAddress);
					address.Append(".");
				}

				if (address.Length > 0 && address[address.Length - 1] == '.')
					address.Remove(address.Length - 1, 1);

				return address.ToString();
			}
		}

		public string PresentationName
		{
			get { return Driver.ShortName; }
		}

		public string FullPresentationName
		{
			get
			{
				var result = PresentationName;
				if (!string.IsNullOrEmpty(Description))
				{
					result += " (" + Description + ")";
				}
				return result;
			}
		}

		public string PresentationAddressAndName
		{
			get
			{
				if (Driver.HasAddress)
					return PresentationAddress + " - " + PresentationName;
				return PresentationName;
			}
		}

		public string DottedPresentationAddressAndName
		{
			get
			{
				if (Driver.HasAddress)
					return DottedAddress + " - " + PresentationName;
				return PresentationName;
			}
		}

		public string DottedPresentationNameAndAddress
		{
			get
			{
				if (Driver.HasAddress)
					return PresentationName + " - " + DottedAddress;
				return PresentationName;
			}
		}

		public string DatabaseName
		{
			get
			{
				if (Driver.HasAddress)
					return PresentationName + " " + DottedAddress;
				return PresentationName;
			}
		}

		public int ShleifNo
		{
			get { return IntAddress / 256; }
		}

		public int AddressOnShleif
		{
			get { return IntAddress % 256; }
		}

		public List<Device> GetRealChildren()
		{
			var devices = new List<Device>();
			foreach (var device in Children)
			{
				if (!IsGroupDevice(device.Driver.DriverType))
				{
					devices.Add(device);
				}
				foreach (var child in device.Children)
				{
					if (child.IsNotUsed)
						continue;
					devices.Add(child);
				}
			}
			return devices;
		}

		public List<Device> GetAllChildren()
		{
			var result = new List<Device>();
			foreach (var device in Children)
			{
				result.Add(device);
				result.AddRange(device.GetAllChildren());
			}
			return result;
		}

		bool IsGroupDevice(DriverType driverType)
		{
			switch (driverType)
			{
				case DriverType.AM4:
				case DriverType.AMP_4:
				case DriverType.RM_2:
				case DriverType.RM_3:
				case DriverType.RM_4:
				case DriverType.RM_5:
					return true;
			}
			return false;
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

		public object Clone()
		{
			return this.MemberwiseClone();
		}

		public int PumpAddress
		{
			get
			{
				switch(Driver.DriverType)
				{
					case DriverType.JokeyPump:
						return 12;
					case DriverType.Compressor:
						return 13;
					case DriverType.DrenazhPump:
						return 14;
					case DriverType.CompensationPump:
						return 15;
				}
				return IntAddress;
			}
		}
	}
}