using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SKUDModule.Models
{
	public class Employee
	{
		public Employee()
		{
			Person = new Person();
		}

		public int Id { get; set; }
		public Person Person { get; set; }
		public string ClockNumber { get; set; }
		public Department Department { get; set; }
		public Position Position { get; set; }
		public string Phone { get; set; }
		public string Comment { get; set; }
		public string Email { get; set; }
		public Group Group { get; set; }
		public bool IsDeleted { get; set; }
	}
}
