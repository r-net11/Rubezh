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

		[DescriptionAttribute("Тело цикла")]
		ForeachBody,

		[DescriptionAttribute("Список")]
		ForeachList,

		[DescriptionAttribute("Текущий элемент списка")]
		ForeachElement,

		[DescriptionAttribute("Функция выбора процедур")]
		ProcedureSelection,

		[DescriptionAttribute("Изменить значение поля объекта")]
		SetObjectField,

		[DescriptionAttribute("Получить значение поля объекта")]
		GetObjectField,

		[DescriptionAttribute("Проигрывание звуков")]
		PlaySound,

		[DescriptionAttribute("Арифметическая операция")]
		Arithmetics,

		[DescriptionAttribute("Выборочный личный досмотр персонала")]
		PersonInspection,

		[DescriptionAttribute("Найти объекты")]
		FindObjects,

		[DescriptionAttribute("Отправить сообщение")]
		SendMessage,

		[DescriptionAttribute("Экспорт отчета")]
		ReportExport,

		[DescriptionAttribute("Выход из процедуры")]
		Exit,

		[DescriptionAttribute("Задание значения глобальной переменной")]
		SetGlobalValue,

		[DescriptionAttribute("Запуск программ")]
		RunProgramm,

		[DescriptionAttribute("Инкремент значения глобальной переменной")]
		IncrementGlobalValue,

		[DescriptionAttribute("Отправить сообщение по электронной почте")]
		SendEmail,

		[DescriptionAttribute("Пауза")]
		Pause,

		[DescriptionAttribute("Послать отладочное сообщение")]
		SendDebugMessage,

		[DescriptionAttribute("Управление устройством ГК")]
		ControlGKDevice,

		[DescriptionAttribute("Управление пожарной зоной")]
		ControlGKFireZone,

		[DescriptionAttribute("Управление охраной зоной")]
		ControlGKGuardZone,

		[DescriptionAttribute("Управление устройством СКД")]
		ControlSKDGKDevice,

		[DescriptionAttribute("Управление зоной СКД")]
		ControlSKDZone,

		[DescriptionAttribute("Управление камерой")]
		ControlCamera,

		[DescriptionAttribute("Управление направлением")]
		ControlDirection,

		[DescriptionAttribute("Управление точкой доступа")]
		ControlDoor,

		[DescriptionAttribute("")]
		DoAction,
	}
}