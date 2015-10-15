using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace ResursAPI
{
	public class Device
	{
		public Device()
		{
			UID = Guid.NewGuid();
			Children = new List<Device>();
			Parameters = new List<Parameter>();
			FullAddress = "";
		}
		public Device(ResursAPI.DriverType driverType, Device parent = null)
			: this()
		{
			Name = driverType.ToDescription();
			Driver = ResursAPI.DriversConfiguration.GetDriver(driverType);
			DriverType = driverType;
			TariffType = Driver.DefaultTariffType;
			foreach (var item in Driver.DriverParameters)
			{
				var parameter = new Parameter { Device = this };
				parameter.Initialize(item);
				Parameters.Add(parameter);
			}

			SetParent(parent);
		}

		public void SetParent(Device parent, int? address = null)
		{
			if (parent == null)
				return;
			Parent = parent;
			Parent.Children.Add(this);
			if (address != null)
				Address = address.Value;
			else
				Address = Parent.Children.Count;
			SetFullAddress();
		}

		public void SetFullAddress()
		{
			if (Parent == null || Parent.FullAddress == null)
				FullAddress = "";
			else if (Parent.FullAddress.Equals(""))
				FullAddress = Address.ToString();
			else
				FullAddress = Parent.FullAddress + "." + Address.ToString();
		}

		public ValueType GetParameter(string name)
		{
			if (name == ParameterNames.ParameterNamesBase.Id)
				return UID;
			if (name == ParameterNames.ParameterNamesBase.Address)
				return Address;
			if (name == ParameterNames.ParameterNamesBase.PortName)
			{
				if(DeviceType != ResursAPI.DeviceType.Network)
					throw new Exception("Для данного типа устройства значение PortName не задано");
				return new ParameterStringContainer { Value = ComPort };
			}
			if (name == ParameterNames.ParameterNamesBase.DateTime)
			{
				if(DeviceType != ResursAPI.DeviceType.Counter)
					throw new Exception("Для данного типа устройства значение DateTime не задано");
				return DateTime;
			}
			return Parameters.FirstOrDefault(x => x.DriverParameter.Name == name).ValueType;
		}

		[Key]
		public Guid UID { get; set; }
		[MaxLength(50)]
		public string Name { get; set; }
		[MaxLength(200)]
		public string Description { get; set; }
		public Guid? ParentUID { get; set; }
		public Device Parent { get; set; }
		[InverseProperty("Parent")]
		public List<Device> Children { get; set; }
		public List<Parameter> Parameters { get; set; }
		public Guid? TariffUID { get; set; }
		public Tariff Tariff { get; set; }
		public Guid? BillUID { get; set; }
		public Bill Bill { get; set; } 
		public DriverType DriverType { get; set; }
		public int Address { get; set; }
		public bool IsActive { get; set; }
		public bool IsDbMissmatch { get; set; }
		public TariffType TariffType { get; set; }
		public DateTime DateTime { get; set;}
		[MaxLength(10)]
		public string ComPort { get; set; }
		[NotMapped]
		public Driver Driver { get; set; }
		[NotMapped]
		public DeviceType DeviceType { get { return Driver.DeviceType; } }
		[NotMapped]
		public string FullAddress { get; private set; }
		[NotMapped]
		public bool IsLoaded { get; set; }
		[NotMapped]
		public bool CanMonitor 
		{ 
			get 
			{
				return DriverType == DriverType.IncotextNetwork ||
					DriverType == DriverType.Mercury203Counter ||
					DriverType == ResursAPI.DriverType.VirtualIncotextNetwork ||
					DriverType == ResursAPI.DriverType.VirtualMercury203Counter ||
					DriverType == ResursAPI.DriverType.VirtualMZEP55Counter ||
					DriverType == ResursAPI.DriverType.VirtualMZEP55Network;
			} 
		}

	}
}
