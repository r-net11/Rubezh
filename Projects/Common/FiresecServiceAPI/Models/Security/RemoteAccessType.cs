using System.ComponentModel;

namespace FiresecAPI.Models
{
    public enum RemoteAccessType
    {
        [DescriptionAttribute("Запрещен")]
        RemoteAccessBanned,

        [DescriptionAttribute("Разрешен с любых компьютеров")]
        RemoteAccessAllowed,

        [DescriptionAttribute("Разрешен только с указанных компьютеров")]
        SelectivelyAllowed
    }
}