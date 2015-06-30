﻿using System.Collections.Generic;

namespace DeviveModelManager
{
	public static class CommonStatesHelper
	{
		public static List<string> States
		{
			get
			{
				var states = new List<string>();
				states.Add("Тревога");
				states.Add("Внимание (предтревожное)");
				states.Add("Неисправность");
				states.Add("Требуется обслуживание");
				states.Add("Обход устройств");
				states.Add("Неопределено");
				states.Add("Норма(*)");
				states.Add("Норма");
				states.Add("Отсутствует в конфигурации сервера оборудования");
				states.Add("Нет связи с сервером оборудования");
				return states;
			}
		}
	}
}