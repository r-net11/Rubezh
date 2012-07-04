using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GKModule
{
	public class JournalItem
	{
		public int GKNo { get; set; }
		public int KAUNo { get; set; }

		byte Day { get; set; }
		byte Month { get; set; }
		byte Year { get; set; }
		byte Hour { get; set; }
		byte Minute { get; set; }
		byte Second { get; set; }
		public string StringDate { get; set; }

		public short KAUAddress { get; set; }
		public JournalSourceType Source { get; set; }
		public byte Code { get; set; }
		public string EventName { get; set; }
		public string EventDescription { get; set; }

		public short ObjectNo { get; set; }
		public short ObjectDeviceType { get; set; }
		public short ObjectDeviceAddress { get; set; }
		public int ObjectFactoryNo { get; set; }
		public int ObjectState { get; set; }


		public JournalItem(List<byte> bytes)
		{
			GKNo = ByteHelper.ToInt(bytes, 0);
			KAUNo = ByteHelper.ToInt(bytes, 32);

			Day = bytes[32 + 4];
			Month = bytes[32 + 5];
			Year = bytes[32 + 6];
			Hour = bytes[32 + 7];
			Minute = bytes[32 + 8];
			Second = bytes[32 + 9];
			StringDate = Day.ToString() + "/" + Month.ToString() + "/" + Year.ToString() + " " + Hour.ToString() + ":" + Minute.ToString() + ":" + Second.ToString();

			KAUAddress = ByteHelper.ToShort(bytes, 32 + 10);
			Source = (JournalSourceType)(int)(bytes[32 + 12]);
			Code = bytes[32 + 13];

			switch (Source)
			{
				case JournalSourceType.Controller:
					switch (Code)
					{
						case 0:
							EventName = "технология";
							break;

						case 1:
							EventName = "очистка журнала";
							break;

						case 2:
							EventName = "установка часов";
							break;

						case 3:
							EventName = "запись информации о блоке";
							break;

						case 4:
							EventName = "смена ПО";
							break;

						case 5:
							EventName = "смена БД";
							break;

						case 6:
							EventName = "работа";
							break;
					}
					break;

				case JournalSourceType.Device:
					switch (Code)
					{
						case 0:
							EventName = "неизвестный тип";
							break;

						case 1:
							EventName = "устройство с таким адресом не описано при конфигурации";
							break;
					}
					break;

				case JournalSourceType.Object:
					ObjectNo = ByteHelper.ToShort(bytes, 32 + 18);
					ObjectDeviceType = ByteHelper.ToShort(bytes, 32 + 20);
					ObjectDeviceAddress = ByteHelper.ToShort(bytes, 32 + 22);
					ObjectFactoryNo = ByteHelper.ToInt(bytes, 32 + 24);
					ObjectState = ByteHelper.ToInt(bytes, 32 + 28);
					switch (Code)
					{
						case 0:
							EventName = "при конфигурации описан другой тип";
							EventDescription = ByteHelper.ToShort(bytes, 32 + 14).ToString();
							break;
						case 1:
							EventName = "изменился заводской номер";
							EventDescription = ByteHelper.ToInt(bytes, 32 + 14).ToString();
							break;
						case 2:
							EventName = "пожар" + StringHelper.ToYesNo(bytes[32 + 14]);
							EventDescription = StringHelper.ToFire(bytes[32 + 15]);
							break;

						case 3:
							EventName = "пожар-2" + StringHelper.ToYesNo(bytes[32 + 14]);
							break;

						case 4:
							EventName = "внимание" + StringHelper.ToYesNo(bytes[32 + 14]);
							break;

						case 5:
							EventName = "неисправность" + StringHelper.ToYesNo(bytes[32 + 14]);
							EventDescription = StringHelper.ToFailure(bytes[32 + 15]);
							break;

						case 6:
							EventName = "тест" + StringHelper.ToYesNo(bytes[32 + 14]);
							EventDescription = StringHelper.ToTest(bytes[32 + 15]);
							break;

						case 7:
							EventName = "запыленность" + StringHelper.ToYesNo(bytes[32 + 14]);
							EventDescription = StringHelper.ToDustinness(bytes[32 + 15]);
							break;

						case 8:
							EventName = "управление" + StringHelper.ToYesNo(bytes[32 + 14]);
							EventDescription = StringHelper.ToControl(bytes[32 + 15]);
							break;

						case 9:
							EventName = "состояние" + StringHelper.ToYesNo(bytes[32 + 14]);
							EventDescription = StringHelper.ToState(bytes[32 + 15]);
							break;

						case 10:
							EventName = "режим работы" + StringHelper.ToRegime(bytes[32 + 14]);
							break;

						case 11:
							EventName = "дежурный" + StringHelper.ToYesNo(bytes[32 + 14]);
							break;

						case 12:
							EventName = "обход" + StringHelper.ToYesNo(bytes[32 + 14]);
							break;
					}
					break;
			}
		}
	}

	public static class ByteHelper
	{
		public static int ToInt(List<byte> bytes, int startByte)
		{
			var result = 1 * bytes[startByte + 0] + 256 * bytes[startByte + 1] + 256 * 256 * bytes[startByte + 2] + 256 * 256 * 256 * bytes[startByte + 3];
			return result;
		}

		public static short ToShort(List<byte> bytes, int startByte)
		{
			var result = 1 * bytes[startByte + 0] + 256 * bytes[startByte + 1];
			return (short)result;
		}
	}

	public static class StringHelper
	{
		public static string ToYesNo(byte b)
		{
			if (b == 0)
				return " нет";
			if (b == 0)
				return " есть";
			return "";
		}

		public static string ToRegime(byte b)
		{
			switch (b)
			{
				case 0: return "автомат";
				case 1: return "ручной";
				case 2: return "отключен";
				case 3: return "неопределен";
			}
			return "";
		}

		public static string ToFire(byte b)
		{
			switch (b)
			{
				case 1: return "ручник сорван";
				case 2: return "срабатывание по дыму";
				case 3: return "срабатывание по температуре";
				case 4: return "срабатывание по градиенту температуры";
			}
			return "";
		}

		public static string ToFailure(byte b)
		{
			switch (b)
			{
				case 1: return "неисправность питания";
				case 2: return "неисправность оптического канала или фотоусилителя";
				case 3: return "неисправность температурного канала";
				case 4: return "кз ШС";
				case 5: return "обрыв ШС";
				case 6: return "состояние датчика давления";
				case 7: return "состояние датчика массы";
				case 8: return "вскрытие";
				case 9: return "реле не реагирует на команды (контакт не переключается)";
				case 10: return "напряжение запуска реле ниже нормы";
				case 11: return "кз выхода";
				case 12: return "обрыв выхода";
				case 13: return "напряжение питания ШС ниже нормы";
				case 14: return "ошибка памяти";
				case 15: return "кз выхода 1";
				case 16: return "кз выхода 2";
				case 17: return "кз выхода 3";
				case 18: return "кз выхода 4";
				case 19: return "кз выхода 5";
				case 20: return "обрыв выхода 1";
				case 21: return "обрыв выхода 2";
				case 22: return "обрыв выхода 3";
				case 23: return "обрыв выхода 4";
				case 24: return "обрыв выхода 5";
				case 25: return "несовместимость команд";
				case 26: return "низкое напряжение питания привода";
				case 27: return "обрыв в цепи НОРМА";
				case 28: return "кз  в цепи НОРМА";
				case 29: return "обрыв  в цепи ЗАЩИТА";
				case 30: return "кз  в цепи ЗАЩИТА";
				case 31: return "обрыв  в цепи ОТКРЫТО";
				case 32: return "обрыв  в цепи ЗАКРЫТО";
				case 33: return "обрыв в цепи ДВИГАТЕЛЬ";
				case 34: return "замкнуты/разомкнуты оба концевика";
				case 35: return "превышение времени хода";
				case 36: return "обрыв в линии РЕЛЕ";
				case 37: return "кз в линии РЕЛЕ";
				case 38: return "неисправность выхода 1";
				case 39: return "неисправность выхода 2";
				case 40: return "неисправность выхода 3";
				case 41: return "нет питания на вводе";
				case 42: return "обрыв шлейфа с концевого выключателя ОТКРЫТО";
				case 43: return "кз шлейфа с концевого выключателя ОТКРЫТО";
				case 44: return "обрыв шлейфа с муфтового выключателя ОТКРЫТО";
				case 45: return "кз шлейфа с муфтового выключателя ОТКРЫТО";
				case 46: return "обрыв шлейфа с концевого выключателя ЗАКРЫТО";
				case 47: return "кз шлейфа с концевого выключателя ЗАКРЫТО";
				case 48: return "обрыв шлейфа с муфтового выключателя ЗАКРЫТО/ДУ ЗАКРЫТЬ";
				case 49: return "кз шлейфа с муфтового выключателя ЗАКРЫТО/ ДУ ЗАКРЫТЬ";
				case 50: return "обрыв шлейфа с муфтового выключателя ОТКРЫТЬ УЗЗ/ЗАКРЫТЬ УЗЗ";
				case 51: return "кз шлейфа с муфтового выключателя ОТКРЫТЬ УЗЗ/ЗАКРЫТЬ УЗЗ";
				case 52: return "обрыв шлейфа с муфтового выключателя СТОП УЗЗ";
				case 53: return "кз шлейфа с муфтового выключателя СТОП УЗЗ";
				case 57: return "неисправность КВ/МВ";
				case 58: return "не задан режим";
				case 59: return "отказ ШУЗ";
				case 60: return "неисправность ДУ/ДД";
				case 61: return "обрыв вх 9";
				case 62: return "кз вх 9";
				case 63: return "обрыв вх 10";
				case 64: return "кз вх 10";
				case 65: return "обрыв вх 11";
				case 66: return "кз вх 11";
				case 67: return "обрыв вх 12";
				case 68: return "кз вх 12";
				case 69: return "не задан тип";
				case 70: return "отказ ПН";
				case 71: return "отказ ШУН";
				case 72: return "неисправность питания основного";
				case 73: return "неисправность питания резервного";
				case 74: return "неисправность шлейфа 1, 2";
				case 75: return "неисправность шлейфа 3, 4";
				case 76: return "неисправность шлейфа 5, 6";
				case 77: return "неисправность шлейфа 7, 8";
				case 255: return "потеря связи";
			}
			return "";
		}

		public static string ToTest(byte b)
		{
			switch (b)
			{
				case 1: return "кнопка";
				case 2: return "указка";
			}
			return "";
		}

		public static string ToDustinness(byte b)
		{
			switch (b)
			{
				case 1: return "предварительная";
				case 2: return "критическая";
			}
			return "";
		}

		public static string ToControl(byte b)
		{
			switch (b)
			{
				case 1: return "пуск";
				case 2: return "отмена задержки";
				case 3: return "аналоговый вход - память; источник сигнала)";
				case 4: return "выключить";
				case 5: return "стоп";
				case 6: return "запрет пуска";
			}
			return "";
		}

		public static string ToState(byte b)
		{
			switch (b)
			{
				case 1: return "отсчет задержки";
				case 2: return "включено";
				case 3: return "выключено";
				case 4: return "включается";
				case 5: return "выключается";
				case 6: return "кнопка (0 — ППКП ; источник команды)";
				case 7: return "изменение автоматики по неисправности";
				case 8: return "изменение автоматики по кнопке СТОП";
				case 9: return "изменение автоматики по датчику ДВЕРИ-ОКНА";
				case 10: return "изменение автоматики по ТМ";
				case 11: return "автоматика включена";
				case 12: return "ручной пуск АУП от ИПР";
				case 13: return "отложенный пуск АУП по датчику ДВЕРИ-ОКНА";
				case 14: return "пуск АУП завершен";
				case 15: return "останов тушения по кнопке СТОП";
				case 16: return "программирование мастер-ключа";
				case 17: return "отсчет удержания";
				case 18: return "уровень высокий";
				case 19: return "уровень низкий";
				case 20: return "ход по команде с УЗЗ";
				case 21: return "у ДУ сообщение ПУСК НЕВОЗМОЖЕН";
				case 22: return "авария пневмоемкости";
				case 23: return "уровень аварийный";
				case 24: return "запрет пуска НС";
				case 25: return "запрет пуска компрессора";
				case 26: return "команда с УЗН";
				case 27: return "перевод в режим ручного управления";
			}
			return "";
		}
	}
}