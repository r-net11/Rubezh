using System.ComponentModel;

namespace FiresecAPI.GK
{
	public enum GKBaseObjectType
	{
		[DescriptionAttribute("Устройство")]
		Deivce,

		[DescriptionAttribute("Точка доступа")]
		Door,

		[DescriptionAttribute("Зона СКД")]
		SKDZone,
	}
}