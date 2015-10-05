using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ResursNetwork.Management;

namespace ResursNetwork.Networks
{
    public class StatusChangedEventArgs: EventArgs
    {
        #region Fields And Propeties

        private Status _Status;
        private Guid _Id;

        /// <summary>
        /// Идентификатор устройства или сетевого контроллера (интерфейса)
        /// </summary>
        public Guid Id
        {
            get { return _Id; }
            set { _Id = value; }
        }

        /// <summary>
        /// Статус устройтства или сетевого контроллера (интерфейса)
        /// </summary>
        public Status Status
        {
            get { return _Status; }
            set { _Status = value; }
        }

        #endregion
    }
}
