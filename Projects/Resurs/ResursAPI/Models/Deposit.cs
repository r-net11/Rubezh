using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ResursAPI
{
	public class Deposit : ModelBase
	{
		public DateTime Moment { get; set; }
		public Guid ConsumerUID { get; set; }
		public Consumer Consumer { get; set; }
		public Decimal Amount { get; set; }
	}
}
