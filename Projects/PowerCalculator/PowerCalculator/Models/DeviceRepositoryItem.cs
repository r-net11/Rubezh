
namespace PowerCalculator.Models
{
	public class DeviceRepositoryItem
	{
		public DeviceRepositoryItem()
		{
			Count = 1;
		}

		public DriverType DriverType { get; set; }
		public int Count { get; set; }
	}
}