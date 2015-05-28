using System.Xml.Serialization;
using PowerCalculator.Processor;
namespace PowerCalculator.Models
{
	public class Device
	{
		public Device()
		{
			Cable = new Cable();
		}

		public DriverType DriverType { get; set; }
        public Cable Cable { get; set; }

		[XmlIgnore]
		public Driver Driver
		{
			get { return DriversHelper.GetDriver(DriverType); }
		}
	}
}