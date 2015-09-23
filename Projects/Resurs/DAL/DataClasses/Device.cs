using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ResursDAL.DataClasses
{
	public class Device:DbModelBase
	{
		public Device():base()
		{
			Children = new List<Device>();
			Parameters = new List<Parameter>();
		}
		
		public Guid? ParentUID { get; set; }
		[ForeignKey("ParentUID")]
		public Device Parent { get; set; }
		[InverseProperty("Parent")]
		public ICollection<Device> Children { get; set; }

		public ICollection<Parameter> Parameters { get; set; }

		public Guid? TariffUID { get; set; } 
		public Tariff Tariff { get; set; }

		public Guid? BillUID { get; set; }
		public Bill Bill { get; set; } 

		public RubezhResurs.Devices.DeviceType Type { get; set; }

		public int Address { get; set; }
	}
}
