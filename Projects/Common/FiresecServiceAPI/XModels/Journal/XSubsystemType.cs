using System.ComponentModel;

namespace XFiresecAPI
{
	public enum XSubsystemType
	{
		[DescriptionAttribute("Система")]
		System = 0,

		[DescriptionAttribute("Прибор")]
		GK = 1,
	}
}