using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;

namespace FiresecAPI.Models
{
    public enum ReportType
    {
        [DescriptionAttribute("Блоки индикации")]
        ReportIndicationBlock,

        [DescriptionAttribute("Журнал событий")]
        ReportJournal,

        [DescriptionAttribute("Количество устройств по типам")]
        ReportDriverCounter,

        [DescriptionAttribute("Параметры устройств")]
        ReportDeviceParams,

        [DescriptionAttribute("Список устройств")]
        ReportDevicesList
    }
}
