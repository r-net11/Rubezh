using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ResursAPI.Models
{
    /// <summary>
    /// Хранит флаги ошибок устройтсва
    /// </summary>
    public struct DeviceErrors
    {
        #region Fields And Properties

        public bool HasErrors
        {
            get { return (CommunicationError || ConfigurationError || RTCError) ? true : false; }
        }

        /// <summary>
        /// Ошибка связи
        /// </summary>
        public bool CommunicationError;

        /// <summary>
        /// Ошибка конфигурации
        /// </summary>
        public bool ConfigurationError;

        /// <summary>
        /// Часы не исправны
        /// </summary>
        public bool RTCError;

        #endregion

        #region Methods
        public void Reset()
        {
            CommunicationError = false;
            ConfigurationError = false;
            RTCError = false;
        }

        public static bool operator == (DeviceErrors str1, DeviceErrors str2)
        {
            if ((str1.CommunicationError == str2.CommunicationError) &&
                (str1.ConfigurationError == str2.ConfigurationError) &&
                (str1.RTCError == str2.RTCError))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public static bool operator != (DeviceErrors str1, DeviceErrors str2)
        {
            return (str1 == str2) ? false : true;
        }

        #endregion
    }
}
