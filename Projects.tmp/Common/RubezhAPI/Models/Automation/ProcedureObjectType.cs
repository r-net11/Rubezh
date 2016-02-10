using System.ComponentModel;

namespace FiresecAPI.Automation
{
	public enum ProcedureObjectType
	{
		[Description("Устройства ГК")]
		XDevice,

		[Description("Камеры")]
		Camera
	}
}