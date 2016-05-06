using System.ComponentModel;

namespace StrazhAPI.Models
{
	public enum RemoteAccessType
	{
		[Description("Запрещен")]
		RemoteAccessBanned,

		[Description("Разрешен с любых компьютеров")]
		RemoteAccessAllowed,

		[Description("Разрешен только с указанных компьютеров")]
		SelectivelyAllowed
	}
}