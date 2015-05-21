using System.ComponentModel;

namespace PowerCalculator.Models
{
	public enum DriverType
	{
		[Description("Контроллер адресных устройств")]
		KAU,

		[Description("Извещатель пожарный дымовой")]
		IPD,

		[Description("Извещатель пожарный тепловой")]
		IPT,

		[Description("Извещатель пожарный комбинорованный")]
		IPK,

		[Description("Извещатель пожарный ручной")]
		IPR,

		[Description("Модуль дымоудаления")]
		MDU,

		[Description("Метка адресная - 4")]
		MA4,

		[Description("Метка адресная с питанием - 4")]
		MAP4,

		[Description("Модуль релейный - 2")]
		MR2,

		[Description("Модуль релейный - 4")]
		MR4,

		[Description("Модуль выходов с контрлоем")]
		MVK8,

		[Description("Модуль подпитки")]
		MP,

		[Description("Модуль ветвления и подпитки")]
		MVP,

		[Description("Оповещатель световой")]
		OPS,

		[Description("Оповещатель звуковой")]
		OPZ,

		[Description("Оповещатель комбинированный")]
		OPK
	}
}