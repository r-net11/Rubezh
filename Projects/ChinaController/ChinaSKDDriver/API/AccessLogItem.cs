using System;
using System.ComponentModel;
using Localization.Converters;
using Localization.StrazhDeviceSDK.Common;

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
		//[Description("Неизвестно")]
		[LocalizedDescription(typeof(CommonResources),"Unknown")]
		NET_ACCESS_DOOROPEN_METHOD_UNKNOWN = 0,

		//[Description("Только пароль")]
		[LocalizedDescription(typeof(CommonResources), "PasswordOnly")]
		NET_ACCESS_DOOROPEN_METHOD_PWD_ONLY,

		//[Description("Карта")]
		[LocalizedDescription(typeof(CommonResources), "Passcard")]
		NET_ACCESS_DOOROPEN_METHOD_CARD,

		//[Description("Сначала карта")]
		[LocalizedDescription(typeof(CommonResources), "PasscardFirst")]
		NET_ACCESS_DOOROPEN_METHOD_CARD_FIRST,

		//[Description("Сначала пароль")]
		[LocalizedDescription(typeof(CommonResources), "PasswordFirst")]
		NET_ACCESS_DOOROPEN_METHOD_PWD_FIRST,

		//[Description("Удаленно")]
		[LocalizedDescription(typeof(CommonResources), "Remote")]
		NET_ACCESS_DOOROPEN_METHOD_REMOTE,

		//[Description("Кнопка")]
		[LocalizedDescription(typeof(CommonResources), "Button")]
		NET_ACCESS_DOOROPEN_METHOD_BUTTON
	}
}