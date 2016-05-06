using System.ComponentModel;

namespace StrazhAPI.Enums
{
	public enum ModuleType
	{
		[Description("Администратор")]
		Administrator,

		[Description("Автоматизация")]
		Automation,

		[Description("Устройства, Зоны")]
		Devices,

		[Description("Фильтры журнала событий")]
		Filters,

		[Description("Конфигуратор макетов ОЗ")]
		Layout,

		[Description("Библиотека устройств")]
		Library,

		[Description("Уведомления")]
		Notification,

		[Description("Графические планы")]
		Plans,

		[Description("Права доступа")]
		Security,

		[Description("Настройки")]
		Settings,

		[Description("СКД")]
		SKD,

		[Description("Страж")]
		Strazh,

		[Description("Звуки")]
		Sounds,

		[Description("Видео")]
		Video,

		[Description("Cостояния")]
		Alarm,

		[Description("Журнал событий и Архив")]
		Journal,

		[Description("Отчёты")]
		Reports,

		[Description("Монитор")]
		Monitor,
	}
}
