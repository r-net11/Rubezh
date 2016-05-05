using System.ComponentModel;
using LocalizationConveters;

namespace FiresecAPI.Enums
{
	/// <summary>
	/// Рабочее окружение, заменяющее стандартный рабочий стол Windows
	/// </summary>
	public enum ShellType
	{
		/// <summary>
		/// Стандартный рабочий стол Windows
		/// </summary>
		//[Description("Стандартный рабочий стол Windows")]
        [LocalizedDescription(typeof(Resources.Language.Enums.ShellType), "Default")]
        Default = 0,
		
		/// <summary>
		/// Заменить рабочий стол Windows на ОЗ
		/// </summary>
        //[Description("Оперативная задача")]
        [LocalizedDescription(typeof(Resources.Language.Enums.ShellType), "Monitor")]
		Monitor = 1,

		/// <summary>
		/// Заменить рабочий стол Windows на ОЗ с макетами
		/// </summary>
        //[Description("Оперативная задача с макетами")]
        [LocalizedDescription(typeof(Resources.Language.Enums.ShellType), "Layouts")]
		Layouts = 2
	}
}