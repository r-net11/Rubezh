using System.ComponentModel;
using LocalizationConveters;

namespace FiresecAPI.Models
{
	public enum RemoteAccessType
	{
        //[DescriptionAttribute("Запрещен")]
        [LocalizedDescription(typeof(Resources.Language.Security.RemoteAccessType), "RemoteAccessBanned")]
		RemoteAccessBanned,

        //[DescriptionAttribute("Разрешен с любых компьютеров")]
        [LocalizedDescription(typeof(Resources.Language.Security.RemoteAccessType), "RemoteAccessAllowed")]
		RemoteAccessAllowed,

		//[DescriptionAttribute("Разрешен только с указанных компьютеров")]
        [LocalizedDescription(typeof(Resources.Language.Security.RemoteAccessType), "SelectivelyAllowed")]
		SelectivelyAllowed
	}
}