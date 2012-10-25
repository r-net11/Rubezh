using System.ComponentModel;

namespace XFiresecAPI
{
	public enum XStateClass
	{
		[DescriptionAttribute("Норма")]
		Norm = 0,

		[DescriptionAttribute("Внимание")]
		Attention = 1,

		[DescriptionAttribute("Пожар 1")]
		Fire1 = 2,

		[DescriptionAttribute("Пожар 2")]
		Fire2 = 3,
	}
}