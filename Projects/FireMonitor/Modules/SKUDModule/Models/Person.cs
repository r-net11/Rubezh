using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SKUDModule.Models
{
	public class Person
	{
		public Person()
		{
			Passport = new Passport();
		}

		public string LastName { get; set; }
		public string FirstName { get; set; }
		public string SecondName { get; set; }
		//public Bitmap Photo{ get; set; }
		public Gender Gender { get; set; }
		public DateTime BirthDate { get; set; }
		public string BirthPlace { get; set; }
		public Passport Passport { get; set; }
		public string ITN { get; set; }
		public string SNILS { get; set; }
		public string Cell { get; set; }
		public string Address { get; set; }
		public string AddressFact { get; set; }
		public string Comment { get; set; }
	}
}
