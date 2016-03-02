using System.ComponentModel;

namespace FiresecAPI.Automation
{
	public enum ProcedureStepType
	{
		[DescriptionAttribute("Условие")]
		If,

		[DescriptionAttribute("Выполняется")]
		IfYes,

		[DescriptionAttribute("Не выполняется")]
		IfNo,

		[DescriptionAttribute("Цикл по списку")]
		Foreach,

		[DescriptionAttribute("Цикл со счетчиком (For)")]
		For,

		[DescriptionAttribute("Цикл с условием (While)")]
		While,

		[DescriptionAttribute("выйти из цикла")]
		Break,

		[DescriptionAttribute("продолжить цикл")]
		Continue,

		[DescriptionAttribute("Тело цикла")]
		ForeachBody,

		[DescriptionAttribute("Функция выбора процедуры")]
		ProcedureSelection,

		[DescriptionAttribute("Получить значение свойства объекта")]
		GetObjectProperty,

		[DescriptionAttribute("Проигрывание звука")]
		PlaySound,

		[DescriptionAttribute("Арифметическая операция")]
		Arithmetics,

		[DescriptionAttribute("Найти объекты")]
		FindObjects,

		[DescriptionAttribute("Изменение списка")]
		ChangeList,

		[DescriptionAttribute("Проверка прав")]
		CheckPermission,

		[DescriptionAttribute("Получить значение журнала")]
		GetJournalItem,

		[DescriptionAttribute("Получить размер списка")]
		GetListCount,

		[DescriptionAttribute("Получить элемент списка")]
		GetListItem,

		[DescriptionAttribute("Показать сообщение")]
		ShowMessage,

		[DescriptionAttribute("Добавить запись в журнал")]
		AddJournalItem,

		[DescriptionAttribute("Выход из процедуры")]
		Exit,

		[DescriptionAttribute("Задание значения переменной")]
		SetValue,

		[DescriptionAttribute("Запуск программы")]
		RunProgram,

		[DescriptionAttribute("Инкремент значения переменной")]
		IncrementValue,

		[DescriptionAttribute("Отправить сообщение по электронной почте")]
		SendEmail,

		[DescriptionAttribute("Пауза")]
		Pause,

		[DescriptionAttribute("Случайное значение")]
		Random,

		[DescriptionAttribute("Управление устройством СКД")]
		ControlSKDDevice,

		[DescriptionAttribute("Управление зоной СКД")]
		ControlSKDZone,

		[DescriptionAttribute("Управление точкой доступа")]
		ControlDoor,

		[DescriptionAttribute("Чтение свойства визуального элемента")]
		ControlVisualGet,

		[DescriptionAttribute("Установка свойства визуального элемента")]
		ControlVisualSet,

		[DescriptionAttribute("Чтение свойства элементами плана")]
		ControlPlanGet,

		[DescriptionAttribute("Установка свойства элементами плана")]
		ControlPlanSet,

		[DescriptionAttribute("Показать диалоговую форму")]
		ShowDialog,

		[DescriptionAttribute("Показать свойство объекта")]
		ShowProperty,

		[DescriptionAttribute("Генерировать идентификатор")]
		GenerateGuid,

		[DescriptionAttribute("Назначить идентификатор события")]
		SetJournalItemGuid,

		[DescriptionAttribute("")]
		DoAction,

		[DescriptionAttribute("Экспорт организации")]
		ExportOrganisation,

		[DescriptionAttribute("Экспорт списка организаций")]
		ExportOrganisationList,

		[DescriptionAttribute("Экспорт конфигурации")]
		ExportConfiguration,

		[DescriptionAttribute("Экспорт журнала")]
		ExportJournal,

		[DescriptionAttribute("Импорт организации")]
		ImportOrganisation,

		[DescriptionAttribute("Импорт списка организаций")]
		ImportOrganisationList,

		[DescriptionAttribute("Управление PTZ камерой")]
		Ptz,

		[DescriptionAttribute("Начать запись")]
		StartRecord,

		[DescriptionAttribute("Остановить запись")]
		StopRecord,

		[DescriptionAttribute("Вызвать тревогу в RVI Оператор")]
		RviAlarm,

		[Description("Экспорт отчета")]
		ExportReport,

		[Description("Получение свойства устройства СКД")]
		GetSkdDeviceProperty,

		[Description("Получение свойства точки доступа")]
		GetDoorProperty
	}
}