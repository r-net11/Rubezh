using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ResursNetwork.Devices
{
    public class ErrorOccuredEventArgs: EventArgs
    {
        private string _DescriptionError = String.Empty;
        /// <summary>
        /// Описание ошибки
        /// </summary>
        public string DescriptionError
        {
            get { return _DescriptionError; }
            set { _DescriptionError = value; }
        }
    }
}
