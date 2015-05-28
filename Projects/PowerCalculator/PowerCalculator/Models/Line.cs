using System.Collections.Generic;

namespace PowerCalculator.Models
{
	public class Line
	{
		public Line()
		{
			Name = "Название АЛС";
			Devices = new List<Device>();
		}

        public Line Init()
        {
            Devices.Add((new Device() { DriverType = DriverType.KAU }));
            return this;
        }
        
		public string Name { get; set; }
		public List<Device> Devices { get; private set; }
	}
}