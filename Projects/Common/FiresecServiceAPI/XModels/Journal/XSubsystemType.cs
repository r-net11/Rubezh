using System.ComponentModel;

namespace FiresecAPI.GK
{
	public enum XSubsystemType
	{
		[DescriptionAttribute("Система")]
		System = 0,

		[DescriptionAttribute("Прибор")]
		GK = 1,
	}
}