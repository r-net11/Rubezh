using System;
using System.Collections.Generic;

namespace ChinaSKDDriverAPI
{
	public class Access
	{
		public int RecordNo { get; set; }
		public string CardNo { get; set; }
		public string Password { get; set; }
		public DateTime Time { get; set; }
		public bool Status { get; set; }
		public AccessMethodType MethodType { get; set; }
		public int ReaderID { get; set; }
		public int DoorNo { get; set; }
	}

	public enum AccessMethodType
	{
		// Неизвестно
		NET_ACCESS_DOOROPEN_METHOD_UNKNOWN = 0,
		// Пароль
		NET_ACCESS_DOOROPEN_METHOD_PWD_ONLY,
		// Карта
		NET_ACCESS_DOOROPEN_METHOD_CARD,
		// Сначала карта
		NET_ACCESS_DOOROPEN_METHOD_CARD_FIRST,
		// Сначала пароль
		NET_ACCESS_DOOROPEN_METHOD_PWD_FIRST,
		// Удаленно
		NET_ACCESS_DOOROPEN_METHOD_REMOTE,
		// Кнопка
		NET_ACCESS_DOOROPEN_METHOD_BUTTON
	}
}