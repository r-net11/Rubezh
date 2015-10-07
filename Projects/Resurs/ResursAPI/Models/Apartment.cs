using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;

namespace ResursAPI
{
	public class Apartment : ModelBase
	{
		public Apartment():base()
		{
			Children = new List<Apartment>();
			Bills = new List<Bill>();
		}

		public Guid? ParentUID { get; set; }
		public Apartment Parent { get; set; }
		[InverseProperty("Parent")]
		public List<Apartment> Children { get; set; }
		public List<Bill> Bills { get; set; }
		public bool IsFolder { get; set; }
		[MaxLength(100)]
		public string FIO { get; set; }
		[MaxLength(100)]
		public string Address { get; set; }
		[MaxLength(100)]
		public string Phone { get; set; }
		[MaxLength(100)]
		public string Email { get; set; }
		[MaxLength(100)]
		public string Login { get; set; }
		[MaxLength(100)]
		public string Password { get; set; }
		public bool IsSendEmail { get; set; }
	}
}
