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
	}
}