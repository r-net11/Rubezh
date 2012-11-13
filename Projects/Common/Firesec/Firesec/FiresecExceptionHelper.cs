namespace Firesec
{
	internal class FiresecExceptionHelper
	{
		public static bool IsWellKnownComException(string name)
		{
			switch (name)
			{
				case "Разрушительный сбой":
				case "Операция прервана":
				case "Не могу преобразовать вариант типа (Array Variant) в тип (OleStr)":
				case "Прибор не отвечает":
				case "Обновление завершилось неудачно. Повторите обновление":
				case "Операция записи БД окончилась неудачей. Повторите запись":
				case "Управление устройством невозможно. Нет связи с прибором":
				case "USB устройство отсутствует":
				case "Член группы не найден":
				case "Ошибка":
				case "Ошибка записи потока":
				case "Версия 5 базы в блоке индикации не поддерживается. Обновите FireSec":
				case "Неверный аргумент":
                case "Catastrophic failure":
				case "ERR: Адрес (номер параметра, подфункция) является недопустимым":
				case "Версия 3 базы в ПДУ не поддерживается. Обновите FireSec":
				case "Указанный прибор не имеет модели базы данных":
				case "Устройство относится к нескольким направлениям тушения":
					return true;
			}
			if (name.StartsWith("Предотвращена возможная взаимная блокировка"))
				return true;
			return false;
		}

		public static bool IsWellKnownInvalidComObjectException(string name)
		{
			switch (name)
			{
				case "Объект COM, который был отделен от своего базового RCW, использоваться не может.":
					return true;
			}
			return false;
		}

		public static bool IsWellKnownTargetParameterCountException(string name)
		{
			switch (name)
			{
				case "Недопустимое число параметров":
					return true;
			}
			return false;
		}
	}
}