using System.ComponentModel;

namespace XFiresecAPI
{
	public enum XSubsystemType
	{
		[DescriptionAttribute("Система")]
		System = 0,

		[DescriptionAttribute("ГК")]
		GK = 1,
	}
}