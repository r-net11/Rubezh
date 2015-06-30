using System.ComponentModel;

namespace FiresecAPI.SKD
{
	public enum PassCardImagePropertyType
	{
		[Description("Фото сотрудника")]
		Photo,
		[Description("Логотип организации")]
		OrganisationLogo,
		[Description("Логотип подразделения")]
		DepartmentLogo,
		[Description("Логотип должности")]
		PositionLogo,
		[Description("Дополнительно")]
		Additional,
	}
}