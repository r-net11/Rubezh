using System.ComponentModel;

namespace StrazhAPI.SKD
{
	public enum PersonType
	{
		[Description("Сотрудник")]
		Employee,

		[Description("Посетитель")]
		Guest
	}
}