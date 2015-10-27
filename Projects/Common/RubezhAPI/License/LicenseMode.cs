using System.ComponentModel;

namespace RubezhAPI.License
{
	/// <summary>
	/// Тип лицензии
	/// </summary>
	public enum LicenseMode
	{
		[Description("Лицензия отсутствует")]
		NoLicense = 0,
		[Description("Лицензия получена")]
		HasLicense = 1,
		[Description("Демонстрационный режим")]
		Demonstration = 2
	}
}