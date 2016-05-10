using System.ComponentModel;
using LocalizationConveters;

namespace StrazhAPI.SKD
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