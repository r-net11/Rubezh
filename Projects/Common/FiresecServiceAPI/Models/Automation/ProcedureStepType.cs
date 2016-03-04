using System.ComponentModel;

namespace FiresecAPI.Automation
{
	public enum ProcedureStepType
	{
		[Description("Условие")]
		If,

		[Description("Выполняется")]
		IfYes,

		[Description("Не выполняется")]
		IfNo,

		[Description("Цикл по списку")]
		Foreach,

		[Description("Цикл со счетчиком (For)")]
		For,

		[Description("Цикл с условием (While)")]
		While,

		[Description("Выйти из цикла")]
		Break,

		[Description("Продолжить цикл")]
		Continue,

		[Description("Тело цикла")]
		ForeachBody,

		[Description("Функция выбора процедуры")]
		ProcedureSelection,

		[Description("Получить значение свойства объекта")]
		GetObjectProperty,

		[Description("Проигрывание звука")]
		PlaySound,

		[Description("Арифметическая операция")]
		Arithmetics,

		[Description("Найти объекты")]
		FindObjects,

		[Description("Изменение списка")]
		ChangeList,

		[Description("Проверка прав")]
		CheckPermission,

		[Description("Получить значение журнала")]
		GetJournalItem,

		[Description("Получить размер списка")]
		GetListCount,

		[Description("Получить элемент списка")]
		GetListItem,

		[Description("Показать сообщение")]
		ShowMessage,

		[Description("Добавить запись в журнал")]
		AddJournalItem,

		[Description("Выход из процедуры")]
		Exit,

		[Description("Задание значения переменной")]
		SetValue,

		[Description("Запуск программы")]
		RunProgram,

		[Description("Инкремент значения переменной")]
		IncrementValue,

		[Description("Отправить сообщение по электронной почте")]
		SendEmail,

		[Description("Пауза")]
		Pause,

		[Description("Случайное значение")]
		Random,

		[Description("Управление устройством СКД")]
		ControlSKDDevice,

		[Description("Управление зоной СКД")]
		ControlSKDZone,

		[Description("Управление точкой доступа")]
		ControlDoor,

		[Description("Чтение свойства визуального элемента")]
		ControlVisualGet,

		[Description("Установка свойства визуального элемента")]
		ControlVisualSet,

		[Description("Чтение свойства элементами плана")]
		ControlPlanGet,

		[Description("Установка свойства элементами плана")]
		ControlPlanSet,

		[Description("Показать диалоговую форму")]
		ShowDialog,

		[Description("Показать свойство объекта")]
		ShowProperty,

		[Description("Генерировать идентификатор")]
		GenerateGuid,

		[Description("Назначить идентификатор события")]
		SetJournalItemGuid,

		[Description("")]
		DoAction,

		[Description("Экспорт организации")]
		ExportOrganisation,

		[Description("Экспорт списка организаций")]
		ExportOrganisationList,

		[Description("Экспорт конфигурации")]
		ExportConfiguration,

		[Description("Экспорт журнала")]
		ExportJournal,

		[Description("Импорт организации")]
		ImportOrganisation,

		[Description("Импорт списка организаций")]
		ImportOrganisationList,

		[Description("Управление PTZ камерой")]
		Ptz,

		[Description("Начать запись")]
		StartRecord,

		[Description("Остановить запись")]
		StopRecord,

		[Description("Вызвать тревогу в RVI Оператор")]
		RviAlarm,

		[Description("Экспорт отчета")]
		ExportReport,

		[Description("Получить текущее время")]
		GetDateTimeNow,

		[Description("Получение свойства устройства СКД")]
		GetSkdDeviceProperty,

		[Description("Получение свойства точки доступа")]
		GetDoorProperty,

		[Description("Получение свойства зоны СКД")]
		GetSkdZoneProperty
	}
}