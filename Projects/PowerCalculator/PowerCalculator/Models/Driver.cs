namespace PowerCalculator.Models
{
	public class Driver
	{
		public DriverType DriverType { get; set; }
		public uint Mult { get; set; }
		public double R { get; set; }
		public double I { get; set; }
		public double U { get; set; }
		public DeviceType DeviceType { get; set; }
		public double Umin { get; set; }
		public double Imax { get; set; }
	}
}