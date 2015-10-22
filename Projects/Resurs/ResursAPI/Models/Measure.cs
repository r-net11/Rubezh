using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ResursAPI
{
	public class Measure
	{
		public Measure()
		{
			UID = Guid.NewGuid();
		}

		[Key]
		public Guid UID { get; set; }
		public Guid DeviceUID { get; set; }
		public int TariffPartNo { get; set; }
		/// <summary>
		/// Количество потреблённого ресурса в единицах ресурса
		/// </summary>
		public float Value { get; set; }
		/// <summary>
		/// Количество потреблённого ресурса в деньгах, NULL, если тариф не указан
		/// </summary>
		public double? MoneyValue { get; set; }
		public DateTime DateTime { get; set; }
	}
}
