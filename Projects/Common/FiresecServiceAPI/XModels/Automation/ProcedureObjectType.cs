using System.ComponentModel;

namespace FiresecAPI.Models
{
	public enum ProcedureObjectType
	{
		[Description("Устройства ГК")]
		XDevice,

		[Description("Камеры")]
		Camera
	}
}