using System.ComponentModel;

namespace RubezhAPI.Automation
{
	public enum ProcedureStepType
	{
		[DescriptionAttribute("Пусто")]
		Null,

		[DescriptionAttribute("Условие")]
		If,

		[DescriptionAttribute("Выполняется")]
		IfYes,

		[DescriptionAttribute("Не выполняется")]
		IfNo,

		[DescriptionAttribute("Цикл по списку")]
		Foreach,

		[DescriptionAttribute("Цикл Для")]
		For,

		[DescriptionAttribute("Цикл Пока")]
		While,

		[DescriptionAttribute("Выйти из цикла")]
		Break,

		[DescriptionAttribute("Продолжить цикл")]
		Continue,

		[DescriptionAttribute("Тело цикла")]
		ForeachBody,

		[DescriptionAttribute("Вызов процедуры")]
		ProcedureSelection,

		[DescriptionAttribute("Получить значение свойства объекта")]
		GetObjectProperty,

		[DescriptionAttribute("Проигрывание звука")]
		PlaySound,

		[DescriptionAttribute("Арифметическая операция")]
		Arithmetics,

		[DescriptionAttribute("Создать цвет")]
		CreateColor,

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

		[DescriptionAttribute("Управление устройством ГК")]
		ControlGKDevice,

		[DescriptionAttribute("Управление пожарной зоной")]
		ControlGKFireZone,

		[DescriptionAttribute("Управление охраной зоной")]
		ControlGKGuardZone,

		[DescriptionAttribute("Управление направлением")]
		ControlDirection,

		[DescriptionAttribute("Управление точкой доступа")]
		ControlGKDoor,

		[DescriptionAttribute("Управление задержкой")]
		ControlDelay,

		[DescriptionAttribute("Управление насосной станцией")]
		ControlPumpStation,

		[DescriptionAttribute("Управление МПТ")]
		ControlMPT,

		[DescriptionAttribute("Чтение свойства визуального элемента")]
		ControlVisualGet,

		[DescriptionAttribute("Установка свойства визуального элемента")]
		ControlVisualSet,

		[DescriptionAttribute("Чтение свойства элемента плана")]
		ControlPlanGet,

		[DescriptionAttribute("Установка свойства элемента плана")]
		ControlPlanSet,

		[DescriptionAttribute("Показать диалоговую форму")]
		ShowDialog,

		[DescriptionAttribute("Закрыть диалоговую форму")]
		CloseDialog,

		[DescriptionAttribute("Показать свойства объекта")]
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

		[DescriptionAttribute("Ptz камеры")]
		Ptz,

		[DescriptionAttribute("Начать запись")]
		StartRecord,

		[DescriptionAttribute("Остановить запись")]
		StopRecord,

		[DescriptionAttribute("Вызвать тревогу в Rvi Оператор")]
		RviAlarm,

		[DescriptionAttribute("Показать раскладку в Rvi Оператор")]
		RviOpenWindow,

		[DescriptionAttribute("Получить текущие дату и время")]
		Now,

		[DescriptionAttribute("HTTP запрос")]
		HttpRequest,

		[DescriptionAttribute("Чтение значения OPC DA Тэга")]
		ControlOpcDaTagGet,

		[DescriptionAttribute("Установка значения OPC DA Тэга")]
		ControlOpcDaTagSet,
	}
}