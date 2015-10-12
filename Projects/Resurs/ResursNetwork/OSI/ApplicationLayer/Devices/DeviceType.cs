using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ResursNetwork.OSI.ApplicationLayer.Devices
{
    /// <summary>
    /// Типы счётчиков электроэнергии
    /// </summary>
    public enum DeviceType: int
    {
        /// <summary>
        /// Неизвестное устройство
        /// </summary>
        Unknown,
        /// <summary>
        /// Счётчик электрической энергии Меркурий 203
        /// </summary>
        Mercury203
    }
}
