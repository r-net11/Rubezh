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
			Parameters = new List<Parameter>(); ;
		}
		public Device(ResursAPI.DriverType driverType)
			: this()
		{
			Driver = ResursAPI.DriversConfiguration.GetDriver(driverType);
			DriverType = driverType;
			foreach (var item in Driver.DriverParameters)
			{
				Parameters.Add(new Parameter { DriverParameter = item, Device = this, Number = item.Number });
			}
		}

		public void AddChild(ResursAPI.DriverType driverType)
		{
			if (!Driver.Children.Any(x => x == driverType))
				return;
			var device = new Device(driverType);
			device.SetAddress(this, Children.Count + 1);
			device.Parent = this;
			Children.Add(device);
		}

		public void AddChild(Device device)
		{
			if (!Driver.Children.Any(x => x == device.Driver.DriverType))
				return;
			device.SetAddress(this, Children.Count + 1);
			device.Parent = this;
			Children.Add(device);
		}
		void SetAddress(Device parent, int number)
		{
			if (parent.Children.Any(x => x.Address == number))
				return;
			Address = number;
		}

		public void SetFullAddress()
		{
			if (Parent == null)
				FullAddress = "";
			else if (Parent.FullAddress.Equals(""))
				FullAddress = Address.ToString();
			else
				FullAddress = Parent.FullAddress + "." + Address.ToString();
		}

		[Key]
		public Guid UID { get; set; }
		[MaxLength(200)]
		public string Description { get; set; }
		public Device Parent { get; set; }
		[InverseProperty("Parent")]
		public List<Device> Children { get; set; }
		public List<Parameter> Parameters { get; set; }
		public Tariff Tariff { get; set; }
		public Bill Bill { get; set; } 
		public DriverType DriverType { get; set; }
		public int Address { get; set; }
		public bool IsActive { get; set; }
		[NotMapped]
		public string Name { get { return Driver.DriverType.ToDescription(); } }
		[NotMapped]
		public Driver Driver { get; set; }
		[NotMapped]
		public string FullAddress { get; private set; }
	}
}
