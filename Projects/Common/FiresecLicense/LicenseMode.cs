using System.ComponentModel;

namespace FiresecLicense
{
	public enum LicenseMode
	{
		[Description("Лицензия отсутствует")]
		NoLicense,
		[Description("Лицензия получена")]
		HasLicense,
		[Description("Демонстрационный режим")]
		Demonstration
	}
}
