using System.ComponentModel;

namespace FiresecAPI.GK
{
	public enum EventDescription
	{
		[DescriptionAttribute("Подтверждение тревоги")]
		Подтверждение_тревоги,

		[DescriptionAttribute("Остановка пуска")]
		Остановка_пуска,

		[DescriptionAttribute("Выключить немедленно")]
		Выключить_немедленно,

		[DescriptionAttribute("Выключить")]
		Выключить,

		[DescriptionAttribute("Включить немедленно")]
		Включить_немедленно,

		[DescriptionAttribute("Включить")]
		Включить,

		[DescriptionAttribute("Перевод в ручной режим")]
		Перевод_в_ручной_режим,

		[DescriptionAttribute("Перевод в автоматический режим")]
		Перевод_в_автоматический_режим,

		[DescriptionAttribute("Перевод в отключенный режим")]
		Перевод_в_отключенный_режим,

		[DescriptionAttribute("Сброс")]
		Сброс,

		[DescriptionAttribute("Не найдено родительское устройство ГК")]
		Не_найдено_родительское_устройство_ГК,

		[DescriptionAttribute("Старт мониторинга")]
		Старт_мониторинга,

		[DescriptionAttribute("Не совпадает хэш")]
		Не_совпадает_хэш,

		[DescriptionAttribute("Совпадает хэш")]
		Совпадает_хэш,

		[DescriptionAttribute("Не совпадает количество байт в пришедшем ответе")]
		Не_совпадает_количество_байт_в_пришедшем_ответе,

		[DescriptionAttribute("Не совпадает тип устройства")]
		Не_совпадает_тип_устройства,

		[DescriptionAttribute("Не совпадает физический адрес устройства")]
		Не_совпадает_физический_адрес_устройства,

		[DescriptionAttribute("Не совпадает адрес на контроллере")]
		Не_совпадает_адрес_на_контроллере,

		[DescriptionAttribute("Не совпадает тип для зоны")]
		Не_совпадает_тип_для_зоны,

		[DescriptionAttribute("Не совпадает тип для направления")]
		Не_совпадает_тип_для_направления,

		[DescriptionAttribute("Не совпадает тип для НС")]
		Не_совпадает_тип_для_НС,

		[DescriptionAttribute("Не совпадает тип для МПТ")]
		Не_совпадает_тип_для_МПТ,

		[DescriptionAttribute("Не совпадает тип для Задержки")]
		Не_совпадает_тип_для_Задержки,

		[DescriptionAttribute("Не совпадает тип для ПИМ")]
		Не_совпадает_тип_для_ПИМ,

		[DescriptionAttribute("Не совпадает описание компонента")]
		Не_совпадает_описание_компонента,

		[DescriptionAttribute("")]
		Нет,

		[DescriptionAttribute("Установить режим ОТКРЫТО")]
		Установить_режим_ОТКРЫТО,

		[DescriptionAttribute("Установить режим ЗАКРЫТО")]
		Установить_режим_ЗАКРЫТО,

		[DescriptionAttribute("Установить режим КОНТРОЛЬ")]
		Установить_режим_КОНТРОЛЬ,

		[DescriptionAttribute("Установить режим СОВЕЩАНИЕ")]
		Установить_режим_СОВЕЩАНИЕ,

		[DescriptionAttribute("Открыть")]
		Открыть,
		
		[DescriptionAttribute("Закрыть")]
		Закрыть,
		
		[DescriptionAttribute("Разрешить проход")]
		Разрешить_проход,
		
		[DescriptionAttribute("Запретить проход")]
		Запретить_проход,	
	
		[DescriptionAttribute("Запись_одного_идентификатора")]
		Запись_одного_идентификатора,
		
		[DescriptionAttribute("Запись_всех_временных_интервалов")]
		Запись_всех_временных_интервалов,	
	}
}