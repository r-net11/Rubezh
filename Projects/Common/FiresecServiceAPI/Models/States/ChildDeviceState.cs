
namespace FiresecAPI.Models
{
	public class ChildDeviceState
	{
		public Device ChildDevice { get; set; }
		public DriverState DriverState { get; set; }
		public bool IsDeleting { get; set; }
	}
}