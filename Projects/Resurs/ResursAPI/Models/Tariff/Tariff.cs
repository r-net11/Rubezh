using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ResursAPI
{
	public class Tariff:ModelBase
	{
		public Tariff():base()
		{
			Devices = new List<Device>();
			TariffParts = new List<TariffPart>();
		}
		public List<Device> Devices { get; set; }
		public List<TariffPart> TariffParts { get; set; }
		public TariffType TariffType { get; set; }
		public bool IsDiscount { get; set; }
	}
}
