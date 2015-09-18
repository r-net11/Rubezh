using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ResursAPI
{
	public class Apartment : ModelBase
	{
		public Apartment()
		{
			Children = new List<Apartment>();
		}

		public List<Apartment> Children { get; set; }
	}
}