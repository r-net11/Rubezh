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
		public TariffPart(Tariff tariff)
		{
			UID = Guid.NewGuid();
			Tariff = tariff;
		}

		[Key]
		public Guid UID { get; set; }
		public double Price { get; set; }
		public double Discount { get; set; }
		public DateTime StartTime { get; set; }
		public double Threshhold { get; set; }
		public Tariff Tariff { get; set; } 
	}
}
