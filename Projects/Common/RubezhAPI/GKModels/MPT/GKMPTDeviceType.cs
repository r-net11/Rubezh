using System.ComponentModel;

namespace RubezhAPI.GK
{
	/// <summary>
	/// Тип устройства МПТ
	/// </summary>
	public enum GKMPTDeviceType
	{
		[DescriptionAttribute("Табличка Не входи")]
		DoNotEnterBoard,

		[DescriptionAttribute("Табличка Уходи")]
		ExitBoard,

		[DescriptionAttribute("Табличка Автоматика отключена")]
		AutomaticOffBoard,

		[DescriptionAttribute("Сирена")]
		Speaker,

		[DescriptionAttribute("Ручной запуск")]
		HandStart,

		[DescriptionAttribute("Ручной останов")]
		HandStop,

		[DescriptionAttribute("Ручное включение автоматики")]
		HandAutomaticOn,

		[DescriptionAttribute("Ручное выключение автоматики")]
		HandAutomaticOff,

		[DescriptionAttribute("Пуск")]
		Bomb,

		[DescriptionAttribute("Неизвестно")]
		Unknown
	}
}