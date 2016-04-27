using System.ComponentModel;
using Localization;

namespace FiresecAPI.SKD
{
	public enum PersonType
	{
		//[DescriptionAttribute("Сотрудник")]
        [LocalizedDescription(typeof(Resources.Language.SKD.Employee.PersonType), "Employee")]
		Employee,

		//[DescriptionAttribute("Посетитель")]
        [LocalizedDescription(typeof(Resources.Language.SKD.Employee.PersonType), "Guest")]
        Guest
	}
}