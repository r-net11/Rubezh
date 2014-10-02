using System.ComponentModel;

namespace FiresecAPI.GK
{
	public enum GKSubsystemType
	{
		[DescriptionAttribute("Система")]
		System = 0,

		[DescriptionAttribute("Прибор")]
		GK = 1,
	}
}