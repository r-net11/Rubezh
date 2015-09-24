using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;

namespace ResursDAL.DataClasses
{
	public class Bill:DbModelBase
	{
		public Bill():base()
		{
			Devices = new List<Device>();
		}
		public Guid ApartmentUID { get; set; }
		public Apartment Apartment { get; set; }
		public ICollection<Device> Devices { get; set; }
		public int Balance { get; set; }
		[MaxLength(4000)]
		public string TemplatePath { get; set; }
	}
}
