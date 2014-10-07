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

		[DescriptionAttribute("Цикл For")]
		For,

		[DescriptionAttribute("Цикл While")]
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
		RunProgramm,

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

		[DescriptionAttribute("Управление устройством СКД")]
		ControlSKDDevice,

		[DescriptionAttribute("Управление зоной СКД")]
		ControlSKDZone,

		[DescriptionAttribute("Управление видеоустройством")]
		ControlCamera,

		[DescriptionAttribute("Управление направлением")]
		ControlDirection,

		[DescriptionAttribute("Управление точкой доступа")]
		ControlDoor,

		[DescriptionAttribute("")]
		DoAction,
	}
}