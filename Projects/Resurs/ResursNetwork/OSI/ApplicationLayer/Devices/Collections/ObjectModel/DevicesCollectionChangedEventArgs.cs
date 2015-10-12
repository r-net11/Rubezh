using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using ResursNetwork.OSI.ApplicationLayer.Devices;

namespace ResursNetwork.OSI.ApplicationLayer.Devices.Collections.ObjectModel
{
    public class DevicesCollectionChangedEventArgs: EventArgs
    {
        #region Fields And Properties

        private IDevice _Device;
        private NotifyCollectionChangedAction _Action;

        public IDevice Device
        {
            get { return _Device; }
            set { _Device = value; }
        }

        public NotifyCollectionChangedAction Action
        {
            get { return _Action; }
            set { _Action = value; }
        }
        #endregion
    }
}
