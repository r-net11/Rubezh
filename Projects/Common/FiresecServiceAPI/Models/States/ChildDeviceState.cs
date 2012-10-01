
namespace FiresecAPI.Models
{
	public class ChildDeviceState
	{
		public Device ChildDevice { get; set; }
		public StateType StateType { get; set; }
		public bool IsDeleting { get; set; }
	}
}