using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;

namespace ResursDAL.DataClasses
{
	public class TariffPart
	{
		[Key]
		public Guid UID { get; set; }

		public int Number { get; set; }

		public double Price { get; set; }

		public double Discount { get; set; }

		public DateTime StartTime { get; set; }

		public Guid TariffUID { get; set; }
		public Tariff Tariff { get; set; }
	}
}
