using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;

namespace ResursAPI
{
	public class Bill:ModelBase
	{
		public Bill():base()
		{
			Devices = new List<Device>();
		}
		public Consumer Consumer { get; set; }
		public List<Device> Devices { get; set; }
		public int Balance { get; set; }
		[MaxLength(4000)]
		public string TemplatePath { get; set; }
	}
}
