using System;
using System.ComponentModel.DataAnnotations;

namespace ResursAPI
{
	public class Deposit
	{
		public Deposit()
		{
			UID = Guid.NewGuid();
		}
		[Key]
		public Guid UID { get; set; }
		public DateTime Moment { get; set; }
		public Guid ConsumerUID { get; set; }
		public Consumer Consumer { get; set; }
		public Decimal Amount { get; set; }
		[MaxLength(200)]
		public string Description { get; set; }
	}
}
