using System.ComponentModel;

namespace FiresecAPI.Journal
{
	public enum JournalEventDescriptionType
	{
		[EventDescriptionAttribute("")]
		NULL,

		[EventDescriptionAttribute("Остановка пуска", JournalEventNameType.Команда_оператора)]
		Остановка_пуска,

		[EventDescriptionAttribute("Выключить немедленно", JournalEventNameType.Команда_оператора)]
		Выключить_немедленно,

		[EventDescriptionAttribute("Выключить", JournalEventNameType.Команда_оператора)]
		Выключить,

		[EventDescriptionAttribute("Включить немедленно", JournalEventNameType.Команда_оператора)]
		Включить_немедленно,

		[EventDescriptionAttribute("Включить", JournalEventNameType.Команда_оператора)]
		Включить,

		[EventDescriptionAttribute("Перевод в ручной режим", JournalEventNameType.Команда_оператора)]
		Перевод_в_ручной_режим,

		[EventDescriptionAttribute("Перевод в автоматический режим", JournalEventNameType.Команда_оператора)]
		Перевод_в_автоматический_режим,

		[EventDescriptionAttribute("Перевод в отключенный режим", JournalEventNameType.Команда_оператора)]
		Перевод_в_отключенный_режим,

		[EventDescriptionAttribute("Сброс", JournalEventNameType.Команда_оператора)]
		Сброс,

		[EventDescriptionAttribute("Не найдено родительское устройство ГК", JournalEventNameType.Ошибка_при_опросе_состояний_компонентов_ГК)]
		Не_найдено_родительское_устройство_ГК,

		[EventDescriptionAttribute("Старт мониторинга", JournalEventNameType.Ошибка_при_опросе_состояний_компонентов_ГК)]
		Старт_мониторинга,

		[EventDescriptionAttribute("Не совпадает хэш", JournalEventNameType.Ошибка_при_опросе_состояний_компонентов_ГК)]
		Не_совпадает_хэш,

		[EventDescriptionAttribute("Совпадает хэш", JournalEventNameType.Ошибка_при_опросе_состояний_компонентов_ГК)]
		Совпадает_хэш,

		[EventDescriptionAttribute("Не совпадает количество байт в пришедшем ответе", JournalEventNameType.Ошибка_при_опросе_состояний_компонентов_ГК)]
		Не_совпадает_количество_байт_в_пришедшем_ответе,

		[EventDescriptionAttribute("Не совпадает тип устройства", JournalEventNameType.Ошибка_при_опросе_состояний_компонентов_ГК)]
		Не_совпадает_тип_устройства,

		[EventDescriptionAttribute("Не совпадает физический адрес устройства", JournalEventNameType.Ошибка_при_опросе_состояний_компонентов_ГК)]
		Не_совпадает_физический_адрес_устройства,

		[EventDescriptionAttribute("Не совпадает адрес на контроллере", JournalEventNameType.Ошибка_при_опросе_состояний_компонентов_ГК)]
		Не_совпадает_адрес_на_контроллере,

		[EventDescriptionAttribute("Не совпадает тип для зоны", JournalEventNameType.Ошибка_при_опросе_состояний_компонентов_ГК)]
		Не_совпадает_тип_для_зоны,

		[EventDescriptionAttribute("Не совпадает тип для направления", JournalEventNameType.Ошибка_при_опросе_состояний_компонентов_ГК)]
		Не_совпадает_тип_для_направления,

		[EventDescriptionAttribute("Не совпадает тип для НС", JournalEventNameType.Ошибка_при_опросе_состояний_компонентов_ГК)]
		Не_совпадает_тип_для_НС,

		[EventDescriptionAttribute("Не совпадает тип для МПТ", JournalEventNameType.Ошибка_при_опросе_состояний_компонентов_ГК)]
		Не_совпадает_тип_для_МПТ,

		[EventDescriptionAttribute("Не совпадает тип для Задержки", JournalEventNameType.Ошибка_при_опросе_состояний_компонентов_ГК)]
		Не_совпадает_тип_для_Задержки,

		[EventDescriptionAttribute("Не совпадает тип для ПИМ", JournalEventNameType.Ошибка_при_опросе_состояний_компонентов_ГК)]
		Не_совпадает_тип_для_ПИМ,

		[EventDescriptionAttribute("Не совпадает тип для охранной зоны", JournalEventNameType.Ошибка_при_опросе_состояний_компонентов_ГК)]
		Не_совпадает_тип_для_охранной_зоны,

		[EventDescriptionAttribute("Не совпадает тип для кода", JournalEventNameType.Ошибка_при_опросе_состояний_компонентов_ГК)]
		Не_совпадает_тип_для_кода,

		[EventDescriptionAttribute("Не совпадает описание компонента", JournalEventNameType.Ошибка_при_опросе_состояний_компонентов_ГК)]
		Не_совпадает_описание_компонента,

		[EventDescriptionAttribute("Ручник сорван", JournalEventNameType.Сработка_1, JournalEventNameType.Сработка_2)]
		Ручник_сорван,

		[EventDescriptionAttribute("Срабатывание по дыму", JournalEventNameType.Сработка_1, JournalEventNameType.Сработка_2)]
		Срабатывание_по_дыму,

		[EventDescriptionAttribute("Срабатывание по температуре", JournalEventNameType.Сработка_1, JournalEventNameType.Сработка_2)]
		Срабатывание_по_температуре,

		[EventDescriptionAttribute("Срабатывание по градиенту температуры", JournalEventNameType.Сработка_1, JournalEventNameType.Сработка_2)]
		Срабатывание_по_градиенту_температуры,


		[EventDescriptionAttribute("Напряжение питания устройства не в норме", JournalEventNameType.Неисправность, JournalEventNameType.Неисправность_устранена)]
		Напряжение_питания_устройства_не_в_норме,

		[EventDescriptionAttribute("Оптический канал или фотоусилитель", JournalEventNameType.Неисправность, JournalEventNameType.Неисправность_устранена)]
		Оптический_канал_или_фотоусилитель,

		[EventDescriptionAttribute("Температурный канал", JournalEventNameType.Неисправность, JournalEventNameType.Неисправность_устранена)]
		Температурный_канал,

		[EventDescriptionAttribute("КЗ ШС", JournalEventNameType.Неисправность, JournalEventNameType.Неисправность_устранена)]
		КЗ_ШС,

		[EventDescriptionAttribute("Обрыв ШС", JournalEventNameType.Неисправность, JournalEventNameType.Неисправность_устранена)]
		Обрыв_ШС,

		[EventDescriptionAttribute("Вскрытие корпуса", JournalEventNameType.Неисправность, JournalEventNameType.Неисправность_устранена)]
		Вскрытие_корпуса,

		[EventDescriptionAttribute("Контакт не переключается", JournalEventNameType.Неисправность, JournalEventNameType.Неисправность_устранена)]
		Контакт_не_переключается,

		[EventDescriptionAttribute("Напряжение запуска реле ниже нормы", JournalEventNameType.Неисправность, JournalEventNameType.Неисправность_устранена)]
		Напряжение_запуска_реле_ниже_нормы,

		[EventDescriptionAttribute("КЗ выхода", JournalEventNameType.Неисправность, JournalEventNameType.Неисправность_устранена)]
		КЗ_выхода,

		[EventDescriptionAttribute("Обрыв выхода", JournalEventNameType.Неисправность, JournalEventNameType.Неисправность_устранена)]
		Обрыв_выхода,

		[EventDescriptionAttribute("Напряжение питания ШС ниже нормы", JournalEventNameType.Неисправность, JournalEventNameType.Неисправность_устранена)]
		Напряжение_питания_ШС_ниже_нормы,

		[EventDescriptionAttribute("Ошибка памяти", JournalEventNameType.Неисправность, JournalEventNameType.Неисправность_устранена)]
		Ошибка_памяти,

		[EventDescriptionAttribute("КЗ выхода 1", JournalEventNameType.Неисправность, JournalEventNameType.Неисправность_устранена)]
		КЗ_выхода_1,

		[EventDescriptionAttribute("КЗ выхода 2", JournalEventNameType.Неисправность, JournalEventNameType.Неисправность_устранена)]
		КЗ_выхода_2,

		[EventDescriptionAttribute("КЗ выхода 3", JournalEventNameType.Неисправность, JournalEventNameType.Неисправность_устранена)]
		КЗ_выхода_3,

		[EventDescriptionAttribute("КЗ выхода 4", JournalEventNameType.Неисправность, JournalEventNameType.Неисправность_устранена)]
		КЗ_выхода_4,

		[EventDescriptionAttribute("КЗ выхода 5", JournalEventNameType.Неисправность, JournalEventNameType.Неисправность_устранена)]
		КЗ_выхода_5,

		[EventDescriptionAttribute("Обрыв выхода 1", JournalEventNameType.Неисправность, JournalEventNameType.Неисправность_устранена)]
		Обрыв_выхода_1,

		[EventDescriptionAttribute("Обрыв выхода 2", JournalEventNameType.Неисправность, JournalEventNameType.Неисправность_устранена)]
		Обрыв_выхода_2,

		[EventDescriptionAttribute("Обрыв выхода 3", JournalEventNameType.Неисправность, JournalEventNameType.Неисправность_устранена)]
		Обрыв_выхода_3,

		[EventDescriptionAttribute("Обрыв выхода 4", JournalEventNameType.Неисправность, JournalEventNameType.Неисправность_устранена)]
		Обрыв_выхода_4,

		[EventDescriptionAttribute("Обрыв выхода 5", JournalEventNameType.Неисправность, JournalEventNameType.Неисправность_устранена)]
		Обрыв_выхода_5,

		[EventDescriptionAttribute("Блокировка пуска", JournalEventNameType.Неисправность, JournalEventNameType.Неисправность_устранена)]
		Блокировка_пуска,

		[EventDescriptionAttribute("Низкое напряжение питания привода", JournalEventNameType.Неисправность, JournalEventNameType.Неисправность_устранена)]
		Низкое_напряжение_питания_привода,

		[EventDescriptionAttribute("Обрыв кнопки НОРМА", JournalEventNameType.Неисправность, JournalEventNameType.Неисправность_устранена)]
		Обрыв_кнопки_НОРМА,

		[EventDescriptionAttribute("КЗ кнопки НОРМА", JournalEventNameType.Неисправность, JournalEventNameType.Неисправность_устранена)]
		КЗ_кнопки_НОРМА,

		[EventDescriptionAttribute("Обрыв кнопка ЗАЩИТА", JournalEventNameType.Неисправность, JournalEventNameType.Неисправность_устранена)]
		Обрыв_кнопка_ЗАЩИТА,

		[EventDescriptionAttribute("КЗ кнопки ЗАЩИТА", JournalEventNameType.Неисправность, JournalEventNameType.Неисправность_устранена)]
		КЗ_кнопки_ЗАЩИТА,

		[EventDescriptionAttribute("Обрыв концевого выключателя ОТКРЫТО", JournalEventNameType.Неисправность, JournalEventNameType.Неисправность_устранена)]
		Обрыв_концевого_выключателя_ОТКРЫТО,

		[EventDescriptionAttribute("Обрыв концевого выключателя ЗАКРЫТО", JournalEventNameType.Неисправность, JournalEventNameType.Неисправность_устранена)]
		Обрыв_концевого_выключателя_ЗАКРЫТО,

		[EventDescriptionAttribute("Обрыв цепи 1 ДВИГАТЕЛЯ", JournalEventNameType.Неисправность, JournalEventNameType.Неисправность_устранена)]
		Обрыв_цепи_1_ДВИГАТЕЛЯ,

		[EventDescriptionAttribute("Замкнуты/разомкнуты оба концевика", JournalEventNameType.Неисправность, JournalEventNameType.Неисправность_устранена)]
		Замкнуты_разомкнуты_оба_концевика,

		[EventDescriptionAttribute("Превышение времени хода", JournalEventNameType.Неисправность, JournalEventNameType.Неисправность_устранена)]
		Превышение_времени_хода,

		[EventDescriptionAttribute("Обрыв в линии РЕЛЕ", JournalEventNameType.Неисправность, JournalEventNameType.Неисправность_устранена)]
		Обрыв_в_линии_РЕЛЕ,

		[EventDescriptionAttribute("КЗ в линии РЕЛЕ", JournalEventNameType.Неисправность, JournalEventNameType.Неисправность_устранена)]
		КЗ_в_линии_РЕЛЕ,

		[EventDescriptionAttribute("Выход 1", JournalEventNameType.Неисправность, JournalEventNameType.Неисправность_устранена)]
		Выход_1,

		[EventDescriptionAttribute("Выход 2", JournalEventNameType.Неисправность, JournalEventNameType.Неисправность_устранена)]
		Выход_2,

		[EventDescriptionAttribute("Выход 3", JournalEventNameType.Неисправность, JournalEventNameType.Неисправность_устранена)]
		Выход_3,

		[EventDescriptionAttribute("КЗ концевого выключателя ОТКРЫТО", JournalEventNameType.Неисправность, JournalEventNameType.Неисправность_устранена)]
		КЗ_концевого_выключателя_ОТКРЫТО,

		[EventDescriptionAttribute("Обрыв муфтового выключателя ОТКРЫТО", JournalEventNameType.Неисправность, JournalEventNameType.Неисправность_устранена)]
		Обрыв_муфтового_выключателя_ОТКРЫТО,

		[EventDescriptionAttribute("КЗ муфтового выключателя ОТКРЫТО", JournalEventNameType.Неисправность, JournalEventNameType.Неисправность_устранена)]
		КЗ_муфтового_выключателя_ОТКРЫТО,

		[EventDescriptionAttribute("КЗ концевого выключателя ЗАКРЫТО", JournalEventNameType.Неисправность, JournalEventNameType.Неисправность_устранена)]
		КЗ_концевого_выключателя_ЗАКРЫТО,

		[EventDescriptionAttribute("Обрыв муфтового выключателя ЗАКРЫТО", JournalEventNameType.Неисправность, JournalEventNameType.Неисправность_устранена)]
		Обрыв_муфтового_выключателя_ЗАКРЫТО,

		[EventDescriptionAttribute("КЗ муфтового выключателя ЗАКРЫТО", JournalEventNameType.Неисправность, JournalEventNameType.Неисправность_устранена)]
		КЗ_муфтового_выключателя_ЗАКРЫТО,

		[EventDescriptionAttribute("Обрыв кнопки Открыть УЗЗ/Закрыть УЗЗ", JournalEventNameType.Неисправность, JournalEventNameType.Неисправность_устранена)]
		Обрыв_кнопки_Открыть_УЗЗ_Закрыть_УЗЗ,

		[EventDescriptionAttribute("КЗ кнопки Открыть УЗЗ/Закрыть УЗЗ", JournalEventNameType.Неисправность, JournalEventNameType.Неисправность_устранена)]
		КЗ_кнопки_Открыть_УЗЗ_Закрыть_УЗЗ,

		[EventDescriptionAttribute("Обрыв кнопки СТОП УЗЗ", JournalEventNameType.Неисправность, JournalEventNameType.Неисправность_устранена)]
		Обрыв_кнопки_СТОП_УЗЗ,

		[EventDescriptionAttribute("КЗ кнопки СТОП УЗЗ", JournalEventNameType.Неисправность, JournalEventNameType.Неисправность_устранена)]
		КЗ_кнопки_СТОП_УЗЗ,

		[EventDescriptionAttribute("Обрыв давление низкое", JournalEventNameType.Неисправность, JournalEventNameType.Неисправность_устранена)]
		Обрыв_давление_низкое,

		[EventDescriptionAttribute("КЗ давление низкое", JournalEventNameType.Неисправность, JournalEventNameType.Неисправность_устранена)]
		КЗ_давление_низкое,

		[EventDescriptionAttribute("Таймаут по давлению", JournalEventNameType.Неисправность, JournalEventNameType.Неисправность_устранена)]
		Таймаут_по_давлению,

		[EventDescriptionAttribute("КВ/МВ", JournalEventNameType.Неисправность, JournalEventNameType.Неисправность_устранена)]
		КВ_МВ,

		[EventDescriptionAttribute("Не задан режим", JournalEventNameType.Неисправность, JournalEventNameType.Неисправность_устранена)]
		Не_задан_режим,

		[EventDescriptionAttribute("Отказ ШУЗ", JournalEventNameType.Неисправность, JournalEventNameType.Неисправность_устранена)]
		Отказ_ШУЗ,

		[EventDescriptionAttribute("ДУ/ДД", JournalEventNameType.Неисправность, JournalEventNameType.Неисправность_устранена)]
		ДУ_ДД,

		[EventDescriptionAttribute("Обрыв входа 1", JournalEventNameType.Неисправность, JournalEventNameType.Неисправность_устранена)]
		Обрыв_входа_1,

		[EventDescriptionAttribute("КЗ входа 1", JournalEventNameType.Неисправность, JournalEventNameType.Неисправность_устранена)]
		КЗ_входа_1,

		[EventDescriptionAttribute("Обрыв входа 2", JournalEventNameType.Неисправность, JournalEventNameType.Неисправность_устранена)]
		Обрыв_входа_2,

		[EventDescriptionAttribute("КЗ входа 2", JournalEventNameType.Неисправность, JournalEventNameType.Неисправность_устранена)]
		КЗ_входа_2,

		[EventDescriptionAttribute("Обрыв входа 3", JournalEventNameType.Неисправность, JournalEventNameType.Неисправность_устранена)]
		Обрыв_входа_3,

		[EventDescriptionAttribute("КЗ входа 3", JournalEventNameType.Неисправность, JournalEventNameType.Неисправность_устранена)]
		КЗ_входа_3,

		[EventDescriptionAttribute("Обрыв входа 4", JournalEventNameType.Неисправность, JournalEventNameType.Неисправность_устранена)]
		Обрыв_входа_4,

		[EventDescriptionAttribute("КЗ входа 4", JournalEventNameType.Неисправность, JournalEventNameType.Неисправность_устранена)]
		КЗ_входа_4,

		[EventDescriptionAttribute("Не задан тип", JournalEventNameType.Неисправность, JournalEventNameType.Неисправность_устранена)]
		Не_задан_тип,

		[EventDescriptionAttribute("Отказ ПН", JournalEventNameType.Неисправность, JournalEventNameType.Неисправность_устранена)]
		Отказ_ПН,

		[EventDescriptionAttribute("Отказ ШУН", JournalEventNameType.Неисправность, JournalEventNameType.Неисправность_устранена)]
		Отказ_ШУН,

		[EventDescriptionAttribute("Питание 1", JournalEventNameType.Неисправность, JournalEventNameType.Неисправность_устранена)]
		Питание_1,

		[EventDescriptionAttribute("Питание 2", JournalEventNameType.Неисправность, JournalEventNameType.Неисправность_устранена)]
		Питание_2,

		[EventDescriptionAttribute("Отказ АЛС 1 или 2", JournalEventNameType.Неисправность, JournalEventNameType.Неисправность_устранена)]
		Отказ_АЛС_1_или_2,

		[EventDescriptionAttribute("Отказ АЛС 3 или 4", JournalEventNameType.Неисправность, JournalEventNameType.Неисправность_устранена)]
		Отказ_АЛС_3_или_4,

		[EventDescriptionAttribute("Отказ АЛС 5 или 6", JournalEventNameType.Неисправность, JournalEventNameType.Неисправность_устранена)]
		Отказ_АЛС_5_или_6,

		[EventDescriptionAttribute("Отказ АЛС 7 или 8", JournalEventNameType.Неисправность, JournalEventNameType.Неисправность_устранена)]
		Отказ_АЛС_7_или_8,

		[EventDescriptionAttribute("Обрыв цепи 2 ДВИГАТЕЛЯ", JournalEventNameType.Неисправность, JournalEventNameType.Неисправность_устранена)]
		Обрыв_цепи_2_ДВИГАТЕЛЯ,

		[EventDescriptionAttribute("КЗ АЛС 1", JournalEventNameType.Неисправность, JournalEventNameType.Неисправность_устранена)]
		КЗ_АЛС_1,

		[EventDescriptionAttribute("КЗ АЛС 2", JournalEventNameType.Неисправность, JournalEventNameType.Неисправность_устранена)]
		КЗ_АЛС_2,

		[EventDescriptionAttribute("КЗ АЛС 3", JournalEventNameType.Неисправность, JournalEventNameType.Неисправность_устранена)]
		КЗ_АЛС_3,

		[EventDescriptionAttribute("КЗ АЛС 4", JournalEventNameType.Неисправность, JournalEventNameType.Неисправность_устранена)]
		КЗ_АЛС_4,

		[EventDescriptionAttribute("КЗ АЛС 5", JournalEventNameType.Неисправность, JournalEventNameType.Неисправность_устранена)]
		КЗ_АЛС_5,

		[EventDescriptionAttribute("КЗ АЛС 6", JournalEventNameType.Неисправность, JournalEventNameType.Неисправность_устранена)]
		КЗ_АЛС_6,

		[EventDescriptionAttribute("КЗ АЛС 7", JournalEventNameType.Неисправность, JournalEventNameType.Неисправность_устранена)]
		КЗ_АЛС_7,

		[EventDescriptionAttribute("КЗ АЛС 8", JournalEventNameType.Неисправность, JournalEventNameType.Неисправность_устранена)]
		КЗ_АЛС_8,

		[EventDescriptionAttribute("Истекло время вкл", JournalEventNameType.Неисправность, JournalEventNameType.Неисправность_устранена)]
		Истекло_время_вкл,

		[EventDescriptionAttribute("Истекло время выкл", JournalEventNameType.Неисправность, JournalEventNameType.Неисправность_устранена)]
		Истекло_время_выкл,

		[EventDescriptionAttribute("Контакт реле 1", JournalEventNameType.Неисправность, JournalEventNameType.Неисправность_устранена)]
		Контакт_реле_1,

		[EventDescriptionAttribute("Контакт реле 2", JournalEventNameType.Неисправность, JournalEventNameType.Неисправность_устранена)]
		Контакт_реле_2,

		[EventDescriptionAttribute("Обрыв кнопки ПУСК", JournalEventNameType.Неисправность, JournalEventNameType.Неисправность_устранена)]
		Обрыв_кнопки_ПУСК,

		[EventDescriptionAttribute("КЗ кнопки ПУСК", JournalEventNameType.Неисправность, JournalEventNameType.Неисправность_устранена)]
		КЗ_кнопки_ПУСК,

		[EventDescriptionAttribute("Обрыв кнопки СТОП", JournalEventNameType.Неисправность, JournalEventNameType.Неисправность_устранена)]
		Обрыв_кнопки_СТОП,

		[EventDescriptionAttribute("КЗ кнопки СТОП", JournalEventNameType.Неисправность, JournalEventNameType.Неисправность_устранена)]
		КЗ_кнопки_СТОП,

		[EventDescriptionAttribute("Отсутствуют или испорчены сообщения для воспроизведения", JournalEventNameType.Неисправность, JournalEventNameType.Неисправность_устранена)]
		Отсутствуют_или_испорчены_сообщения_для_воспроизведения,

		[EventDescriptionAttribute("Выход", JournalEventNameType.Неисправность, JournalEventNameType.Неисправность_устранена)]
		Выход,

		[EventDescriptionAttribute("Обрыв Низкий уровень", JournalEventNameType.Неисправность, JournalEventNameType.Неисправность_устранена)]
		Обрыв_Низкий_уровень,

		[EventDescriptionAttribute("КЗ Низкий уровень", JournalEventNameType.Неисправность, JournalEventNameType.Неисправность_устранена)]
		КЗ_Низкий_уровень,

		[EventDescriptionAttribute("Обрыв Высокий уровень", JournalEventNameType.Неисправность, JournalEventNameType.Неисправность_устранена)]
		Обрыв_Высокий_уровень,

		[EventDescriptionAttribute("КЗ Высокий уровень", JournalEventNameType.Неисправность, JournalEventNameType.Неисправность_устранена)]
		КЗ_Высокий_уровень,

		[EventDescriptionAttribute("Обрыв Аварийный уровень", JournalEventNameType.Неисправность, JournalEventNameType.Неисправность_устранена)]
		Обрыв_Аварийный_уровень,

		[EventDescriptionAttribute("КЗ Аварийный уровень", JournalEventNameType.Неисправность, JournalEventNameType.Неисправность_устранена)]
		КЗ_Аварийный_уровень,

		[EventDescriptionAttribute("Аварийный уровень", JournalEventNameType.Неисправность, JournalEventNameType.Неисправность_устранена)]
		Аварийный_уровень,

		[EventDescriptionAttribute("Питание силовое", JournalEventNameType.Неисправность, JournalEventNameType.Неисправность_устранена)]
		Питание_силовое,

		[EventDescriptionAttribute("Питание контроллера", JournalEventNameType.Неисправность, JournalEventNameType.Неисправность_устранена)]
		Питание_контроллера,

		[EventDescriptionAttribute("Несовместимость сигналов", JournalEventNameType.Неисправность, JournalEventNameType.Неисправность_устранена)]
		Несовместимость_сигналов,

		[EventDescriptionAttribute("Неисправность одной или обеих фаз(контроль нагрузки)", JournalEventNameType.Неисправность, JournalEventNameType.Неисправность_устранена)]
		Неисправность_одной_или_обеих_фаз_контроль_нагрузки,

		[EventDescriptionAttribute("Обрыв Давление на выходе", JournalEventNameType.Неисправность, JournalEventNameType.Неисправность_устранена)]
		Обрыв_Давление_на_выходе,

		[EventDescriptionAttribute("КЗ Давление на выходе", JournalEventNameType.Неисправность, JournalEventNameType.Неисправность_устранена)]
		КЗ_Давление_на_выходе,

		[EventDescriptionAttribute("Обрыв ДУ ПУСК", JournalEventNameType.Неисправность, JournalEventNameType.Неисправность_устранена)]
		Обрыв_ДУ_ПУСК,

		[EventDescriptionAttribute("КЗ ДУ ПУСК", JournalEventNameType.Неисправность, JournalEventNameType.Неисправность_устранена)]
		КЗ_ДУ_ПУСК,

		[EventDescriptionAttribute("Обрыв ДУ СТОП", JournalEventNameType.Неисправность, JournalEventNameType.Неисправность_устранена)]
		Обрыв_ДУ_СТОП,

		[EventDescriptionAttribute("КЗ ДУ СТОП", JournalEventNameType.Неисправность, JournalEventNameType.Неисправность_устранена)]
		КЗ_ДУ_СТОП,

		[EventDescriptionAttribute("АЛС 1 Неизвестное устройство", JournalEventNameType.Неисправность, JournalEventNameType.Неисправность_устранена)]
		АЛС_1_Неизвестное_устройство,

		[EventDescriptionAttribute("АЛС 2 Неизвестное устройство", JournalEventNameType.Неисправность, JournalEventNameType.Неисправность_устранена)]
		АЛС_2_Неизвестное_устройство,

		[EventDescriptionAttribute("АЛС 3 Неизвестное устройство", JournalEventNameType.Неисправность, JournalEventNameType.Неисправность_устранена)]
		АЛС_3_Неизвестное_устройство,

		[EventDescriptionAttribute("АЛС 4 Неизвестное устройство", JournalEventNameType.Неисправность, JournalEventNameType.Неисправность_устранена)]
		АЛС_4_Неизвестное_устройство,

		[EventDescriptionAttribute("АЛС 5 Неизвестное устройство", JournalEventNameType.Неисправность, JournalEventNameType.Неисправность_устранена)]
		АЛС_5_Неизвестное_устройство,

		[EventDescriptionAttribute("АЛС 6 Неизвестное устройство", JournalEventNameType.Неисправность, JournalEventNameType.Неисправность_устранена)]
		АЛС_6_Неизвестное_устройство,

		[EventDescriptionAttribute("АЛС 7 Неизвестное устройство", JournalEventNameType.Неисправность, JournalEventNameType.Неисправность_устранена)]
		АЛС_7_Неизвестное_устройство,

		[EventDescriptionAttribute("АЛС 8 Неизвестное устройство", JournalEventNameType.Неисправность, JournalEventNameType.Неисправность_устранена)]
		АЛС_8_Неизвестное_устройство,

		[EventDescriptionAttribute("АЛС 1 Неизвестный тип устройства", JournalEventNameType.Неисправность, JournalEventNameType.Неисправность_устранена)]
		АЛС_1_Неизвестный_тип_устройства,

		[EventDescriptionAttribute("АЛС 2 Неизвестный тип устройства", JournalEventNameType.Неисправность, JournalEventNameType.Неисправность_устранена)]
		АЛС_2_Неизвестный_тип_устройства,

		[EventDescriptionAttribute("АЛС 3 Неизвестный тип устройства", JournalEventNameType.Неисправность, JournalEventNameType.Неисправность_устранена)]
		АЛС_3_Неизвестный_тип_устройства,

		[EventDescriptionAttribute("АЛС 4 Неизвестный тип устройства", JournalEventNameType.Неисправность, JournalEventNameType.Неисправность_устранена)]
		АЛС_4_Неизвестный_тип_устройства,

		[EventDescriptionAttribute("АЛС 5 Неизвестный тип устройства", JournalEventNameType.Неисправность, JournalEventNameType.Неисправность_устранена)]
		АЛС_5_Неизвестный_тип_устройства,

		[EventDescriptionAttribute("АЛС 6 Неизвестный тип устройства", JournalEventNameType.Неисправность, JournalEventNameType.Неисправность_устранена)]
		АЛС_6_Неизвестный_тип_устройства,

		[EventDescriptionAttribute("АЛС 7 Неизвестный тип устройства", JournalEventNameType.Неисправность, JournalEventNameType.Неисправность_устранена)]
		АЛС_7_Неизвестный_тип_устройства,

		[EventDescriptionAttribute("АЛС 8 Неизвестный тип устройства", JournalEventNameType.Неисправность, JournalEventNameType.Неисправность_устранена)]
		АЛС_8_Неизвестный_тип_устройства,

		[EventDescriptionAttribute("АЛС 1 Другой тип устройства", JournalEventNameType.Неисправность, JournalEventNameType.Неисправность_устранена)]
		АЛС_1_Другой_тип_устройства,

		[EventDescriptionAttribute("АЛС 2 Другой тип устройства", JournalEventNameType.Неисправность, JournalEventNameType.Неисправность_устранена)]
		АЛС_2_Другой_тип_устройства,

		[EventDescriptionAttribute("АЛС 3 Другой тип устройства", JournalEventNameType.Неисправность, JournalEventNameType.Неисправность_устранена)]
		АЛС_3_Другой_тип_устройства,

		[EventDescriptionAttribute("АЛС 4 Другой тип устройства", JournalEventNameType.Неисправность, JournalEventNameType.Неисправность_устранена)]
		АЛС_4_Другой_тип_устройства,

		[EventDescriptionAttribute("АЛС 5 Другой тип устройства", JournalEventNameType.Неисправность, JournalEventNameType.Неисправность_устранена)]
		АЛС_5_Другой_тип_устройства,

		[EventDescriptionAttribute("АЛС 6 Другой тип устройства", JournalEventNameType.Неисправность, JournalEventNameType.Неисправность_устранена)]
		АЛС_6_Другой_тип_устройства,

		[EventDescriptionAttribute("АЛС 7 Другой тип устройства", JournalEventNameType.Неисправность, JournalEventNameType.Неисправность_устранена)]
		АЛС_7_Другой_тип_устройства,

		[EventDescriptionAttribute("АЛС 8 Другой тип устройства", JournalEventNameType.Неисправность, JournalEventNameType.Неисправность_устранена)]
		АЛС_8_Другой_тип_устройства,

		[EventDescriptionAttribute("Обрыв АЛС 1-2", JournalEventNameType.Неисправность, JournalEventNameType.Неисправность_устранена)]
		Обрыв_АЛС_1_2,

		[EventDescriptionAttribute("Обрыв АЛС 3-4", JournalEventNameType.Неисправность, JournalEventNameType.Неисправность_устранена)]
		Обрыв_АЛС_3_4,

		[EventDescriptionAttribute("Обрыв АЛС 5-6", JournalEventNameType.Неисправность, JournalEventNameType.Неисправность_устранена)]
		Обрыв_АЛС_5_6,

		[EventDescriptionAttribute("Обрыв АЛС 7-8", JournalEventNameType.Неисправность, JournalEventNameType.Неисправность_устранена)]
		Обрыв_АЛС_7_8,

		[EventDescriptionAttribute("Обрыв АЛС 1", JournalEventNameType.Неисправность, JournalEventNameType.Неисправность_устранена)]
		Обрыв_АЛС_1,

		[EventDescriptionAttribute("Обрыв АЛС 2", JournalEventNameType.Неисправность, JournalEventNameType.Неисправность_устранена)]
		Обрыв_АЛС_2,

		[EventDescriptionAttribute("Обрыв АЛС 3", JournalEventNameType.Неисправность, JournalEventNameType.Неисправность_устранена)]
		Обрыв_АЛС_3,

		[EventDescriptionAttribute("Обрыв АЛС 4", JournalEventNameType.Неисправность, JournalEventNameType.Неисправность_устранена)]
		Обрыв_АЛС_4,

		[EventDescriptionAttribute("Обрыв АЛС 5", JournalEventNameType.Неисправность, JournalEventNameType.Неисправность_устранена)]
		Обрыв_АЛС_5,

		[EventDescriptionAttribute("Обрыв АЛС 6", JournalEventNameType.Неисправность, JournalEventNameType.Неисправность_устранена)]
		Обрыв_АЛС_6,

		[EventDescriptionAttribute("Обрыв АЛС 7", JournalEventNameType.Неисправность, JournalEventNameType.Неисправность_устранена)]
		Обрыв_АЛС_7,

		[EventDescriptionAttribute("Обрыв АЛС 8", JournalEventNameType.Неисправность, JournalEventNameType.Неисправность_устранена)]
		Обрыв_АЛС_8,

		[EventDescriptionAttribute("ОЛС", JournalEventNameType.Неисправность, JournalEventNameType.Неисправность_устранена)]
		ОЛС,

		[EventDescriptionAttribute("РЛС", JournalEventNameType.Неисправность, JournalEventNameType.Неисправность_устранена)]
		РЛС,

		[EventDescriptionAttribute("Потеря связи", JournalEventNameType.Неисправность, JournalEventNameType.Неисправность_устранена)]
		Потеря_связи,

		[EventDescriptionAttribute("Отсутствие сетевого напряжения", JournalEventNameType.Неисправность, JournalEventNameType.Неисправность_устранена)]
		Отсутствие_сетевого_напряжения,

		[EventDescriptionAttribute("КЗ Выхода 1", JournalEventNameType.Неисправность, JournalEventNameType.Неисправность_устранена)]
		КЗ_Выхода_1,

		[EventDescriptionAttribute("Перегрузка Выхода 1", JournalEventNameType.Неисправность, JournalEventNameType.Неисправность_устранена)]
		Перегрузка_Выхода_1,

		[EventDescriptionAttribute("Напряжение Выхода 1 выше нормы", JournalEventNameType.Неисправность, JournalEventNameType.Неисправность_устранена)]
		Напряжение_Выхода_1_выше_нормы,

		[EventDescriptionAttribute("КЗ Выхода 2", JournalEventNameType.Неисправность, JournalEventNameType.Неисправность_устранена)]
		КЗ_Выхода_2,

		[EventDescriptionAttribute("Перегрузка Выхода 2", JournalEventNameType.Неисправность, JournalEventNameType.Неисправность_устранена)]
		Перегрузка_Выхода_2,

		[EventDescriptionAttribute("Напряжение Выхода 2 выше нормы", JournalEventNameType.Неисправность, JournalEventNameType.Неисправность_устранена)]
		Напряжение_Выхода_2_выше_нормы,

		[EventDescriptionAttribute("АКБ 1", JournalEventNameType.Неисправность, JournalEventNameType.Неисправность_устранена)]
		АКБ_1,

		[EventDescriptionAttribute("АКБ 1 Разряд", JournalEventNameType.Неисправность, JournalEventNameType.Неисправность_устранена)]
		АКБ_1_Разряд,

		[EventDescriptionAttribute("АКБ 1 Глубокий Разряд", JournalEventNameType.Неисправность, JournalEventNameType.Неисправность_устранена)]
		АКБ_1_Глубокий_Разряд,

		[EventDescriptionAttribute("АКБ 1 Отсутствие", JournalEventNameType.Неисправность, JournalEventNameType.Неисправность_устранена)]
		АКБ_1_Отсутствие,

		[EventDescriptionAttribute("АКБ 2", JournalEventNameType.Неисправность, JournalEventNameType.Неисправность_устранена)]
		АКБ_2,

		[EventDescriptionAttribute("АКБ 2 Разряд", JournalEventNameType.Неисправность, JournalEventNameType.Неисправность_устранена)]
		АКБ_2_Разряд,

		[EventDescriptionAttribute("АКБ 2 Глубокий Разряд", JournalEventNameType.Неисправность, JournalEventNameType.Неисправность_устранена)]
		АКБ_2_Глубокий_Разряд,

		[EventDescriptionAttribute("АКБ 2 Отсутствие", JournalEventNameType.Неисправность, JournalEventNameType.Неисправность_устранена)]
		АКБ_2_Отсутствие,


		[EventDescriptionAttribute("Команда от прибора", JournalEventNameType.Информация)]
		Команда_от_прибора,

		[EventDescriptionAttribute("Команда от кнопки", JournalEventNameType.Информация)]
		Команда_от_кнопки,

		[EventDescriptionAttribute("Изменение автоматики по неисправности", JournalEventNameType.Информация)]
		Изменение_автоматики_по_неисправности,

		[EventDescriptionAttribute("Изменение автомат по СТОП", JournalEventNameType.Информация)]
		Изменение_автомат_по_СТОП,

		[EventDescriptionAttribute("Изменение автоматики по Д-О", JournalEventNameType.Информация)]
		Изменение_автоматики_по_Д_О,

		[EventDescriptionAttribute("Изменение автоматики по ТМ", JournalEventNameType.Информация)]
		Изменение_автоматики_по_ТМ,

		[EventDescriptionAttribute("Ручной пуск", JournalEventNameType.Информация)]
		Ручной_пуск,

		[EventDescriptionAttribute("Отлож пуск АУП Д-О", JournalEventNameType.Информация)]
		Отлож_пуск_АУП_Д_О,

		[EventDescriptionAttribute("Пуск АУП завершен", JournalEventNameType.Информация)]
		Пуск_АУП_завершен,

		[EventDescriptionAttribute("Стоп по кнопке СТОП", JournalEventNameType.Информация)]
		Стоп_по_кнопке_СТОП,

		[EventDescriptionAttribute("Программирование мастер-ключа", JournalEventNameType.Информация)]
		Программирование_мастер_ключа,

		[EventDescriptionAttribute("Датчик ДАВЛЕНИЕ", JournalEventNameType.Информация)]
		Датчик_ДАВЛЕНИЕ,

		[EventDescriptionAttribute("Датчик МАССА", JournalEventNameType.Информация)]
		Датчик_МАССА,

		[EventDescriptionAttribute("Сигнал из памяти", JournalEventNameType.Информация)]
		Сигнал_из_памяти,

		[EventDescriptionAttribute("Сигнал аналог входа", JournalEventNameType.Информация)]
		Сигнал_аналог_входа,

		[EventDescriptionAttribute("Замена списка на 1", JournalEventNameType.Информация)]
		Замена_списка_на_1,

		[EventDescriptionAttribute("Замена списка на 2", JournalEventNameType.Информация)]
		Замена_списка_на_2,

		[EventDescriptionAttribute("Замена списка на 3", JournalEventNameType.Информация)]
		Замена_списка_на_3,

		[EventDescriptionAttribute("Замена списка на 4", JournalEventNameType.Информация)]
		Замена_списка_на_4,

		[EventDescriptionAttribute("Замена списка на 5", JournalEventNameType.Информация)]
		Замена_списка_на_5,

		[EventDescriptionAttribute("Замена списка на 6", JournalEventNameType.Информация)]
		Замена_списка_на_6,

		[EventDescriptionAttribute("Замена списка на 7", JournalEventNameType.Информация)]
		Замена_списка_на_7,

		[EventDescriptionAttribute("Замена списка на 8", JournalEventNameType.Информация)]
		Замена_списка_на_8,

		[EventDescriptionAttribute("Низкий уровень", JournalEventNameType.Информация)]
		Низкий_уровень,

		[EventDescriptionAttribute("Высокий уровень", JournalEventNameType.Информация)]
		Высокий_уровень,

		[EventDescriptionAttribute("Уровень норма", JournalEventNameType.Информация)]
		Уровень_норма,

		[EventDescriptionAttribute("Перевод в автоматический режим со шкафа", JournalEventNameType.Информация)]
		Перевод_в_автоматический_режим_со_шкафа,

		[EventDescriptionAttribute("Перевод в ручной режим со шкафа", JournalEventNameType.Информация)]
		Перевод_в_ручной_режим_со_шкафа,

		[EventDescriptionAttribute("Перевод в отключенный режим со шкафа", JournalEventNameType.Информация)]
		Перевод_в_отключенный_режим_со_шкафа,

		[EventDescriptionAttribute("Неопределено", JournalEventNameType.Информация)]
		Неопределено,

		[EventDescriptionAttribute("Пуск невозможен", JournalEventNameType.Информация)]
		Пуск_невозможен,

		[EventDescriptionAttribute("Авария пневмоемкости", JournalEventNameType.Информация)]
		Авария_пневмоемкости,

		[EventDescriptionAttribute("Аварийный уровень", JournalEventNameType.Информация)]
		Аварийный_уровень_Информация,

		[EventDescriptionAttribute("Запрет пуска НС", JournalEventNameType.Информация)]
		Запрет_пуска_НС,

		[EventDescriptionAttribute("Запрет пуска компрессора", JournalEventNameType.Информация)]
		Запрет_пуска_компрессора,

		[EventDescriptionAttribute("Ввод 1", JournalEventNameType.Информация)]
		Ввод_1,

		[EventDescriptionAttribute("Ввод 2", JournalEventNameType.Информация)]
		Ввод_2,

		[EventDescriptionAttribute("Команда от логики", JournalEventNameType.Информация)]
		Команда_от_логики,

		[EventDescriptionAttribute("Команда от ДУ", JournalEventNameType.Информация)]
		Команда_от_ДУ,

		[EventDescriptionAttribute("Давление низкое", JournalEventNameType.Информация)]
		Давление_низкое,

		[EventDescriptionAttribute("Давление высокое", JournalEventNameType.Информация)]
		Давление_высокое,

		[EventDescriptionAttribute("Давление норма", JournalEventNameType.Информация)]
		Давление_норма,

		[EventDescriptionAttribute("Давление неопределен", JournalEventNameType.Информация)]
		Давление_неопределен,

		[EventDescriptionAttribute("Давление на выходе есть", JournalEventNameType.Информация)]
		Давление_на_выходе_есть,

		[EventDescriptionAttribute("Давления на выходе нет", JournalEventNameType.Информация)]
		Давления_на_выходе_нет,

		[EventDescriptionAttribute("Выключить", JournalEventNameType.Информация)]
		Выключить_Информация,

		[EventDescriptionAttribute("Стоп", JournalEventNameType.Информация)]
		Стоп,

		[EventDescriptionAttribute("Запрет пуска", JournalEventNameType.Информация)]
		Запрет_пуска,

		[EventDescriptionAttribute("Оператор", JournalEventNameType.Вход_пользователя_в_прибор, JournalEventNameType.Выход_пользователя_из_прибора)]
		Оператор,

		[EventDescriptionAttribute("Администратор", JournalEventNameType.Вход_пользователя_в_прибор, JournalEventNameType.Выход_пользователя_из_прибора)]
		Администратор,

		[EventDescriptionAttribute("Инсталлятор", JournalEventNameType.Вход_пользователя_в_прибор, JournalEventNameType.Выход_пользователя_из_прибора)]
		Инсталлятор,

		[EventDescriptionAttribute("Изготовитель", JournalEventNameType.Вход_пользователя_в_прибор, JournalEventNameType.Выход_пользователя_из_прибора)]
		Изготовитель,

		[EventDescriptionAttribute("Кнопка", JournalEventNameType.Тест, JournalEventNameType.Тест_устранен)]
		Кнопка,

		[EventDescriptionAttribute("Указка", JournalEventNameType.Тест, JournalEventNameType.Тест_устранен)]
		Указка,

		[EventDescriptionAttribute("Предварительная", JournalEventNameType.Запыленность, JournalEventNameType.Запыленность_устранена)]
		Предварительная,

		[EventDescriptionAttribute("Критическая", JournalEventNameType.Запыленность, JournalEventNameType.Запыленность_устранена)]
		Критическая,

		[EventDescriptionAttribute("Добавление или редактирование", JournalEventNameType.Редактирование_сотрудника, JournalEventNameType.Редактирование_отдела, JournalEventNameType.Редактирование_должности, JournalEventNameType.Редактирование_шаблона_доступа, JournalEventNameType.Редактирование_организации, JournalEventNameType.Редактирование_дополнительной_колонки, JournalEventNameType.Редактирование_дневного_графика, JournalEventNameType.Редактирование_графика_работы, JournalEventNameType.Редактирование_графика_работы, JournalEventNameType.Редактирование_графика_работы_сотрудника, JournalEventNameType.Редактирование_праздничного_дня)]
		Добавление_или_редактирование,

		[EventDescriptionAttribute("Редактирование", JournalEventNameType.Редактирование_сотрудника, JournalEventNameType.Редактирование_отдела, JournalEventNameType.Редактирование_должности, JournalEventNameType.Редактирование_шаблона_доступа, JournalEventNameType.Редактирование_организации, JournalEventNameType.Редактирование_дополнительной_колонки, JournalEventNameType.Редактирование_дневного_графика, JournalEventNameType.Редактирование_графика_работы, JournalEventNameType.Редактирование_графика_работы, JournalEventNameType.Редактирование_графика_работы_сотрудника, JournalEventNameType.Редактирование_праздничного_дня)]
		Редактирование,

		[EventDescriptionAttribute("Удаление", JournalEventNameType.Редактирование_сотрудника, JournalEventNameType.Редактирование_отдела, JournalEventNameType.Редактирование_должности, JournalEventNameType.Редактирование_шаблона_доступа, JournalEventNameType.Редактирование_организации, JournalEventNameType.Редактирование_дополнительной_колонки, JournalEventNameType.Редактирование_дневного_графика, JournalEventNameType.Редактирование_графика_работы, JournalEventNameType.Редактирование_графика_работы, JournalEventNameType.Редактирование_графика_работы_сотрудника, JournalEventNameType.Редактирование_праздничного_дня)]
		Удаление,

		[EventDescriptionAttribute("Восстановление", JournalEventNameType.Редактирование_сотрудника, JournalEventNameType.Редактирование_отдела, JournalEventNameType.Редактирование_должности, JournalEventNameType.Редактирование_шаблона_доступа, JournalEventNameType.Редактирование_организации, JournalEventNameType.Редактирование_дополнительной_колонки, JournalEventNameType.Редактирование_дневного_графика, JournalEventNameType.Редактирование_графика_работы, JournalEventNameType.Редактирование_графика_работы, JournalEventNameType.Редактирование_графика_работы_сотрудника, JournalEventNameType.Редактирование_праздничного_дня)]
		Восстановление,

		[EventDescriptionAttribute("Метод открытия Неизвестно", JournalEventNameType.Проход_разрешен, JournalEventNameType.Проход_запрещен)]
		Метод_открытия_Неизвестно,

		[EventDescriptionAttribute("Метод открытия Пароль", JournalEventNameType.Проход_разрешен, JournalEventNameType.Проход_запрещен)]
		Метод_открытия_Пароль,

		[EventDescriptionAttribute("Метод открытия Карта", JournalEventNameType.Проход_разрешен, JournalEventNameType.Проход_запрещен)]
		Метод_открытия_Карта,

		[EventDescriptionAttribute("Метод открытия Сначала карта", JournalEventNameType.Проход_разрешен, JournalEventNameType.Проход_запрещен)]
		Метод_открытия_Сначала_карта,

		[EventDescriptionAttribute("Метод открытия Сначала пароль", JournalEventNameType.Проход_разрешен, JournalEventNameType.Проход_запрещен)]
		Метод_открытия_Сначала_пароль,

		[EventDescriptionAttribute("Метод открытия Удаленно", JournalEventNameType.Проход_разрешен, JournalEventNameType.Проход_запрещен)]
		Метод_открытия_Удаленно,

		[EventDescriptionAttribute("Метод открытия Кнопка", JournalEventNameType.Проход_разрешен, JournalEventNameType.Проход_запрещен)]
		Метод_открытия_Кнопка,

		[EventDescriptionAttribute("Остановлено", JournalEventNameType.Информация)]
		Остановлено,

		[EventDescriptionAttribute("Отсчет задержки", JournalEventNameType.Информация)]
		Отсчет_задержки,
	}
}