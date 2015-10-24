using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ResursAPI.Models;

namespace ResursNetwork.OSI.ApplicationLayer.Devices
{
    public class DeviceErrorOccuredEventArgs: EventArgs
    {
		private Guid _Id;
        private DeviceErrors _Errors;

		public Guid Id
		{
			get { return _Id; }
			set { _Id = value; }
		}

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
