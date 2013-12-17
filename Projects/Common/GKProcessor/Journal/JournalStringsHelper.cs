using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using XFiresecAPI;

namespace GKProcessor
{
	public static class JournalStringsHelper
	{
		public static string ToFire(byte b)
		{
			switch (b)
			{
				case 1: return "Ручник сорван";
				case 2: return "Срабатывание по дыму";
				case 3: return "Срабатывание по температуре";
				case 4: return "Срабатывание по градиенту температуры";
			}
			return "";
		}

		public static string ToState(byte b)
		{
			switch (b)
			{
				case 1: return "Отсчет задержки";
				case 2: return "Включено";
				case 3: return "Выключено";
				case 4: return "Включается";
				case 5: return "Выключается";
				case 6: return "Кнопка";
				case 7: return "Изменение автоматики по неисправности";
				case 8: return "Изменение автоматики по кнопке СТОП";
				case 9: return "Изменение автоматики по датчику ДВЕРИ-ОКНА";
				case 10: return "Изменение автоматики по ТМ";
				case 11: return "Автоматика включена";
				case 12: return "Ручной пуск АУП от ИПР";
				case 13: return "Отложенный пуск АУП по датчику ДВЕРИ-ОКНА";
				case 14: return "Пуск АУП завершен";
				case 15: return "Останов тушения по кнопке СТОП";
				case 16: return "Программирование мастер-ключа";

				case 17: return "Отсчет удержания";
				case 18: return "Уровень высокий";
				case 19: return "Уровень низкий";
				case 20: return "Ход по команде с УЗЗ";

				case 21: return "У ДУ сообщение ПУСК НЕВОЗМОЖЕН";
				case 22: return "Авария пневмоемкости";
				case 23: return "Уровень аварийный";
				case 24: return "Запрет пуска НС";
				case 25: return "Запрет пуска компрессора";
				case 26: return "Команда с УЗН";
				case 27: return "Перевод в режим ручного управления";

				case 30: return "Состояние не определено";
				case 31: return "Остановлено";
			}
			return "Состояние";
		}

		public static string ToFailure(byte b)
		{
			switch (b)
			{
				case 1: return "Напряжение питания устройства не в норме"; // РМК, МПТ, МРО-2М
				case 2: return "Оптический канал или фотоусилитель"; // ИЗВЕЩАТЕЛИ
				case 3: return "Температурный канал"; // ИЗВЕЩАТЕЛИ
				case 4: return "КЗ ШС"; // МПТ, АМ
				case 5: return "Обрыв ШС"; // МПТ, АМ
				case 6: return "";
				case 7: return "";
				case 8: return "Вскрытие корпуса"; // АМ, ШУН, ШУЗ, КАУ, ГК
				case 9: return "Контакт не переключается"; // РМ
				case 10: return "Напряжение запуска реле ниже нормы"; // РМК
				case 11: return "КЗ выхода"; // РМК
				case 12: return "Обрыв выхода"; // РМК
				case 13: return "Напряжение питания ШС ниже нормы"; // МПТ
				case 14: return "Ошибка памяти"; // МПТ
				case 15: return "КЗ выхода 1"; // МПТ
				case 16: return "КЗ выхода 2"; // МПТ
				case 17: return "КЗ выхода 3"; // МПТ
				case 18: return "КЗ выхода 4"; // МПТ
				case 19: return "КЗ выхода 5"; // МПТ
				case 20: return "Обрыв выхода 1"; // МПТ
				case 21: return "Обрыв выхода 2"; // МПТ
				case 22: return "Обрыв выхода 3"; // МПТ
				case 23: return "Обрыв выхода 4"; // МПТ
				case 24: return "Обрыв выхода 5"; // МПТ
				case 25: return "Блокировка пуска"; // МДУ
				case 26: return "Низкое напряжение питания привода"; // МДУ
				case 27: return "Обрыв кнопки НОРМА"; // МДУ
				case 28: return "КЗ кнопки НОРМА"; // МДУ
				case 29: return "Обрыв кнопка ЗАЩИТА"; // МДУ
				case 30: return "КЗ кнопки ЗАЩИТА"; // МДУ
				case 31: return "Обрыв концевого выключателя ОТКРЫТО"; // МДУ
				case 32: return "Обрыв концевого выключателя ЗАКРЫТО"; // МДУ
				case 33: return "Обрыв цепи 1 ДВИГАТЕЛЯ"; // МДУ
				case 34: return "Замкнуты/разомкнуты оба концевика"; // МДУ
				case 35: return "Превышение времени хода"; // МДУ, ШУЗ
				case 36: return "Обрыв в линии РЕЛЕ";
				case 37: return "КЗ в линии РЕЛЕ";
				case 38: return "Выход 1";
				case 39: return "Выход 2";
				case 40: return "Выход 3";
				case 41: return "";
				case 42: return "Обрыв концевого выключателя ОТКРЫТО"; // ШУЗ
				case 43: return "КЗ концевого выключателя ОТКРЫТО"; // ШУЗ
				case 44: return "Обрыв муфтового выключателя ОТКРЫТО"; // ШУЗ
				case 45: return "КЗ муфтового выключателя ОТКРЫТО"; // ШУЗ
				case 46: return "Обрыв концевого выключателя ЗАКРЫТО"; // ШУЗ
				case 47: return "КЗ концевого выключателя ЗАКРЫТО"; // ШУЗ
				case 48: return "Обрыв муфтового выключателя ЗАКРЫТО"; // ШУЗ
				case 49: return "КЗ муфтового выключателя ЗАКРЫТО"; // ШУЗ
				case 50: return "Обрыв кнопки Открыть УЗЗ/Закрыть УЗЗ"; // ШУЗ
				case 51: return "КЗ кнопки Открыть УЗЗ/Закрыть УЗЗ"; // ШУЗ
				case 52: return "Обрыв кнопки СТОП УЗЗ"; // ШУЗ
				case 53: return "КЗ кнопки СТОП УЗЗ"; // ШУЗ
				case 54: return "Обрыв давление низкое ";
				case 55: return "КЗ давление низкое  ";
				case 56: return "Таймаут по давлению ";
				case 57: return "КВ/МВ"; // ШУЗ
				case 58: return "Не задан режим"; // ШУЗ
				case 59: return "Отказ ШУЗ"; // ШУЗ
				case 60: return "ДУ/ДД"; // ШУН
				case 61: return "Обрыв входа 1"; // ШУН
				case 62: return "КЗ входа 1"; // ШУН
				case 63: return "Обрыв входа 2"; // ШУН
				case 64: return "КЗ входа 2"; // ШУН
				case 65: return "Обрыв входа 3"; // ШУН
				case 66: return "КЗ входа 3"; // ШУН
				case 67: return "Обрыв входа 4"; // ШУН
				case 68: return "КЗ входа 4"; // ШУН
				case 69: return "Не задан тип"; // ШУН
				case 70: return "Отказ ПН"; // ШУН
				case 71: return "Отказ ШУН"; // ШУН
				case 72: return "Питание 1"; // КАУ, ГК
				case 73: return "Питание 2"; // КАУ, ГК
				case 74: return "Отказ АЛС 1 или 2"; // КАУ
				case 75: return "Отказ АЛС 3 или 4"; // КАУ
				case 76: return "Отказ АЛС 5 или 6"; // КАУ
				case 77: return "Отказ АЛС 7 или 8"; // КАУ
				case 78: return "Обрыв цепи 2 ДВИГАТЕЛЯ"; // МДУ
				case 79: return "КЗ АЛС 1"; // КАУ
				case 80: return "КЗ АЛС 2"; // КАУ
				case 81: return "КЗ АЛС 3"; // КАУ
				case 82: return "КЗ АЛС 4"; // КАУ
				case 83: return "КЗ АЛС 5"; // КАУ
				case 84: return "КЗ АЛС 6"; // КАУ
				case 85: return "КЗ АЛС 7"; // КАУ
				case 86: return "КЗ АЛС 8"; // КАУ
				case 87: return "Истекло время вкл";
				case 88: return "Истекло время выкл";
				case 89: return "Контакт реле 1";
				case 90: return "Контакт реле 2";
				case 91: return "Обрыв кнопки ПУСК"; // МРО-2М
				case 92: return "КЗ кнопки ПУСК"; // МРО-2М
				case 93: return "Обрыв кнопки СТОП"; // МРО-2М
				case 94: return "КЗ кнопки СТОП"; // МРО-2М
				case 95: return "Отсутствуют или испорчены сообщения для воспроизведения"; // МРО-2М
				case 96: return "Выход";
                case 97: return "Обрыв Низкий уровень"; // БУШ
                case 98: return "КЗ Низкий уровень"; // БУШ
                case 99: return "Обрыв Высокий уровень"; // БУШ
                case 100: return "КЗ Высокий уровень"; // БУШ
                case 101: return "Обрыв Аварийный уровень"; // БУШ
                case 102: return "КЗ Аварийный уровень"; // БУШ
                case 103: return "Аварийный уровень"; // БУШ
                case 104: return "Питание силовое"; // БУШ
                case 105: return "Питание контроллера"; // БУШ
                case 106: return "Несовместимость сигналов"; // БУШ
                case 107: return "Неисправность одной или обеих фаз(контроль нагрузки)"; // БУШ
                case 108: return "Обрыв Давление на выходе"; // БУШ
                case 109: return "КЗ Давление на выходе"; // БУШ
                case 110: return "Обрыв ДУ ПУСК"; // БУШ
                case 111: return "КЗ ДУ ПУСК"; // БУШ
                case 112: return "Обрыв ДУ СТОП"; // БУШ
                case 113: return "КЗ ДУ СТОП"; // БУШ
				case 241: return "Обрыв АЛС 1-2";
				case 242: return "Обрыв АЛС 3-4";
				case 243: return "Обрыв АЛС 5-6";
				case 244: return "Обрыв АЛС 7-8";
				case 245: return "Обрыв АЛС 1";
				case 246: return "Обрыв АЛС 2";
				case 247: return "Обрыв АЛС 3";
				case 248: return "Обрыв АЛС 4";
				case 249: return "Обрыв АЛС 5";
				case 250: return "Обрыв АЛС 6";
				case 251: return "Обрыв АЛС 7";
				case 252: return "Обрыв АЛС 8";
				case 253: return "ОЛС"; // ГК
				case 254: return "РЛС"; // ГК
				case 255: return "Потеря связи";
			}
			return "";
		}

		public static string ToBUSHFailure(byte b)
		{
			switch (b)
			{
				//case 0: return "Вскрытие"; //
				case 1: return "Неисправность контакта"; //
				//case 2: return "Обрыв Низкий уровень"; //
				//case 3: return "КЗ Низкий уровень"; //
				//case 4: return "Обрыв Высокий уровень"; //
				//case 5: return "КЗ Высокий уровень"; //
				//case 6: return "Обрыв Аварийный уровень"; //
				//case 7: return "КЗ Аварийный уровень"; //
				//case 8: return "Аварийный уровень"; //
				//case 9: return "Питание силовое"; //
				//case 10: return "Питание контроллера"; //
				//case 11: return "Несовместимость сигналов"; //
				//case 12: return "Неисправность одной или обеих фаз(контроль нагрузки)"; //
				case 13: return "Обрыв Давление низкое"; //
				case 14: return "КЗ Давление низкое"; // 
				case 15: return "Таймаут по давлению"; //
				//case 16: return "Обрыв Давление на выходе"; //
				//case 17: return "КЗ Давление на выходе"; //
				//case 18: return "Обрыв ДУ ПУСК"; //
				//case 19: return "КЗ ДУ ПУСК"; //
				//case 20: return "Обрыв ДУ СТОП"; //
				//case 21: return "КЗ ДУ СТОП"; //
				//case 255: return "Потеря связи"; //
			}
			return "";
		}

		public static string ToBatteryFailure(byte b)
		{
			switch (b)
			{
				case 0: return "";
				case 1: return "Отсутствие сетевого напряжения";//
				case 2: return "Выход 1";
				case 3: return "КЗ Выхода 1";//
				case 4: return "Перегрузка Выхода 1";//
				case 5: return "Напряжение Выхода 1 выше нормы";//
				case 6: return "Выход 2";
				case 7: return "КЗ Выхода 2";//
				case 8: return "Перегрузка Выхода 2";//
				case 9: return "Напряжение Выхода 2 выше нормы";//
				case 10: return "АКБ 1";
				case 11: return "АКБ 1 Разряд";//
				case 12: return "АКБ 1 Глубокий Разряд";//
				case 13: return "АКБ 1 Отсутствие";//
				case 14: return "АКБ 2";
				case 15: return "АКБ 2 Разряд";//
				case 16: return "АКБ 2 Глубокий Разряд";//
				case 17: return "АКБ 2 Отсутствие";//
				case 255: return "Потеря связи";//
			}
			return "";
		}

		public static string ToInformation(byte b)
		{
			switch (b)
			{
				case 1: return "Команда от прибора";
				case 2: return "Команда от кнопки";
				case 3: return "Изменение автоматики по неисправности";
				case 4: return "Изменение автомат по СТОП";
				case 5: return "Изменение автоматики по Д-О";
				case 6: return "Изменение автоматики по ТМ";
				case 7: return "Ручной пуск";
				case 8: return "Отлож пуск АУП Д-О";
				case 9: return "Пуск АУП завершен";
				case 10: return "Стоп по кнопке СТОП";
				case 11: return "Программирование мастер-ключа";
				case 12: return "Датчик ДАВЛЕНИЕ";
				case 13: return "Датчик МАССА";
				case 14: return "Сигнал из памяти";
				case 15: return "Сигнал аналог входа";
				case 16: return "Замена списка на 1";
				case 17: return "Замена списка на 2";
				case 18: return "Замена списка на 3";
				case 19: return "Замена списка на 4";
				case 20: return "Замена списка на 5";
				case 21: return "Замена списка на 6";
				case 22: return "Замена списка на 7";
				case 23: return "Замена списка на 8";
                case 24: return "Низкий уровень";
                case 25: return "Высокий уровень";
                case 26: return "Уровень норма";
				case 27: return "Перевод в автоматический режим со шкафа";
				case 28: return "Перевод в ручной режим со шкафа";
				case 29: return "Перевод в отключенный режим со шкафа";
				case 30: return "Неопределено";
				case 31: return "Пуск невозможен";
				case 32: return "Авария пневмоемкости";
                case 33: return "Аварийный уровень";
				case 34: return "Запрет пуска НС";
				case 35: return "Запрет пуска компрессора";
				case 36: return "Ввод 1";
				case 37: return "Ввод 2";
				case 38: return "Команда от логики";
				case 39: return "Команда от ДУ";
				case 40: return "Давление низкое";
				case 41: return "Давление высокое";
				case 42: return "Давление норма";
				case 43: return "Давление неопределен";
				case 44: return "Давление на выходе есть";
                case 45: return "Давления на выходе нет";
				case 46: return "Выключить";
				case 47: return "Стоп";
				case 48: return "Запрет пуска";
				case 49: return "Ручной пуск";
			}
			return "";
		}

		public static string ToUser(byte b)
		{
			switch (b)
			{
				case 0: return "Оператор";
				case 1: return "Администратор";
				case 2: return "Инсталлятор";
				case 3: return "Изготовитель";
			}
			return "";
		}

		public static string GetPumpFailureMessage(string name, int pumpType)
		{
			switch (name)
			{
				case "Обрыв входа 1":
					if (pumpType == 0)
						return "Обрыв линии ЭКМ на входе насоса";
					if (pumpType == 1)
						return "Обрыв линии связи с датчиком низкого давления";
					if (pumpType == 2)
						return "Обрыв линии связи с датчиком верхнего уровня";
					break;
				case "КЗ входа 1":
					if (pumpType == 0)
						return "КЗ линии ЭКМ на входе насоса";
					if (pumpType == 1)
						return "КЗ линии связи с датчиком низкого давления";
					if (pumpType == 2)
						return "КЗ линии связи с датчиком верхнего уровня";
					break;

				case "Обрыв входа 2":
					if (pumpType == 0)
						return "Обрыв линии дистанционного управления";
					if (pumpType == 1)
						return "Обрыв линии связи с датчиком высокого давления";
					if (pumpType == 2)
						return "Обрыв линии связи с датчиком нижнего уровня";
					break;

				case "КЗ входа 2":
					if (pumpType == 0)
						return "КЗ линии дистанционного управления";
					if (pumpType == 1)
						return "КЗ линии связи с датчиком высокого давления";
					if (pumpType == 2)
						return "КЗ линии связи с датчиком нижнего уровня";
					break;

				case "Обрыв входа 3":
					if (pumpType == 0)
						return null;
					if (pumpType == 2)
						return "Обрыв линии связи с датчиком аварийного уровня";
					break;
				case "КЗ входа 3":
					if (pumpType == 0)
						return null;
					if (pumpType == 2)
						return "КЗ линии связи с датчиком аварийного уровня";
					break;

				case "Обрыв входа 4":
				case "КЗ входа 4":
					break;
			}
			return name;
		}
	}
}