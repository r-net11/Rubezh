using System.ComponentModel;

namespace FiresecAPI.SKD.PassCardLibrary
{
	public enum PassCardImagePropertyType
	{
		[Description("Фото")]
		Photo,
		[Description("Логотип организации")]
		OrganizationLogo,
		[Description("Логотип подразделения")]
		DepartmentLogo,
		[Description("Логотип должности")]
		PositionLogo,
		[Description("Дополнительно")]
		Additional,
	}
}
