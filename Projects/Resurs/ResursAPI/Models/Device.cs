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
		public Device(ResursAPI.DriverType driverType, Device parent = null)
			: this()
		{
			Driver = ResursAPI.DriversConfiguration.GetDriver(driverType);
			DriverType = driverType;
			foreach (var item in Driver.DriverParameters)
			{
				Parameters.Add(new Parameter { DriverParameter = item, Device = this, Number = item.Number });
			}
			if(parent != null)
			{
				Parent = parent;
				Parent.Children.Add(this);
				Address = Parent.Children.Count;
			}
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

		[Key]
		public Guid UID { get; set; }
		[MaxLength(200)]
		public string Description { get; set; }
		public Guid? ParentUID { get; set; }
		public Device Parent { get; set; }
		[InverseProperty("Parent")]
		public List<Device> Children { get; set; }
		public List<Parameter> Parameters { get; set; }
		public Tariff Tariff { get; set; }
		public Bill Bill { get; set; } 
		public DriverType DriverType { get; set; }
		public int Address { get; set; }
		public bool IsActive { get; set; }
		public bool IsDbMissmatch { get; set; }
		[NotMapped]
		public string Name { get { return Driver.DriverType.ToDescription(); } }
		[NotMapped]
		public Driver Driver { get; set; }
		[NotMapped]
		public DeviceType DeviceType { get { return Driver.DeviceType; } }
		[NotMapped]
		public string FullAddress { get; private set; }
		[NotMapped]
		public bool IsLoaded { get; set; }
	}
}
