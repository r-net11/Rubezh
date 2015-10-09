using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ResursAPI.Models;

namespace ResursNetwork.OSI.ApplicationLayer.Devices
{
    public class ErrorOccuredEventArgs: EventArgs
    {
        private DeviceErrors _Errors;

        /// <summary>
        /// Структура с флагами ошибок устройтсва
        /// </summary>
        public DeviceErrors Errors
        {
            get { return _Errors; }
            set { _Errors = value; }
        }
    }
}
