using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;

namespace ResursAPI
{
	public class Consumer : ModelBase
	{
		public Consumer()
			: base()
		{
			Children = new List<Consumer>();
			Devices = new List<Device>();
		}

		public Guid? ParentUID { get; set; }
		public Consumer Parent { get; set; }
		[InverseProperty("Parent")]
		public List<Consumer> Children { get; set; }
		public bool IsFolder { get; set; }
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
		[MaxLength(100)]
		public string Number { get; set; }
		[NotMapped]
		public List<Device> Devices { get; set; }
		public Decimal Balance { get; set; }
	}
}
