using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;

namespace PowerCalculator.Models
{
	public class Line
	{
		public Line()
		{
			Name = "Название АЛС";
			Devices = new List<Device>();
            KAU = new Device() { DriverType = DriverType.RSR2_KAU };
		}

        [XmlIgnore]
        public Device KAU { get; private set; } 
		public string Name { get; set; }
		public List<Device> Devices { get; set; }

        public uint MaxAdress
        {
            get { return KAU.Driver.Mult + (uint)Devices.Sum(x=>x.Driver.Mult); }
        }
	}
}