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
			DeviceParameters = new List<XDeviceParameter>();
			OutDependenceUIDs = new List<Guid>();
			Zones = new List<short>();
			DeviceLogic = new XDeviceLogic();
		}

		public XDriver Driver { get; set; }
		public XDevice Parent { get; set; }
		public List<Guid> OutDependenceUIDs { get; set; }

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
		public List<XDevice> Children { get; set; }

		[DataMember]
		public List<XProperty> Properties { get; set; }

		[DataMember]
		public List<XDeviceParameter> DeviceParameters { get; set; }

		[DataMember]
		public List<short> Zones { get; set; }

		[DataMember]
		public XDeviceLogic DeviceLogic { get; set; }

		public string Address
		{
			get
			{
				if (Driver.HasAddress == false)
					return "";

				if (Driver.IsDeviceOnShleif == false)
					return IntAddress.ToString();

				return ShleifNo.ToString() + "." + IntAddress.ToString();
			}
		}

		public string DottedAddress
		{
			get
			{
				var address = new StringBuilder();
				foreach (var parentDevice in AllParents.Where(x => x.Driver.HasAddress))
				{
					if (parentDevice.Driver.IsChildAddressReservedRange)
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

		public bool CanEditAddress
		{
			get
			{
				if (Parent != null && Parent.Driver.IsChildAddressReservedRange && Parent.Driver.DriverType != XDriverType.MRK_30)
					return false;
				return (Driver.HasAddress && Driver.CanEditAddress);
			}
		}

		public byte GetReservedCount()
		{
			byte reservedCount = Driver.ChildAddressReserveRangeCount;
			if (Driver.DriverType == XDriverType.MRK_30)
			{
				reservedCount = 30;

				var reservedCountProperty = Properties.FirstOrDefault(x => x.Name == "MRK30ChildCount");
				if (reservedCountProperty != null)
				{
					reservedCount = (byte)reservedCountProperty.Value;
				}
			}
			return reservedCount;
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

		public XDevice Copy(bool fullCopy)
		{
			var newDevice = new XDevice()
			{
				DriverUID = DriverUID,
				Driver = Driver,
				IntAddress = IntAddress,
				Description = Description
			};

			if (fullCopy)
			{
				newDevice.UID = UID;
			}

			newDevice.Properties = new List<XProperty>();
			foreach (var property in Properties)
			{
				newDevice.Properties.Add(new XProperty()
				{
					Name = property.Name,
					Value = property.Value
				});
			}

			newDevice.Children = new List<XDevice>();
			foreach (var childDevice in Children)
			{
				var newChildDevice = childDevice.Copy(fullCopy);
				newChildDevice.Parent = newDevice;
				newDevice.Children.Add(newChildDevice);
			}

			return newDevice;
		}
	}
}