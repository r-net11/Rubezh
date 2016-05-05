using System;
using System.ComponentModel;

namespace StrazhDeviceSDK.API
{
	public class AccessLogItem
	{
		public int RecordNo { get; set; }

		public string CardNo { get; set; }

		public string Password { get; set; }

		public DateTime Time { get; set; }

		public bool Status { get; set; }

		public AccessMethodType MethodType { get; set; }

		public string ReaderID { get; set; }

		public int DoorNo { get; set; }

		public CardType CardType { get; set; }

		public ErrorCode ErrorCode { get; set; }
	}

	public enum AccessMethodType
	{
		[Description("Неизвестно")]
		NET_ACCESS_DOOROPEN_METHOD_UNKNOWN = 0,

		[Description("Только пароль")]
		NET_ACCESS_DOOROPEN_METHOD_PWD_ONLY,

		[Description("Карта")]
		NET_ACCESS_DOOROPEN_METHOD_CARD,

		[Description("Сначала карта")]
		NET_ACCESS_DOOROPEN_METHOD_CARD_FIRST,

		[Description("Сначала пароль")]
		NET_ACCESS_DOOROPEN_METHOD_PWD_FIRST,

		[Description("Удаленно")]
		NET_ACCESS_DOOROPEN_METHOD_REMOTE,

		[Description("Кнопка")]
		NET_ACCESS_DOOROPEN_METHOD_BUTTON
	}
}