using System.ComponentModel;

namespace StrazhAPI.Enums
{
	/// <summary>
	/// Рабочее окружение, заменяющее стандартный рабочий стол Windows
	/// </summary>
	public enum ShellType
	{
		/// <summary>
		/// Стандартный рабочий стол Windows
		/// </summary>
		[Description("Стандартный рабочий стол Windows")]
		Default = 0,
		
		/// <summary>
		/// Заменить рабочий стол Windows на ОЗ
		/// </summary>
		[Description("Оперативная задача")]
		Monitor = 1,

		/// <summary>
		/// Заменить рабочий стол Windows на ОЗ с макетами
		/// </summary>
		[Description("Оперативная задача с макетами")]
		Layouts = 2
	}
}