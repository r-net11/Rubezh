using System.ComponentModel;

namespace FiresecAPI.GK
{
	public enum GKBaseObjectType
	{
		[DescriptionAttribute("Устройство")]
		Deivce,

		[DescriptionAttribute("ПИМ")]
		Pim,

		[DescriptionAttribute("Точка доступа")]
		Door,

		[DescriptionAttribute("Зона СКД")]
		SKDZone,
	}
}