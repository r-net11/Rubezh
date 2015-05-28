using System.Xml.Serialization;
using PowerCalculator.Processor;

namespace PowerCalculator.Models
{
	public class DeviceRepositoryItem
	{
		public DeviceRepositoryItem()
		{
			DriverType = DriverType.RSR2_SmokeDetector;
			Count = 1;
		}

		public DriverType DriverType { get; set; }
		public int Count { get; set; }

		[XmlIgnore]
		public Driver Driver
		{
			get { return DriversHelper.GetDriver(DriverType); }
		}
	}
}