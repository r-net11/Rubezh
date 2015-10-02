using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ResursNetwork.Incotex.NetworkControllers.ApplicationLayer
{
    /// <summary>
    /// Режим опроса сетевых устройств
    /// </summary>
    public enum PollingMode
    {
        /// <summary>
        /// Основной режим, контроллер по рассписанию опрашивает
        /// все устройства в сети
        /// </summary>
        General,
        /// <summary>
        /// Режим, когда опрашивается единственное устройство в режиме
        /// реального времени
        /// </summary>
        Exclusive
    }
}
