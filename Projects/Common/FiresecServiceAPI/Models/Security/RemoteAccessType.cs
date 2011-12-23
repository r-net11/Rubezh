using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;

namespace FiresecAPI.Models
{
    public enum RemoteAccessType
    {
        [DescriptionAttribute("Запрещен")]
        RemoteAccessBanned,

        [DescriptionAttribute("Резрешен с любых компьютеров")]
        RemoteAccessAllowed,

        [DescriptionAttribute("Разрешен только с указанных компьютеров")]
        SelectivelyAllowed
    }
}
