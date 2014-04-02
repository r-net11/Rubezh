using System.ComponentModel;

namespace FiresecAPI
{
	public enum SKDSubsystemType
	{
		[DescriptionAttribute("Система")]
		System = 0,

		[DescriptionAttribute("Прибор")]
		Device = 1,
	}
}