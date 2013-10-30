using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Common.GK
{
	public static class DescriptionsHelper
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

		public static string ToFailure(byte b)
		{
			switch (b)
			{
				case 1: return "Напряжение питания устройства не в норме"; // РМК, МПТ
				case 2: return "Оптический канал или фотоусилитель"; // ИЗВЕЩАТЕЛИ
				case 3: return "Температурный канал"; // ИЗВЕЩАТЕЛИ
				case 4: return "КЗ ШС"; // МПТ, АМ
				case 5: return "Обрыв ШС"; // МПТ, АМ
				case 6: return "Датчик ДАВЛЕНИЕ     ";
				case 7: return "Датчик МАССА        ";
				case 8: return "Вскрытие корпуса"; // АМ
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
				case 25: return "Несовместим команды ";
				case 26: return "U привода не норма  ";
				case 27: return "Обрыв кнопки НОРМА"; // МДУ
				case 28: return "КЗ кнопки НОРМА"; // МДУ
				case 29: return "Обрыв кнопка ЗАЩИТА"; // МДУ
				case 30: return "КЗ кнопки ЗАЩИТА"; // МДУ
				case 31: return "Обрыв концевого выключателя ОТКРЫТО"; // МДУ
				case 32: return "Обрыв концевого выключателя ЗАКРЫТО"; // МДУ
				case 33: return "Обрыв цепи 1 ДВИГАТЕЛЯ"; // МДУ
				case 34: return "Замкнуты/разомкнуты оба концевика"; // МДУ
				case 35: return "Превышение времени хода"; // МДУ
				case 36: return "Обр в линии РЕЛЕ    ";
				case 37: return "КЗ в линии РЕЛЕ     ";
				case 38: return "Выход 1             ";
				case 39: return "Выход 2             ";
				case 40: return "Выход 3             ";
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
				case 61: return "Обрыв входа 9"; // ШУН
				case 62: return "КЗ входа 9"; // ШУН
				case 63: return "Обрыв входа 10"; // ШУН
				case 64: return "КЗ входа 10"; // ШУН
				case 65: return "Обрыв входа 11"; // ШУН
				case 66: return "КЗ входа 11"; // ШУН
				case 67: return "Обрыв входа 12"; // ШУН
				case 68: return "КЗ входа 12"; // ШУН
				case 69: return "Не задан тип"; // ШУН
				case 70: return "Отказ ПН"; // ШУН
				case 71: return "Отказ ШУН"; // ШУН
				case 72: return "Питание 1           ";
				case 73: return "Питание 2           ";
				case 74: return "АЛС 1-2             ";
				case 75: return "АЛС 3-4             ";
				case 76: return "АЛС 5-6             ";
				case 77: return "АЛС 7-8             ";
				case 78: return "Обр цепи 2 ДВИГАТЕЛЯ";
				case 79: return "КЗ АЛС1             ";
				case 80: return "КЗ АЛС2             ";
				case 81: return "КЗ АЛС3             ";
				case 82: return "КЗ АЛС4             ";
				case 83: return "КЗ АЛС5             ";
				case 84: return "КЗ АЛС6             ";
				case 85: return "КЗ АЛС7             ";
				case 86: return "КЗ АЛС8             ";
				case 87: return "Истекло время вкл   ";
				case 88: return "Истекло время выкл  ";
				case 89: return "Контакт реле 1      ";
				case 90: return "Контакт реле 2      ";
				case 91: return "Обр кнопка ПУСК     ";
				case 92: return "КЗ кнопка ПУСК      ";
				case 93: return "Обр кнопка СТОП     ";
				case 94: return "КЗ кнопка СТОП      ";
				case 95: return "Сообщения           ";
				case 96: return "Выход               ";
				case 97: return "Обр Уровень низкий  ";
				case 98: return "КЗ  Уровень низкий  ";
				case 99: return "Обр Уровень высокий ";
				case 100: return "КЗ  Уровень высокий ";
				case 101: return "Обр Уровень аварийн ";
				case 102: return "КЗ  Уровень аварийн ";
				case 103: return "Уровень аварийный   ";
				case 104: return "Питание силовое     ";
				case 105: return "Питание контроллера ";
				case 106: return "Несоответствие      ";
				case 107: return "Фаза                ";
				case 108: return "Обр Давление на вых ";
				case 109: return "КЗ  Давление на вых ";
				case 110: return "Обр ДУ ПУСК         ";
				case 111: return "КЗ  ДУ ПУСК         ";
				case 112: return "Обр ДУ СТОП         ";
				case 113: return "КЗ  ДУ СТОП         ";
				case 241: return "Обрыв АЛС 1-2      ";
				case 242: return "Обрыв АЛС 3-4      ";
				case 243: return "Обрыв АЛС 5-6      ";
				case 244: return "Обрыв АЛС 7-8      ";
				case 245: return "Обрыв АЛС 1        ";
				case 246: return "Обрыв АЛС 2        ";
				case 247: return "Обрыв АЛС 3        ";
				case 248: return "Обрыв АЛС 4        ";
				case 249: return "Обрыв АЛС 5        ";
				case 250: return "Обрыв АЛС 6        ";
				case 251: return "Обрыв АЛС 7        ";
				case 252: return "Обрыв АЛС 8        ";
				case 253: return "ОЛС                ";
				case 254: return "РЛС                ";
				case 255: return "Потеря связи       ";
			}
			return "";
		}

		public static string ToBUSHFailure(byte b)
		{
			switch (b)
			{
				case 0: return "Вскрытие            ";
				case 1: return "Контакт не переключ ";
				case 2: return "Обр Уровень низкий  ";
				case 3: return "КЗ  Уровень низкий  ";
				case 4: return "Обр Уровень высокий ";
				case 5: return "КЗ  Уровень высокий ";
				case 6: return "Обр Уровень аварийн ";
				case 7: return "КЗ  Уровень аварийн ";
				case 8: return "Уровень аварийный   ";
				case 9: return "Питание силовое     ";
				case 10: return "Питание контроллера ";
				case 11: return "Несоответствие      ";
				case 12: return "Фаза                ";
				case 13: return "Обр Давление низкое ";
				case 14: return "КЗ  Давление низкое ";
				case 15: return "Таймаут по давлению ";
				case 16: return "Обр Давлен на выходе";
				case 17: return "КЗ  Давлен на выходе";
				case 18: return "Обр ДУ ПУСК         ";
				case 19: return "КЗ  ДУ ПУСК         ";
				case 20: return "Обр ДУ СТОП         ";
				case 21: return "КЗ  ДУ СТОП         ";
			}
			return "";
		}

		public static string ToInformation(byte b)
		{
			switch (b)
			{
				case 1: return "Команда от прибора  ";
				case 2: return "Команда от кнопки   ";
				case 3: return "Изм автомат по Н    ";
				case 4: return "Изм автомат по СТОП ";
				case 5: return "Изм автомат по Д-О  ";
				case 6: return "Изм автомат по ТМ   ";
				case 7: return "Ручной пуск         ";
				case 8: return "Отлож пуск АУП Д-О  ";
				case 9: return "Пуск АУП завершен   ";
				case 10: return "Стоп по кнопке СТОП ";
				case 11: return "Прогр мастер-ключа  ";
				case 12: return "Датчик ДАВЛЕНИЕ     ";
				case 13: return "Датчик МАССА        ";
				case 14: return "Сигнал из памяти    ";
				case 15: return "Сигнал аналог входа ";
				case 16: return "Замена списка на 1  ";
				case 17: return "Замена списка на 2  ";
				case 18: return "Замена списка на 3  ";
				case 19: return "Замена списка на 4  ";
				case 20: return "Замена списка на 5  ";
				case 21: return "Замена списка на 6  ";
				case 22: return "Замена списка на 7  ";
				case 23: return "Замена списка на 8  ";
				case 24: return "Уровень низкий      ";
				case 25: return "Уровень высокий     ";
				case 26: return "Уровень норма       ";
				case 27: return "Автоматика          ";
				case 28: return "Ручное              ";
				case 29: return "Отключение          ";
				case 30: return "Неопределено        ";
				case 31: return "Пуск невозможен     ";
				case 32: return "Авария пневмоемкости";
				case 33: return "Уровень аварийный   ";
				case 34: return "Запрет пуска НС     ";
				case 35: return "Запрет пуска компрес";
				case 36: return "Ввод 1              ";
				case 37: return "Ввод 2              ";
				case 38: return "Команда от логики   ";
				case 39: return "Команда от ДУ       ";
				case 40: return "Давление низкое     ";
				case 41: return "Давление высокое    ";
				case 42: return "Давление норма      ";
				case 43: return "Давление неопределен";
				case 44: return "Давление на вых есть";
				case 45: return "Давление на вых нет ";
				case 46: return "Выключить           ";
				case 47: return "Стоп                ";
				case 48: return "Запрет пуска        ";
				case 49: return "Ручной пуск         ";
			}
			return "";
		}

		public static string ToBUSHInformation(byte b)
		{
			switch (b)
			{
				case 0: return "Уровень низкий      ";
				case 1: return "Уровень высокий     ";
				case 2: return "Уровень аварийный   ";
				case 3: return "Уровень норма       ";
				case 4: return "Команда от прибора  ";
				case 5: return "Команда от кнопки   ";
				case 6: return "Команда от логики   ";
				case 7: return "Команда от ДУ       ";
				case 8: return "Давление низкое     ";
				case 9: return "Давление норма      ";
				case 10: return "Давление высокое    ";
				case 11: return "Давление на вых есть";
				case 12: return "Давление на вых нет ";
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

		static List<string> result;

		public static List<string> GetAllDescriptions()
		{
			result = new List<string>();
			for (int i = 1; i <= 255; i++)
			{
				AddToResult(ToFailure((byte)i));
				AddToResult(ToBUSHFailure((byte)i));
				AddToResult(ToInformation((byte)i));
				AddToResult(ToBUSHInformation((byte)i));
			}
			return result;
		}

		static void AddToResult(string description)
		{
			if (description != "" && !result.Contains(description))
			{
				result.Add(description);
			}	
		}
	}
}
