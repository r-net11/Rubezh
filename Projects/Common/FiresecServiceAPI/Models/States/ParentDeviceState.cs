
namespace FiresecAPI.Models
{
	public class ParentDeviceState
	{
		public Device ParentDevice { get; set; }
		public DriverState DriverState { get; set; }
		public bool IsDeleting { get; set; }
	}
}