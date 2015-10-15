using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;

namespace ResursAPI
{
	public class Bill : ModelBase
	{
		public Bill() : base()
		{
			Devices = new List<Device>();
		}
		public Guid ConsumerUID { get; set; }
		public Consumer Consumer { get; set; }
		[NotMapped]
		public List<Device> Devices { get; set; }
		public Decimal Balance { get; set; }
		[MaxLength(4000)]
		public string TemplatePath { get; set; }
	}
}
