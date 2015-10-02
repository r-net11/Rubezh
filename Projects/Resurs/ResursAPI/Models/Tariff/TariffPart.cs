using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;

namespace ResursAPI
{
	public class TariffPart
	{
		public TariffPart()
		{
			UID = Guid.NewGuid();
		}

		[Key]
		public Guid UID { get; set; }
		public int Number { get; set; }
		public double Price { get; set; }
		public double Discount { get; set; }
		public DateTime StartTime { get; set; }
		public Tariff Tariff { get; set; } 
	}
}
