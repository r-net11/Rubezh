﻿using System.ComponentModel;

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

		[DescriptionAttribute("Функция выбора процедур")]
		ProcedureSelection,

		[DescriptionAttribute("Получить значение поля объекта")]
		GetObjectField,

		[DescriptionAttribute("Проигрывание звука")]
		PlaySound,

		[DescriptionAttribute("Арифметическая операция")]
		Arithmetics,

		[DescriptionAttribute("Выборочный личный досмотр персонала")]
		PersonInspection,

		[DescriptionAttribute("Найти объекты")]
		FindObjects,

		[DescriptionAttribute("Показать сообщение")]
		ShowMessage,

		[DescriptionAttribute("Получить строку")]
		GetString,

		[DescriptionAttribute("Добавить запись в журнал")]
		AddJournalItem,

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