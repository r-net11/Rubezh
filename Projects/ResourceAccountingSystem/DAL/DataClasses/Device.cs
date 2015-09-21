using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.DataClasses
{
	public class Device
	{
		[Key]
		public Guid UID { get; set; }

		public Guid? ParentUID { get; set; }
		[ForeignKey("ParentUID")]
		public Device Parent { get; set; }
		[InverseProperty("Parent")]
		public ICollection<Device> Children { get; set; }

		public ICollection<Parameter> Parameters { get; set; }

		public Guid? TariffUID { get; set; } 
		public Tariff Tariff { get; set; } 

		public RubezhResurs.Devices.DeviceType Type { get; set; }

		public int Address { get; set; }
	}
}
