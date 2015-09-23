using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ResursDAL.DataClasses
{
	public class Tariff:DbModelBase
	{
		public Tariff():base()
		{
			Devices = new List<Device>();
			TariffParts = new List<TariffPart>();
		}
		
		public ICollection<Device> Devices { get; set; }

		public ICollection<TariffPart> TariffParts { get; set; }
	}
}
