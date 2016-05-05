using System.ComponentModel;
using LocalizationConveters;

namespace FiresecAPI.SKD
{
	public enum PassCardImagePropertyType
	{
		//[Description("Фото сотрудника")]
        [LocalizedDescription(typeof(Resources.Language.SKD.PassCardLibrary.PassCardImagePropertyType), "Photo")]
		Photo,

        //[Description("Логотип организации")]
        [LocalizedDescription(typeof(Resources.Language.SKD.PassCardLibrary.PassCardImagePropertyType), "OrganisationLogo")]
		OrganisationLogo,

        //[Description("Логотип подразделения")]
        [LocalizedDescription(typeof(Resources.Language.SKD.PassCardLibrary.PassCardImagePropertyType), "DepartmentLogo")]
		DepartmentLogo,

        //[Description("Логотип должности")]
        [LocalizedDescription(typeof(Resources.Language.SKD.PassCardLibrary.PassCardImagePropertyType), "PositionLogo")]
		PositionLogo,

        //[Description("Дополнительно")]
        [LocalizedDescription(typeof(Resources.Language.SKD.PassCardLibrary.PassCardImagePropertyType), "Additional")]
		Additional,
	}
}