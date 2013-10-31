using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SKUDModule.Models
{
	public class Group
	{
		public Group()
		{
			;
		}

		public Group(string name)
		{
			Name = name;
		}

		public string Name { get; set; }
	}
}
