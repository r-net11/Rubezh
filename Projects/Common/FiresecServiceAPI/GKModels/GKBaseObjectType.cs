using System.ComponentModel;

namespace FiresecAPI.GK
{
	public enum GKBaseObjectType
	{
		[DescriptionAttribute("Устройство")]
		Deivce,

		[DescriptionAttribute("Зона СКД")]
		SKDZone,
	}
}