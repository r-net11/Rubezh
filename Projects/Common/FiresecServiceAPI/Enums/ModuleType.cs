using System.ComponentModel;
using LocalizationConveters;

namespace StrazhAPI.Enums
{
	public enum ModuleType
	{
		//[Description("Администратор")]
        [LocalizedDescription(typeof(Resources.Language.Enums.ModuleType), "Administrator")]
		Administrator,

		//[Description("Автоматизация")]
        [LocalizedDescription(typeof(Resources.Language.Enums.ModuleType), "Automation")]
        Automation,

		//[Description("Устройства, Зоны")]
        [LocalizedDescription(typeof(Resources.Language.Enums.ModuleType), "Devices")]
        Devices,

		//[Description("Фильтры журнала событий")]
        [LocalizedDescription(typeof(Resources.Language.Enums.ModuleType), "Filters")]
        Filters,

		//[Description("Конфигуратор макетов ОЗ")]
        [LocalizedDescription(typeof(Resources.Language.Enums.ModuleType), "Layout")]
        Layout,

		//[Description("Библиотека устройств")]
        [LocalizedDescription(typeof(Resources.Language.Enums.ModuleType), "Library")]
        Library,

		//[Description("Уведомления")]
        [LocalizedDescription(typeof(Resources.Language.Enums.ModuleType), "Notification")]
        Notification,

		//[Description("Графические планы")]
        [LocalizedDescription(typeof(Resources.Language.Enums.ModuleType), "Plans")]
        Plans,

		//[Description("Права доступа")]
        [LocalizedDescription(typeof(Resources.Language.Enums.ModuleType), "Security")]
        Security,
        
		//[Description("Настройки")]
        [LocalizedDescription(typeof(Resources.Language.Enums.ModuleType), "Settings")]
        Settings,

		//[Description("СКД")]
        [LocalizedDescription(typeof(Resources.Language.Enums.ModuleType), "SKD")]
        SKD,

		//[Description("Страж")]
        [LocalizedDescription(typeof(Resources.Language.Enums.ModuleType), "Strazh")]
        Strazh,

		//[Description("Звуки")]
        [LocalizedDescription(typeof(Resources.Language.Enums.ModuleType), "Sounds")]
        Sounds,

		//[Description("Видео")]
        [LocalizedDescription(typeof(Resources.Language.Enums.ModuleType), "Video")]
        Video,

		//[Description("Состояния")]
        [LocalizedDescription(typeof(Resources.Language.Enums.ModuleType), "Alarm")]
        Alarm,

		//[Description("Журнал событий и Архив")]
        [LocalizedDescription(typeof(Resources.Language.Enums.ModuleType), "Journal")]
        Journal,

		//[Description("Отчёты")]
        [LocalizedDescription(typeof(Resources.Language.Enums.ModuleType), "Reports")]
        Reports,

		//[Description("Монитор")]
        [LocalizedDescription(typeof(Resources.Language.Enums.ModuleType), "Monitor")]
        Monitor,
	}
}
